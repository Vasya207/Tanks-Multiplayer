using System;
using Unity.Netcode;
using UnityEngine;

namespace Core.Combat
{
    public class Health : NetworkBehaviour
    {
        [field: SerializeField] public int MaxHealth { get; private set; } = 100;
        
        public NetworkVariable<int> CurrentHealth = new NetworkVariable<int>();
        private bool _isDead;
        public Action<Health> OnDie;

        public override void OnNetworkSpawn()
        {
            if (!IsServer) return;
            
            CurrentHealth.Value = MaxHealth;
        }

        public void TakeDamage(int damageValue)
        {
            ModifyHealth(-damageValue);
        }

        public void RestoreHealth(int healValue)
        {
            ModifyHealth(healValue);
        }

        private void ModifyHealth(int value)
        {
            if (_isDead) return;

            var newHealth = CurrentHealth.Value + value;
            CurrentHealth.Value = Mathf.Clamp(newHealth, 0, MaxHealth);
            
            if (CurrentHealth.Value == 0)
            {
                OnDie?.Invoke(this);
                _isDead = true;
            }
        }
    }
}
