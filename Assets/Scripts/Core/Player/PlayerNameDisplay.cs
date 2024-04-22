using System;
using TMPro;
using Unity.Collections;
using UnityEngine;

namespace Core.Player
{
    public class PlayerNameDisplay : MonoBehaviour
    {
        [SerializeField] private TankPlayer player;
        [SerializeField] private TMP_Text playerNameText;
        
        private void Start()
        {
            HandlePlayerNameChanged(string.Empty, player.PlayerName.Value);
            
            player.PlayerName.OnValueChanged += HandlePlayerNameChanged;
        }

        private void HandlePlayerNameChanged(FixedString32Bytes oldName, FixedString32Bytes newName)
        {
            playerNameText.text = newName.ToString();
        }

        private void OnDestroy()
        {
            player.PlayerName.OnValueChanged -= HandlePlayerNameChanged;
        }
    }
}
