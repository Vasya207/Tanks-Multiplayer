using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;

namespace UI
{
    public class LobbyItem : MonoBehaviour
    {
        [SerializeField] private TMP_Text lobbyNameText;
        [SerializeField] private TMP_Text lobbyPlayersText;

        private LobbiesList _lobbiesList;
        private Lobby _lobby;
        
        public void Initialize(LobbiesList lobbiesList, Lobby lobby)
        {
            _lobbiesList = lobbiesList;
            _lobby = lobby;
            
            lobbyNameText.text = lobby.Name;
            lobbyPlayersText.text = $"{lobby.Players.Count} / {lobby.MaxPlayers}";
        }

        public void Join()
        {
            _lobbiesList.JoinAsync(_lobby);
        }
    }
}
