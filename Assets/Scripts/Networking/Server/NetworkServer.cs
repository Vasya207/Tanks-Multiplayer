using Networking.Shared;
using Unity.Netcode;
using UnityEngine;

namespace Networking.Server
{
    public class NetworkServer
    {
        private NetworkManager _networkManager;
        
        public NetworkServer(NetworkManager networkManager)
        {
            _networkManager = networkManager;

            _networkManager.ConnectionApprovalCallback += ApprovalCheck;
        }

        private void ApprovalCheck(
            NetworkManager.ConnectionApprovalRequest request, 
            NetworkManager.ConnectionApprovalResponse response)
        {
            string payload = System.Text.Encoding.UTF8.GetString(request.Payload);
            var userData = JsonUtility.FromJson<UserData>(payload);
            
            Debug.Log(userData.UserName);

            response.Approved = true;
            response.CreatePlayerObject = true;
        }
    }
}
