using System;
using Unity.Mathematics;
using UnityEngine;

namespace Utils
{
    public class SpawnOnDestroy : MonoBehaviour
    {
        [SerializeField] private GameObject prefab;

        private void OnDestroy()
        {
            Instantiate(prefab, transform.position, Quaternion.identity);
        }
    }
}
