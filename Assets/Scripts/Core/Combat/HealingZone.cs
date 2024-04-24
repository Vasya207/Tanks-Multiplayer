using System;
using System.Collections.Generic;
using Core.Player;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace Core.Combat
{
    public class HealingZone : NetworkBehaviour
    {
        [Header("References")]
        [SerializeField] private Image healPowerBar;

        [Header("Settings")] 
        [SerializeField] private int maxHealPower = 30;
        [SerializeField] private float healCooldown = 60f;
        [SerializeField] private float healTickRate = 1f;
        [SerializeField] private int coinsPerTick = 10;
        [SerializeField] private int healthPerTick = 10;

        private List<TankPlayer> _playersInZone = new List<TankPlayer>();

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!IsServer) return;

            if (!other.attachedRigidbody.TryGetComponent(out TankPlayer tankPlayer)) return;
            
            _playersInZone.Add(tankPlayer);
            Debug.Log($"Entered: {tankPlayer.PlayerName.Value}");
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (!IsServer) return;
            
            if (!other.attachedRigidbody.TryGetComponent(out TankPlayer tankPlayer)) return;
            
            _playersInZone.Remove(tankPlayer);
            Debug.Log($"Left: {tankPlayer.PlayerName.Value}");

        }
    }
}
