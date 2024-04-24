using System;
using UnityEngine;

namespace Core.Coins
{
    public class BountyCoin : Coin
    {
        public override int Collect()
        {
            if (!IsServer)
            {
                Show(false);
                return 0;
            }

            if (AlreadyCollected) return 0;
            AlreadyCollected = true;
            
            Destroy(gameObject);
            return CoinValue;
        }
    }
}
