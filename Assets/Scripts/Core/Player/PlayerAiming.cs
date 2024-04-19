using Input;
using Unity.Netcode;
using UnityEngine;

namespace Core.Player
{
    public class PlayerAiming : NetworkBehaviour
    {
        [SerializeField] private InputReader inputReader;
        [SerializeField] private Transform turretTransform;

        //private Camera _mainCamera;
        
        public override void OnNetworkSpawn()
        {
            if (!IsOwner) return;

            //_mainCamera = Camera.main;
        }

        private void LateUpdate()
        {
            if (!IsOwner) return;

            Vector2 aimScreenPosition = inputReader.AimPosition;
            Vector2 aimWorldPosition = Camera.main.ScreenToWorldPoint(aimScreenPosition);

            turretTransform.up = new Vector2(
                aimWorldPosition.x - turretTransform.position.x,
                aimWorldPosition.y - turretTransform.position.y);
            //turretTransform.up = (Vector2)_mainCamera.ScreenToWorldPoint(inputReader.AimPosition);
        }
    }
}
