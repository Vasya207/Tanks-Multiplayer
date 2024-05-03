using System;
using Core.Coins;
using Core.Combat;
using Input;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

namespace Core.Player
{
    public class ProjectileLauncher : NetworkBehaviour
    {
        [Header("References")] 
        [SerializeField] private CoinWallet coinWallet;
        [SerializeField] private InputReader inputReader;
        [SerializeField] private Transform projectileSpawnPoint;
        [SerializeField] private GameObject serverProjectilePrefab;
        [SerializeField] private GameObject clientProjectilePrefab;
        [SerializeField] private GameObject muzzleFlash;
        [SerializeField] private Collider2D playerCollider;

        [Header("Settings")] 
        [SerializeField] private float projectileSpeed;
        [SerializeField] private float fireRate;
        [SerializeField] private float muzzleFlashDuration;
        [SerializeField] private int costToFire;

        private bool _isFiring;
        private float _muzzleFlashTimer;
        private float _timer;
        private AimStick _aimStick;

        public override void OnNetworkSpawn()
        {
            if (!IsOwner) return;
            inputReader.PrimaryFireEvent += HandlePrimaryFire;
            
            _aimStick = FindObjectOfType<AimStick>();
            if(_aimStick == null)
                SceneManager.sceneLoaded += OnSceneLoaded;
            else 
                _aimStick.OnJoyStickReleased += FireOneTime;
        }

        public override void OnNetworkDespawn()
        {
            if (!IsOwner) return;
            inputReader.PrimaryFireEvent -= HandlePrimaryFire;
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void Update()
        {
            if (_muzzleFlashTimer > 0f)
            {
                _muzzleFlashTimer -= Time.deltaTime;

                if (_muzzleFlashTimer <= 0f)
                {
                    muzzleFlash.SetActive(false);
                }
            }
            
            if (!IsOwner) return;

            if(_timer > 0) _timer -= Time.deltaTime;
            
            if (!_isFiring) return;

            if (_timer > 0) return;

            if (coinWallet.TotalCoins.Value < costToFire) return;
            
            PrimaryFireServerRpc(projectileSpawnPoint.position, projectileSpawnPoint.up);
            
            SpawnDummyProjectile(projectileSpawnPoint.position, projectileSpawnPoint.up);

            _timer = 1 / fireRate;
        }

        private void HandlePrimaryFire(bool shouldFire)
        {
            _isFiring = shouldFire;
        }
        
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            _aimStick = FindObjectOfType<AimStick>();
            _aimStick.OnJoyStickReleased += FireOneTime;
        }

        private void FireOneTime()
        {
            if (_muzzleFlashTimer > 0f)
            {
                _muzzleFlashTimer -= Time.deltaTime;

                if (_muzzleFlashTimer <= 0f)
                {
                    muzzleFlash.SetActive(false);
                }
            }
            
            if (!IsOwner) return;

            if (coinWallet.TotalCoins.Value < costToFire) return;
            
            PrimaryFireServerRpc(projectileSpawnPoint.position, projectileSpawnPoint.up);
            
            SpawnDummyProjectile(projectileSpawnPoint.position, projectileSpawnPoint.up);
        }

        [ServerRpc]
        private void PrimaryFireServerRpc(Vector3 spawnPos, Vector3 direction)
        {
            if (coinWallet.TotalCoins.Value < costToFire) return;
            
            coinWallet.SpendCoins(costToFire);
            
            GameObject projectileInstance = 
                Instantiate(serverProjectilePrefab, spawnPos, Quaternion.identity);
            
            projectileInstance.transform.up = direction;
            
            Physics2D.IgnoreCollision(playerCollider, projectileInstance.GetComponent<Collider2D>());
            
            if (projectileInstance.TryGetComponent<Rigidbody2D>(out Rigidbody2D rb))
            {
                rb.velocity = rb.transform.up * projectileSpeed;
            }
            
            if (projectileInstance.TryGetComponent<DealDamageOnContact>(out DealDamageOnContact dealDamage))
            {
                dealDamage.SetOwner(OwnerClientId);
            }
            
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
            muzzleFlash.SetActive(true);
            _muzzleFlashTimer = muzzleFlashDuration;
            
            GameObject projectileInstance = 
                Instantiate(clientProjectilePrefab, spawnPos, Quaternion.identity);

            projectileInstance.transform.up = direction;
            
            Physics2D.IgnoreCollision(playerCollider, projectileInstance.GetComponent<Collider2D>());

            if (projectileInstance.TryGetComponent<DealDamageOnContact>(out DealDamageOnContact dealDamageOnContact))
            {
                dealDamageOnContact.SetOwner(OwnerClientId);
            }
            
            if (projectileInstance.TryGetComponent<Rigidbody2D>(out Rigidbody2D rb))
            {
                rb.velocity = rb.transform.up * projectileSpeed;
            }
        }
    }
}
