using Networking.Host;
using UnityEngine;

namespace UI
{
    public class MainMenu : MonoBehaviour
    {
        public async void StartHost()
        {
            await HostSingleton.Instance.GameManager.StartHostAsync();
        }
    }
}
