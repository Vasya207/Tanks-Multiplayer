using System;
using UnityEngine;

namespace Utils
{
    public class Lifetime : MonoBehaviour
    {
        [SerializeField] private float lifetime = 1f;

        private void Start()
        {
            Destroy(gameObject, lifetime);
        }
    }
}
