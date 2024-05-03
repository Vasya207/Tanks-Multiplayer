using Input;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Core.Player
{
    public class PlayerAiming : NetworkBehaviour
    {
        [SerializeField] private InputReader inputReader;
        [SerializeField] private Transform turretTransform;

        private AimStick _aimStick;
        private Vector2 _aimScreenPosition;
        private bool _aimStickFound;
        
        public override void OnNetworkSpawn()
        {
            if (!IsOwner) return;
            
            _aimStick = FindObjectOfType<AimStick>();
            if(_aimStick == null) 
                SceneManager.sceneLoaded += OnSceneLoaded;
            else
                _aimStickFound = true;
        }

        public override void OnNetworkDespawn()
        {
            if (!IsOwner) return;
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            _aimStick = FindObjectOfType<AimStick>();
            _aimStickFound = true;
        }

        private void LateUpdate()
        {
            if (!IsOwner) return;
#if UNITY_EDITOR || UNITY_EDITOR_WIN
            
            _aimScreenPosition = inputReader.AimPosition;
            
            Vector2 aimWorldPosition = Camera.main.ScreenToWorldPoint(_aimScreenPosition);
            
            turretTransform.up = new Vector2(
                aimWorldPosition.x - turretTransform.position.x,
                aimWorldPosition.y - turretTransform.position.y);
#else
            if (_aimStickFound)
            {
                if (_aimStick.IsTouchingJoystick)
                {
                    if (inputReader.AimPosition != new Vector2(0, 0))
                    {
                        _aimScreenPosition = inputReader.AimPosition;
                    }
                }
            }

            turretTransform.up = new Vector2(
                _aimScreenPosition.x,
                _aimScreenPosition.y);
#endif
        }
    }
}
