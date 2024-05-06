using System;
using Input;
using Unity.Netcode;
using UnityEngine;

namespace Core.Player
{
    public class PlayerMovement : NetworkBehaviour
    {
        [Header("References")]
        [SerializeField] private InputReader inputReader;
        [SerializeField] private Transform bodyTransform;
        [SerializeField] private Rigidbody2D rigidbody2D;

        [Header("Settings")] 
        [SerializeField] private float movementSpeed = 4f;
        [SerializeField] private float turningRate = 30f;

        private Vector2 _previousMovementInput;
        
        public override void OnNetworkSpawn()
        {
            if (!IsOwner) return;

            inputReader.MoveEvent += HandleMove;
        }

        public override void OnNetworkDespawn()
        {
            if (!IsOwner) return;
            
            inputReader.MoveEvent -= HandleMove;
        }

        private void Update()
        {
            if (!IsOwner) return;

            if (Application.platform == RuntimePlatform.Android)
            {
                float angle = Mathf.Atan2(_previousMovementInput.x, _previousMovementInput.y) * Mathf.Rad2Deg;

                Quaternion targetRotation = Quaternion.Euler(0f, 0f, -angle);
                bodyTransform.rotation = Quaternion.Slerp(bodyTransform.rotation, targetRotation, turningRate * Time.deltaTime);
            }
            else
            {
                float zRotation = _previousMovementInput.x * -turningRate * Time.deltaTime;
                bodyTransform.Rotate(0f,0f, zRotation);
            }
        }

        private void FixedUpdate()
        {
            if (!IsOwner) return;

            if (Application.platform == RuntimePlatform.Android)
            {
                Vector2 movementDirection = _previousMovementInput.normalized;

                rigidbody2D.velocity = movementDirection * movementSpeed;
            }
            else
            {
                rigidbody2D.velocity = (Vector2)bodyTransform.up * (_previousMovementInput.y * movementSpeed);
            }
        }

        private void HandleMove(Vector2 currentMovementInput)
        {
            _previousMovementInput = currentMovementInput;
        }
    }
}
