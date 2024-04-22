using Unity.Netcode;
using UnityEngine;
using Cinemachine;
using Networking.Host;
using Networking.Shared;
using Unity.Collections;

namespace Core.Player
{
    public class TankPlayer : NetworkBehaviour
    {
        [Header("References")]
        [SerializeField] private CinemachineVirtualCamera cinemachineVirtualCamera;
        
        [Header("Settings")]
        [SerializeField] private int ownerPriority = 15;

        public NetworkVariable<FixedString32Bytes> PlayerName = new NetworkVariable<FixedString32Bytes>();
        
        public override void OnNetworkSpawn()
        {
            if (IsServer)
            {
                UserData userData = 
                    HostSingleton.Instance.GameManager.NetworkServer.GetUserDataByClientId(OwnerClientId);

                PlayerName.Value = userData.userName;
            }
            
            if (IsOwner)
            {
                cinemachineVirtualCamera.Priority = ownerPriority;
            }
        }
    }
}
