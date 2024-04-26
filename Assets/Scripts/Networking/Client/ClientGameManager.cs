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
    public class ClientGameManager : IDisposable
    {
        private JoinAllocation _allocation;
        private NetworkClient _networkClient;
        private MatchplayMatchmaker _matchmaker;
        private UserData _userData;
        private const string MenuSceneName = "Menu";
        
        public async Task<bool> InitAsync()
        {
            await UnityServices.InitializeAsync();

            _networkClient = new NetworkClient(NetworkManager.Singleton);
            _matchmaker = new MatchplayMatchmaker();

            AuthState authState = await AuthenticationWrapper.DoAuth();

            if (authState == AuthState.Authenticated)
            {
                _userData = new UserData()
                {
                    userName = PlayerPrefs.GetString(NameSelector.PlayerNameKey, "Missing Name"),
                    userAuthId = AuthenticationService.Instance.PlayerId
                };
                
                return true;
            }

            return false;
        }

        public void GoToMenu()
        {
            SceneManager.LoadScene(MenuSceneName);
        }

        private void StartClient(string ip, int port)
        {
            UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
            transport.SetConnectionData(ip, (ushort)port);
            ConnectClient();
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
            ConnectClient();
        }

        private void ConnectClient()
        {
            string payload = JsonUtility.ToJson(_userData);
            byte[] payloadBytes = Encoding.UTF8.GetBytes(payload);

            NetworkManager.Singleton.NetworkConfig.ConnectionData = payloadBytes;
            
            NetworkManager.Singleton.StartClient();
        }

        public async void MatchmakeAsync(Action<MatchmakerPollingResult> onMatchmakeResponse)
        {
            if (_matchmaker.IsMatchmaking) return;

            MatchmakerPollingResult matchResult = await GetMatchAsync();
            onMatchmakeResponse?.Invoke(matchResult);
        }

        private async Task<MatchmakerPollingResult> GetMatchAsync()
        {
            MatchmakingResult matchmakingResult = await _matchmaker.Matchmake(_userData);

            if (matchmakingResult.result == MatchmakerPollingResult.Success)
            {
                StartClient(matchmakingResult.ip, matchmakingResult.port);
            }

            return matchmakingResult.result;
        }

        public void Disconnect()
        {
            _networkClient.Disconnect();
        }
        
        public void Dispose()
        {
            _networkClient?.Dispose();
        }

        public async Task CancelMatchmaking()
        {
            await _matchmaker.CancelMatchmaking();
        }
    }
}
