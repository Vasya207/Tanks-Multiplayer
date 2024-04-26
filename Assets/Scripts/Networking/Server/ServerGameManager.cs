using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Networking.Shared;
using UI;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Networking.Server
{
    public class ServerGameManager : IDisposable
    {
        private string _serverIp;
        private int _serverPort;
        private int _queryPort;
        private NetworkServer _networkServer;
        private MultiplayAllocationService _multiplayAllocationService;
        private const string GameSceneName = "Game";
        
        public ServerGameManager(string serverIP, int serverPort, int queryPort, NetworkManager manager)
        {
            _serverIp = serverIP;
            _serverPort = serverPort;
            _queryPort = queryPort;
            _networkServer = new NetworkServer(manager);
            _multiplayAllocationService = new MultiplayAllocationService();
        }
        
        public async Task StartGameServerAsync()
        {
            await _multiplayAllocationService.BeginServerCheck();

            if(!_networkServer.OpenConnection(_serverIp, _serverPort))
            {
                Debug.LogWarning("NetworkServer did not start as expected.");
                return;
            }
            
            NetworkManager.Singleton.SceneManager.LoadScene(GameSceneName, LoadSceneMode.Single);
        }
        
        public void Dispose()
        {
            _multiplayAllocationService?.Dispose();
            _networkServer?.Dispose();
        }
    }
}
