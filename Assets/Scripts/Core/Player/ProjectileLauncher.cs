using System;
using Input;
using Unity.Netcode;
using UnityEngine;

namespace Core.Player
{
    public class ProjectileLauncher : NetworkBehaviour
    {
        [Header("References")]
        [SerializeField] private InputReader inputReader;
        [SerializeField] private Transform projectileSpawnPoint;
        [SerializeField] private GameObject serverProjectilePrefab;
        [SerializeField] private GameObject clientProjectilePrefab;

        [Header("Settings")] 
        [SerializeField] private float projectileSpeed;

        private bool _isFiring;

        public override void OnNetworkSpawn()
        {
            if (!IsOwner) return;

            inputReader.PrimaryFireEvent += HandlePrimaryFire;
        }

        public override void OnNetworkDespawn()
        {
            if (!IsOwner) return;

            inputReader.PrimaryFireEvent -= HandlePrimaryFire;
        }

        private void Update()
        {
            if (!IsOwner || !_isFiring) return;
            
            PrimaryFireServerRpc(projectileSpawnPoint.position, projectileSpawnPoint.up);
            
            SpawnDummyProjectile(projectileSpawnPoint.position, projectileSpawnPoint.up);
        }

        private void HandlePrimaryFire(bool shouldFire)
        {
            _isFiring = shouldFire;
        }

        [ServerRpc]
        private void PrimaryFireServerRpc(Vector3 spawnPos, Vector3 direction)
        {
            GameObject projectileInstance = 
                Instantiate(serverProjectilePrefab, spawnPos, Quaternion.identity);
            
            projectileInstance.transform.up = direction;
            
            SpawnDummyProjectileClientRpc(spawnPos, direction);
        }

        [ClientRpc]
        private void SpawnDummyProjectileClientRpc(Vector3 spawnPos, Vector3 direction)
        {
            if(IsOwner) return;
            
            SpawnDummyProjectile(spawnPos, direction);
        }
        
        private void SpawnDummyProjectile(Vector3 spawnPos, Vector3 direction)
        {
            GameObject projectileInstance = 
                Instantiate(clientProjectilePrefab, spawnPos, Quaternion.identity);

            projectileInstance.transform.up = direction;
        }
    }
}
