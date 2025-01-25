using SocketClient.Runtime.Client;
using UnityEngine;

namespace SocketClient.Runtime.Entities.View
{
    public class InstanceChangeUIView : MonoBehaviour
    {
        public ClientBuilder _client;
        private int _id;
        
        public void InjectClient(ClientBuilder client, int clientId)
        {
            _client = client;
            _id = clientId;
        }

        public void ChangeToInstance(int instanceId)
        {
            //if (_id == ClientBuilder.ClientId)
                _client.RequestInstanceChange(instanceId);
        }
    }
}