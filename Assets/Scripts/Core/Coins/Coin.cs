using Unity.Netcode;
using UnityEngine;

namespace Core.Coins
{
    public abstract class Coin : NetworkBehaviour
    {
        [SerializeField] private SpriteRenderer spriteRenderer;

        protected int CoinValue = 10;
        protected bool AlreadyCollected;

        public abstract int Collect();

        public void SetValue(int value)
        {
            CoinValue = value;
        }

        protected void Show(bool show)
        {
            spriteRenderer.enabled = show;
        }
    }
}
