using Core.PickableItems;
using Saving;
using UnityEngine;

namespace SceneManagement
{
    public class CoinManager : MonoBehaviour
    {
        [SerializeField] private GameObject _Coin; // TODO GO

        private Currency coins;
        private CursorManager _cursorManager;
        private SaveGame _saveGame;

        public void Construct(SaveGame saveGame, CursorManager cursorManager)
        {
            _saveGame = saveGame;
            _cursorManager = cursorManager;
            
            Coins = new Currency(_saveGame);
        }

        public Currency Coins
        {
            get => coins;
            set => coins = value;
        }

        public void MakeGoldOnSceneWithCount(float count, Vector3 pos)
        {
            double countCoins = count;
            Instantiate(_Coin, pos, Quaternion.Euler(0, 0, 0))
                .GetComponent<PickableCoin>()
                .Construct(count, _cursorManager, _saveGame);
            AudioManager.Instance.PlaySound("CoinPickup"); // TODO Instance AudioManager
        }
    }
}