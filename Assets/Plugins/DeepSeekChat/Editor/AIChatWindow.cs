using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

namespace DeepSeekChat.Editor
{
    public class AIChatWindow : EditorWindow
    {
        private string inputText = "";
        private Vector2 scrollPosition;
        private Vector2 conversationsScroll;
        private List<Conversation> pastConversations = new();
        private Conversation currentConversation;

        private GUIStyle wrappedTextStyle;
        
        private string apiKey = "";
        private bool isWaitingForResponse;

        [Serializable]
        private class ApiRequest
        {
            public string model = "deepseek-chat";
            public List<MessageData> messages = new();
            public bool stream;
        }

        [Serializable]
        private class MessageData
        {
            public string role;
            public string content;
        }

        [Serializable]
        private class ApiResponse
        {
            public List<Choice> choices;
        }

        [Serializable]
        private class Choice
        {
            public ResponseMessage message;
        }

        [Serializable]
        private class ResponseMessage
        {
            public string role;
            public string content;
        }

        private void OnGUI()
        {
            DrawAPIKeyField();
            DrawLayoutStructure();
        }

        void DrawAPIKeyField()
        {
            EditorGUILayout.Space();
            string newKey = EditorGUILayout.PasswordField("DeepSeek API Key", apiKey);
            if (newKey != apiKey)
            {
                apiKey = newKey;
                EditorPrefs.SetString("DeepSeekAPIKey", apiKey);
            }

            EditorGUILayout.Space();
        }

        private async void SendMessage()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(inputText))
                {
                    ShowError("No puedes enviar mensajes vacíos");
                    return;
                }
            
