using Core.PickableItems;
using UnityEngine;

namespace SceneManagement
{
    public class CoinManager : MonoBehaviour
    {
        [SerializeField] private GameObject _Coin;

        private Currency coins;

        public void Init()
        {
            Coins = new Currency();
        }

        public Currency Coins { get => coins; set => coins = value; }

        public void MakeGoldOnSceneWithCount(float count, Vector3 pos)
        {
            double countCoins = count;
            Instantiate(_Coin, pos, Quaternion.Euler(0, 0, 0))
                .GetComponent<PickableCoin>()
                .Init(count);
            AudioManager.instance.PlaySound("CoinPickup");

        }

    }
}