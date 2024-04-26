using System;
using Networking.Client;
using Networking.Host;
using TMPro;
using UnityEditor.Searcher;
using UnityEngine;

namespace UI
{
    public class MainMenu : MonoBehaviour
    {
        [SerializeField] private TMP_Text queueStatusText;
        [SerializeField] private TMP_Text queueTimerText;
        [SerializeField] private TMP_Text findMatchButtonText;
        [SerializeField] private TMP_InputField joinCodeField;

        private bool _isMatchmaking;
        private bool _isCancelling;

        private void Start()
        {
            if (ClientSingleton.Instance == null) return;
            
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
            
            queueStatusText.text = string.Empty;
            queueTimerText.text = string.Empty;
        }

        public async void FindMatchPressed()
        {
            if (_isCancelling) return;
            
            if (_isMatchmaking)
            {
                queueStatusText.text = "Cancelling...";
                _isCancelling = true;
                // Cancel matchmaking
                _isCancelling = false;
                _isMatchmaking = false;
                findMatchButtonText.text = "Find Match";
                queueStatusText.text = string.Empty;
                return;
            }
            
            // Start queue
            findMatchButtonText.text = "Cancel";
            queueStatusText.text = "Searching...";
            _isMatchmaking = true;
        }

        public async void StartHost()
        {
            await HostSingleton.Instance.GameManager.StartHostAsync();
        }

        public async void StartClient()
        {
            await ClientSingleton.Instance.GameManager.StartClientAsync(joinCodeField.text);
        }
    }
}