                if (!string.IsNullOrEmpty(inputText))
                {
                    currentConversation.messages.Add(new Message
                    {
                        content = inputText,
                        isUser = true,
                        timestamp = System.DateTime.Now
                    });

                    inputText = "";
                    Repaint();

                    await SendRequestToDeepSeek();
                }
            }
            catch (Exception e)
            {
                throw; // TODO handle exception
            }
        }

        async Task SendRequestToDeepSeek()
        {
            if (string.IsNullOrEmpty(apiKey))
            {
                ShowError("API Key no configurada");
                return;
            }

            isWaitingForResponse = true;
            Repaint();

            try
            {
                ApiRequest requestData = CreateRequestData();
                DebugLogRequest(requestData);
                
                string jsonData = JsonUtility.ToJson(requestData);

                using UnityWebRequest request =
                    new UnityWebRequest("https://api.deepseek.com/chat/completions", "POST");
                byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
                request.uploadHandler = new UploadHandlerRaw(bodyRaw);
                request.downloadHandler = new DownloadHandlerBuffer();
                request.SetRequestHeader("Content-Type", "application/json");
                request.SetRequestHeader("Authorization", $"Bearer {apiKey}");

                await request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.Success)
                {
                    HandleSuccessfulResponse(request.downloadHandler.text);
                }
                else
                {
                    HandleError(request.error);
                }
            }
            catch (System.Exception e)
            {
                HandleError(e.Message);
            }
            finally
            {
                isWaitingForResponse = false;
                Repaint();
            }
        }

        ApiRequest CreateRequestData()
        {
            var requestData = new ApiRequest();
    
            // Agregar mensaje de sistema solo una vez
            if (!currentConversation.hasSystemMessage)
            {
                requestData.messages.Add(new MessageData {
                    role = "system",
                    content = "You are a helpful assistant."
                });
                currentConversation.hasSystemMessage = true;
            }

            // Agregar solo los últimos 5 mensajes para evitar sobrecarga
            int startIndex = Mathf.Max(0, currentConversation.messages.Count - 5);
            for (int i = startIndex; i < currentConversation.messages.Count; i++)
            {
                var msg = currentConversation.messages[i];
                requestData.messages.Add(new MessageData {
                    role = msg.isUser ? "user" : "assistant",
                    content = msg.content
                });
            }

            return requestData;
        }

        void HandleSuccessfulResponse(string jsonResponse)
        {
            var response = JsonUtility.FromJson<ApiResponse>(jsonResponse);

            if (response?.choices == null || response.choices.Count == 0)
            {
                ShowError("Respuesta inválida de la API");
                return;
            }

            string aiResponse = response.choices[0].message.content;

            currentConversation.messages.Add(new Message {
                content = aiResponse,
                isUser = false,
                timestamp = System.DateTime.Now
            });
            
            if(currentConversation.messages.Count > 10)
            {
                currentConversation = new Conversation();
                ShowError("Se reinició la conversación por límite de mensajes");
            }
        }

        void HandleError(string error)
        {
            ShowError($"Error: {error}");
            Debug.LogError($"API Error: {error}");
        }

        void ShowError(string message)
        {
            currentConversation.messages.Add(new Message
            {
                content = message,
                isUser = false,
                timestamp = System.DateTime.Now
            });
        }

        [MenuItem("Window/DeepSeek AI Chat")]
        public static void ShowWindow()
        {
            GetWindow<AIChatWindow>("AI Chat");
        }

        private void OnEnable()
        {
            // Configuración del estilo (añadir esto)
            wrappedTextStyle = new GUIStyle(EditorStyles.label)
            {
                wordWrap = true,
                richText = true,
                padding = new RectOffset(8, 8, 4, 4),
                margin = new RectOffset(4, 4, 4, 4),
                normal =
                {
                    // Cambiar color del texto (elige el que prefieras)
                    textColor = new Color(0.2f, 0.8f, 1f) // Cyan claro
                }
            };

            wrappedTextStyle.active.textColor = wrappedTextStyle.normal.textColor;
            wrappedTextStyle.hover.textColor = wrappedTextStyle.normal.textColor;
    
            // Opcional: cambiar color de fondo
            wrappedTextStyle.normal.background = MakeTex(2, 2, new Color(0.1f, 0.1f, 0.1f, 0.2f));
        
            // Resto de la inicialización...
            LoadConversations();
            currentConversation ??= new Conversation();
        }
        
        private Texture2D MakeTex(int width, int height, Color col)
        {
            Color[] pix = new Color[width * height];
            for (int i = 0; i < pix.Length; i++)
                pix[i] = col;
    
            Texture2D result = new Texture2D(width, height);
            result.SetPixels(pix);
            result.Apply();
            return result;
        }

        void DrawLayoutStructure()
        {
            EditorGUILayout.BeginHorizontal();
            
            DrawPreviousConversations(0.2f);
            
            EditorGUILayout.BeginVertical(GUILayout.Width(position.width * 0.8f));
            {
                DrawChatHistory();
                EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
                DrawInputArea();
            }
            EditorGUILayout.EndVertical();

            EditorGUILayout.EndHorizontal();
        }

        void DrawPreviousConversations(float widthRatio)
        {
            EditorGUILayout.BeginVertical(GUILayout.Width(position.width * widthRatio));
            EditorGUILayout.LabelField("Chat Archive", EditorStyles.boldLabel);

            conversationsScroll = EditorGUILayout.BeginScrollView(conversationsScroll);
            foreach (var conversation in pastConversations)
            {
                if (GUILayout.Button(conversation.GetPreview(), GUI.skin.label))
                {
                    currentConversation = conversation;
                }
            }

            EditorGUILayout.EndScrollView();

            EditorGUILayout.EndVertical();
        }

        void DrawChatHistory()
        {
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, 
                GUILayout.Height(position.height - 150));
    
            GUILayout.BeginVertical(GUILayout.ExpandWidth(true));
            foreach (var message in currentConversation.messages)
            {
                DrawMessage(message);
            }
            GUILayout.EndVertical();

            if (isWaitingForResponse)
            {
                EditorGUILayout.HelpBox("DeepSeek está pensando...", MessageType.Info);
            }
    
            EditorGUILayout.EndScrollView();
        }

        void DrawInputArea()
        {
            EditorGUILayout.BeginVertical(GUILayout.Height(80));
            {
                inputText = EditorGUILayout.TextArea(inputText, GUILayout.ExpandHeight(true));

                EditorGUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();

                if (GUILayout.Button("Enviar", GUILayout.Width(100)) ||
                    (Event.current.type == EventType.KeyDown &&
                     Event.current.keyCode == KeyCode.Return))
                {
                    SendMessage();
                }

                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();
        }

        void DrawMessage(Message message)
        {
            GUIStyle messageStyle = message.isUser ? EditorStyles.helpBox : wrappedTextStyle;
            
            float textHeight = messageStyle.CalcHeight(
                new GUIContent(message.content), 
                position.width * 0.75f - 40
            );

            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.BeginVertical(message.isUser ? GUI.skin.box : GUI.skin.textArea);
                {
                    GUILayout.Label(message.isUser ? "Tú" : "DeepSeek", EditorStyles.miniBoldLabel);
                    
                    EditorGUILayout.SelectableLabel(
                        message.content,
                        messageStyle,
                        GUILayout.Height(textHeight),
                        GUILayout.ExpandWidth(true)
                    );
                    
                    GUILayout.Label(message.timestamp.ToString("HH:mm"), EditorStyles.miniLabel);
                }
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndHorizontal();
        
            GUILayout.Space(8);
        }

        void LoadConversations()
        {
            // TODO: Load past conversations
        }

        void SaveConversations()
        {
            // TODO: Save conversations
        }

        private void OnDisable()
        {
            SaveConversations();
        }
        
        void DebugLogRequest(ApiRequest request)
        {
            Debug.Log("Sending to DeepSeek:\n" + 
                      JsonUtility.ToJson(request, true)
                          .Replace(",", ",\n")
                          .Replace("{", "{\n"));
        }
    }

    [System.Serializable]
    public class Conversation
    {
        public List<Message> messages = new();
        public bool hasSystemMessage = false;

        public string GetPreview()
        {
            if (messages.Count > 0)
            {
                return messages[0].content.Length > 30
                    ? messages[0].content[..30] + "..."
                    : messages[0].content;
            }

            return "New conversation";
        }
    }

    [Serializable]
    public class Message
    {
        public string content;
        public bool isUser;
        public DateTime timestamp;
    }
}