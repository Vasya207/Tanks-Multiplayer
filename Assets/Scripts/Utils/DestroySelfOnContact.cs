using System;
using UnityEngine;

namespace Utils
{
    public class DestroySelfOnContact : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D other)
        {
            Destroy(gameObject);
        }
    }
}
