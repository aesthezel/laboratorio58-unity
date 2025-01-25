using SocketClient.Runtime.Client;
using UnityEngine;

namespace SocketClient.Runtime.Entities
{
    public class PlayerNetworkController : MonoBehaviour
    {
        private ClientBuilder _client;
        private int _id;
        private Vector3 _lastPosition;

        public void InjectClient(ClientBuilder client, int clientId)
        {
            _client = client;
            _id = clientId;
        }

        private void Update()
        {
            if (_id == ClientBuilder.ClientId)
            {
                // Solo el jugador local tiene control sobre su movimiento
                HandleLocalInput();
                SendPositionToServer();
            }
            else
            {
                // Jugadores remotos no manejan inputs
                // Movimiento ya se actualiza en ProcessGameState
            }
        }

        private void HandleLocalInput()
        {
            if (Input.GetKey(KeyCode.W))
                transform.Translate(Vector3.forward * Time.deltaTime * 5);
            if (Input.GetKey(KeyCode.S))
                transform.Translate(Vector3.back * Time.deltaTime * 5);
            if (Input.GetKey(KeyCode.A))
                transform.Translate(Vector3.left * Time.deltaTime * 5);
            if (Input.GetKey(KeyCode.D))
                transform.Translate(Vector3.right * Time.deltaTime * 5);
            if (Input.GetKey(KeyCode.Space))
                transform.Translate(Vector3.up * Time.deltaTime * 5);
        }

        private void SendPositionToServer()
        {
            var newPosition = transform.position;

            if (!(Vector3.Distance(_lastPosition, newPosition) > 0.01f)) return;
            _client.SendPlayerPosition(newPosition);
            _lastPosition = newPosition;
        }
    }
}