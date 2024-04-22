using Networking.Shared;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Networking.Client
{
    public class NetworkClient
    {
        private NetworkManager _networkManager;
        private const string MenuSceneName = "Menu";
        
        public NetworkClient(NetworkManager networkManager)
        {
            _networkManager = networkManager;

            _networkManager.OnClientDisconnectCallback += OnClientDisconnect;
        }

        private void OnClientDisconnect(ulong clientId)
        {
            if (clientId != 0 && clientId != _networkManager.LocalClientId) return;

            if (SceneManager.GetActiveScene().name != MenuSceneName)
            {
                SceneManager.LoadScene(MenuSceneName);
            }

            if (_networkManager.IsConnectedClient)
            {
                _networkManager.Shutdown();
            }
        }
    }
}
