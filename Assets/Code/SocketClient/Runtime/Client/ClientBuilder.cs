using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Cinemachine;
using Newtonsoft.Json;
using SocketClient.Runtime.Entities;
using SocketClient.Runtime.Entities.View;
using SocketClient.Runtime.Models;
using TMPro;
using UnityEngine;

namespace SocketClient.Runtime.Client
{
    public class ClientBuilder : MonoBehaviour
    {
        [SerializeField]
        private CinemachineVirtualCamera virtualCamera;

        [SerializeField] private TMP_Text clientIdText;

        private TcpClient client;
        private NetworkStream stream;
        private readonly byte[] buffer = new byte[4096];

        public string serverIP = "127.0.0.1";
        public int serverPort = 9000;

        public GameObject playerPrefab;
        private readonly Dictionary<int, GameObject> _players = new();

        public static int ClientId = -1;

        private void Start()
        {
            ConnectToServer();
        }

        private async void ConnectToServer()
        {
            try
            {
                client = new TcpClient();
                await client.ConnectAsync(serverIP, serverPort);
                stream = client.GetStream();
                Debug.Log("Conectado al servidor");

                // Escuchar mensajes
                _ = ListenForMessages();
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error al conectar: {ex.Message}");
            }
        }

        private async Task ListenForMessages()
        {
            var partialMessage = new StringBuilder();

            while (client.Connected)
            {
                try
                {
                    var bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                    if (bytesRead <= 0) continue;

                    var received = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    partialMessage.Append(received);

                    // Procesa mensajes completos terminados en '\n'
                    while (partialMessage.ToString().Contains("\n"))
                    {
                        var messages = partialMessage.ToString().Split('\n');
                        for (int i = 0; i < messages.Length - 1; i++) // Procesar todos menos el último parcial
                        {
                            var message = messages[i].Trim();
                            ProcessMessage(message);
                        }

                        // Retiene el último fragmento parcial
                        partialMessage = new StringBuilder(messages[^1]);
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Error al procesar mensaje: {ex.Message}");
                    break;
                }
            }
        }
        
        private void ProcessMessage(string message)
        {
            try
            {
                Debug.Log($"Procesando mensaje: {message}");

                if (message.StartsWith("{") && message.Contains("\"Id\":"))
                {
                    // Mensaje de conexión del jugador
                    var playerConnection = JsonConvert.DeserializeObject<PlayerConnection>(message);
                    if (playerConnection != null)
                    {
                        HandlePlayerConnection(playerConnection);
                    }
                }
                else if (message.StartsWith("{") && message.Contains("\"InstanceId\":"))
                {
                    // Mensaje de cambio de instancia
                    ProcessInstanceChange(message);
                }
                else if (!string.IsNullOrWhiteSpace(message))
                {
                    // Mensaje de estado del juego (posiciones)
                    ProcessGameState(message);
                }
                else
                {
                    Debug.LogWarning("Mensaje vacío o desconocido ignorado.");
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error procesando mensaje: {ex.Message}");
            }
        }
        
        private bool IsJson(string message)
        {
            message = message.Trim();
            return (message.StartsWith("{") && message.EndsWith("}")) ||
                   (message.StartsWith("[") && message.EndsWith("]"));
        }

        
        private void HandlePlayerConnection(PlayerConnection connection)
        {
            ClientId = connection.Id;
            clientIdText.text = ClientId.ToString();

            if (_players.ContainsKey(ClientId)) return;

            var localPlayer = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
            localPlayer.GetComponent<PlayerNetworkController>().InjectClient(this, ClientId);
            localPlayer.GetComponent<InstanceChangeUIView>().InjectClient(this, ClientId);
            _players[ClientId] = localPlayer;

            virtualCamera.Follow = localPlayer.transform;
            virtualCamera.LookAt = localPlayer.transform;

            Debug.Log($"Cliente conectado con ID: {ClientId}");
        }

        private void ProcessGameState(string gameState)
        {
            if (string.IsNullOrWhiteSpace(gameState)) return;

            // Almacena los jugadores presentes en el estado recibido
            HashSet<int> currentPlayers = new();

            string[] playersData = gameState.Split('|');
            foreach (string playerData in playersData)
            {
                if (string.IsNullOrWhiteSpace(playerData)) continue;

                var parts = playerData.Split(',');
                int id = int.Parse(parts[0]);
                float x = RoundToPrecision(float.Parse(parts[1]), 2);
                float y = RoundToPrecision(float.Parse(parts[2]), 2);
                float z = RoundToPrecision(float.Parse(parts[3]), 2);

                currentPlayers.Add(id);

                if (id == ClientId) continue;

                if (!_players.TryGetValue(id, out var player))
                {
                    // Instancia un nuevo jugador para clientes remotos
                    var newPlayer = Instantiate(playerPrefab, new Vector3(x, y, z), Quaternion.identity);
                    _players[id] = newPlayer;
                }
                else
                {
                    // Actualiza la posición de los jugadores remotos
                    player.transform.position = new Vector3(x, y, z);
                }
            }

            // Elimina jugadores que ya no están presentes en el estado
            var disconnectedPlayers = new List<int>();
            foreach (var id in _players.Keys)
            {
                if (!currentPlayers.Contains(id))
                {
                    disconnectedPlayers.Add(id);
                }
            }

            foreach (var id in disconnectedPlayers)
            {
                Destroy(_players[id]);
                _players.Remove(id);
                Debug.Log($"Jugador {id} eliminado (desconectado o cambiado de instancia)");
            }
        }

        public async void SendPlayerPosition(Vector3 position)
        {
            try
            {
                if (client?.Connected != true) return;
                string message = $"{position.x},{position.y},{position.z}\n";
                byte[] data = Encoding.UTF8.GetBytes(message);
                await stream.WriteAsync(data, 0, data.Length);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error procesando la petición {e.Message}");
            }
        }
        
        public async void RequestInstanceChange(int targetInstanceId)
        {
            try
            {
                if (client?.Connected != true) return;

                string message = $"CHANGE_INSTANCE:{targetInstanceId}\n";
                byte[] data = Encoding.UTF8.GetBytes(message);
                await stream.WriteAsync(data, 0, data.Length);

                Debug.Log($"Solicitud de cambio a la instancia {targetInstanceId} enviada.");
            }
            catch (Exception e)
            {
                Debug.LogError($"Error enviando solicitud de cambio de instancia: {e.Message}");
            }
        }
        
        private void ProcessInstanceChange(string message)
        {
            try
            {
                // Deserializa el mensaje recibido
                var response = JsonConvert.DeserializeObject<Instance>(message);

                if (response == null)
                {
                    Debug.LogError("La respuesta deserializada como InstanceResponse es nula.");
                    return;
                }

                int newInstanceId = response.InstanceId;
                var playersData = response.Players;

                Debug.Log($"Cambiando a la instancia {newInstanceId}");

                // Limpia los jugadores actuales
                foreach (var player in _players.Values)
                {
                    Destroy(player);
                }
                _players.Clear();

                // Instancia a los jugadores de la nueva instancia
                foreach (var player in playersData)
                {
                    var newPlayer = Instantiate(playerPrefab, new Vector3(player.X, player.Y, player.Z), Quaternion.identity);
                    _players[player.Id] = newPlayer;

                    if (player.Id == ClientId)
                    {
                        // Configura al jugador local
                        virtualCamera.Follow = newPlayer.transform;
                        virtualCamera.LookAt = newPlayer.transform;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error procesando cambio de instancia: {ex.Message}");
            }
        }


        private void OnDestroy()
        {
            stream?.Close();
            client?.Close();
        }

        private float RoundToPrecision(float value, int precision)
        {
            float factor = Mathf.Pow(10, precision);
            return Mathf.Round(value * factor) / factor;
        }
    }
}