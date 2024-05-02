using Input;
using Unity.Netcode;
using UnityEngine;

namespace Core.Player
{
    public class PlayerAiming : NetworkBehaviour
    {
        [SerializeField] private InputReader inputReader;
        [SerializeField] private Transform turretTransform;
        
        public override void OnNetworkSpawn()
        {
            if (!IsOwner) return;
        }

        private void LateUpdate()
        {
            if (!IsOwner) return;

            Vector2 aimScreenPosition = inputReader.AimPosition;
            
#if UNITY_EDITOR || UNITY_EDITOR_WIN
            Vector2 aimWorldPosition = Camera.main.ScreenToWorldPoint(aimScreenPosition);

            turretTransform.up = new Vector2(
                aimWorldPosition.x - turretTransform.position.x,
                aimWorldPosition.y - turretTransform.position.y);
#else
            turretTransform.up = new Vector2(
                aimScreenPosition.x,
                aimScreenPosition.y);
#endif
        }
    }
}
