using System;
using UnityEngine;

namespace Core.Coins
{
    public class RespawningCoin : Coin
    {
        public event Action<RespawningCoin> OnCollected;

        private Vector3 previousPosition;

        private void Update()
        {
            if (previousPosition != transform.position)
            {
                Show(true);
            }

            previousPosition = transform.position;
        }

        public override int Collect()
        {
            if (!IsServer)
            {
                Show(false);
                return 0;
            }

            if (AlreadyCollected) return 0;

            AlreadyCollected = true;
            OnCollected?.Invoke(this);
            return CoinValue;
        }

        public void Reset()
        {
            AlreadyCollected = false;
        }
    }
}
