using System;
using TMPro;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Serialization;

namespace UI.Leaderboard
{
    public class LeaderboardEntityDisplay : MonoBehaviour
    {
        [SerializeField] private TMP_Text displayText;

        private FixedString32Bytes _playerName;
        public ulong ClientId { get; private set; }
        public int Coins { get; private set; }

        public void Initialise(ulong clientId, FixedString32Bytes playerName, int coins)
        {
            ClientId = clientId;
            _playerName = playerName;
            
            UpdateCoins(coins);
        }

        public void UpdateCoins(int coins)
        {
            Coins = coins;
            
            UpdateText();
        }
        
        private void UpdateText()
        {
            displayText.text = $"1. {_playerName} - {Coins}";
        }
    }
}
