using UnityEngine;

namespace Core.Player
{
    public class MainPlayer : MonoBehaviour
    {
        private static MainPlayer _instance; // TODO singleton

        public static MainPlayer Instance // TODO singleton
        {
            get { return _instance; } // Возвращает текущий экземпляр
        }

        private void Awake() // TODO construct project context
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                _instance = this;
                DontDestroyOnLoad(gameObject);
            }
        }

        public void ChangeCountEnergy(int value)
        {
            IGame.Instance.dataPlayer.PlayerData.chargeEnergy += value;
            IGame.Instance.UIManager.setEnergyCharger(IGame.Instance.dataPlayer.PlayerData.chargeEnergy.ToString());
            IGame.Instance.playerController.WeaponPanelUI.ResetFireballCount();
        }

        public void ResetCountEnergy()
        {
            IGame.Instance.dataPlayer.PlayerData.chargeEnergy = 0;
            IGame.Instance.UIManager.setEnergyCharger(IGame.Instance.dataPlayer.PlayerData.chargeEnergy.ToString());
            IGame.Instance.playerController.WeaponPanelUI.ResetFireballCount();
        }
    }
}