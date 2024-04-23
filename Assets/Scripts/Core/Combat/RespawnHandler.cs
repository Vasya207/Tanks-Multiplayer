using System.Collections;
using Core.Player;
using Unity.Netcode;
using UnityEngine;

namespace Core.Combat
{
    public class RespawnHandler : NetworkBehaviour
    {
        [SerializeField] private NetworkObject playerPrefab;

        public override void OnNetworkSpawn()
        {
            if (!IsServer) return;

            TankPlayer[] players = FindObjectsOfType<TankPlayer>();
            foreach (var player in players)
            {
                HandlePlayerSpawned(player);
            }

            TankPlayer.OnPlayerSpawned += HandlePlayerSpawned;
            TankPlayer.OnPlayerDespawned += HandlePlayerDespawned;
        }

        public override void OnNetworkDespawn()
        {
            if (!IsServer) return;
            
            TankPlayer.OnPlayerSpawned -= HandlePlayerSpawned;
            TankPlayer.OnPlayerDespawned -= HandlePlayerDespawned;
        }
        
        private void HandlePlayerSpawned(TankPlayer player)
        {
            player.Health.OnDie += (health) => HandlePlayerDie(player);
        }
        
        private void HandlePlayerDespawned(TankPlayer player)
        {
            player.Health.OnDie -= (health) => HandlePlayerDie(player);
        }

        private void HandlePlayerDie(TankPlayer player)
        {
            Destroy(player.gameObject);

            StartCoroutine(RespawnPlayer(player.OwnerClientId));
        }

        private IEnumerator RespawnPlayer(ulong ownerClientId)
        {
            yield return null;

            NetworkObject playerInstance = Instantiate(
                playerPrefab, SpawnPoint.GetRandomSpawnPos(), Quaternion.identity);
            
            playerInstance.SpawnAsPlayerObject(ownerClientId);
        }
    }
}
