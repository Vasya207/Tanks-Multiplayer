using System;
using Core.Player;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.OnScreen;

namespace Input
{
    public class AimStick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        private ProjectileLauncher _projectileLauncher;

        private void Start()
        {
            _projectileLauncher = FindObjectOfType<ProjectileLauncher>();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            _projectileLauncher.FireOneTime();
        }
    }
}
