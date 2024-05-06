using UnityEngine;

namespace Input
{
    public class JoystickEnabler : MonoBehaviour
    {
        void Start()
        {
            gameObject.SetActive(Application.platform == RuntimePlatform.Android);
        }
    }
}
