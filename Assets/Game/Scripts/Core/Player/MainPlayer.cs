using AINavigation;
using Data;
using UI;
using UnityEngine;

namespace Core.Player
{
    public class MainPlayer : MonoBehaviour
    {
        private static MainPlayer _instance; // TODO singleton
        private DataPlayer _dataPlayer;
        private UIManager _uiManager;
        private PlayerController _playerController;

        public static MainPlayer Instance // TODO singleton
        {
            get { return _instance; } // Возвращает текущий экземпляр
        }

        public void Construct(DataPlayer dataPlayer, UIManager uiManager, PlayerController playerController)
        {
            _dataPlayer = dataPlayer;
            _uiManager = uiManager;
            _playerController = playerController;
            
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
        
        // private void Awake() // TODO construct project context
        // {
        //     if (_instance != null && _instance != this)
        //     {
        //         Destroy(gameObject);
        //     }
        //     else
        //     {
        //         _instance = this;
        //         DontDestroyOnLoad(gameObject);
        //     }
        // }

        public void ChangeCountEnergy(int value)
        {
            _dataPlayer.PlayerData.chargeEnergy += value;
            _uiManager.setEnergyCharger(_dataPlayer.PlayerData.chargeEnergy.ToString());
            _playerController.WeaponPanelUI.ResetFireballCount();
        }

        public void ResetCountEnergy()
        {
            _dataPlayer.PlayerData.chargeEnergy = 0;
            _uiManager.setEnergyCharger(_dataPlayer.PlayerData.chargeEnergy.ToString());
            _playerController.WeaponPanelUI.ResetFireballCount();
        }
    }
}