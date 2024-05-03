using System;
using Core.Player;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.OnScreen;

namespace Input
{
    public class AimStick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        public bool IsTouchingJoystick;
        public event Action OnJoyStickReleased;

        public void OnPointerDown(PointerEventData eventData)
        {
            IsTouchingJoystick = true;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            IsTouchingJoystick = false;
            OnJoyStickReleased?.Invoke();
        }
    }
}
