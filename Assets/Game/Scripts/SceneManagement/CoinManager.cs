using Core.PickableItems;
using UnityEngine;

namespace SceneManagement
{
    public class CoinManager : MonoBehaviour
    {
        [SerializeField] private GameObject _Coin; // TODO GO

        private Currency coins;

        public void Construct()
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
            AudioManager.instance.PlaySound("CoinPickup"); // TODO Instance AudioManager

        }

    }
}