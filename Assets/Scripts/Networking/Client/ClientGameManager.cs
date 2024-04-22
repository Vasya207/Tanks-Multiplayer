using System;
using System.Text;
using System.Threading.Tasks;
using Networking.Shared;
using UI;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Networking.Client
{
    public class ClientGameManager
    {
        private JoinAllocation _allocation;
        private NetworkClient _networkClient;
        private const string MenuSceneName = "Menu";
        
        public async Task<bool> InitAsync()
        {
            await UnityServices.InitializeAsync();

            _networkClient = new NetworkClient(NetworkManager.Singleton);

            AuthState authState = await AuthenticationWrapper.DoAuth();

            if (authState == AuthState.Authenticated) return true;

            return false;
        }

        public void GoToMenu()
        {
            SceneManager.LoadScene(MenuSceneName);
        }

        public async Task StartClientAsync(string joinCode)
        {
            try
            {
                _allocation = await Relay.Instance.JoinAllocationAsync(joinCode);
            }
            catch (Exception exception)
            {
                Debug.Log(exception);
                return;
            }

            UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
            RelayServerData relayServerData = new RelayServerData(_allocation, "dtls");
            
            transport.SetRelayServerData(relayServerData);

            UserData userData = new UserData()
            {
                userName = PlayerPrefs.GetString(NameSelector.PlayerNameKey, "Missing Name"),
                userAuthId = AuthenticationService.Instance.PlayerId
            };
            
            string payload = JsonUtility.ToJson(userData);
            byte[] payloadBytes = Encoding.UTF8.GetBytes(payload);

            NetworkManager.Singleton.NetworkConfig.ConnectionData = payloadBytes;
            
            NetworkManager.Singleton.StartClient();
        }
    }
}
