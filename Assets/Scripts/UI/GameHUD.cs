using Networking.Client;
using Networking.Host;
using Unity.Netcode;
using UnityEngine;

namespace UI
{
    public class GameHUD : MonoBehaviour
    {
        public void ExitGame()
        {
            if (NetworkManager.Singleton.IsHost)
            {
                HostSingleton.Instance.GameManager.Shutdown();
            }

            ClientSingleton.Instance.GameManager.Disconnect();
        }
    }
}
