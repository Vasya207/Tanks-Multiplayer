using System;
using Unity.Netcode;
using UnityEngine;

namespace Core.Coins
{
    public class CoinWallet : NetworkBehaviour
    {
        public NetworkVariable<int> TotalCoins = new NetworkVariable<int>();

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.TryGetComponent<Coin>(out Coin coin)) return;
            var coinValue = coin.Collect();

            if (!IsServer) return;
            TotalCoins.Value += coinValue;
        }
    }
}
