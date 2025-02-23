using AINavigation;
using Combat;
using Data;
using Saving;
using UI;
using UnityEngine;

namespace Core.Player
{
    public class MainPlayer : MonoBehaviour
    {
        private static MainPlayer _instance; // TODO singleton
        [SerializeField] private PlayerArmorManager _playerArmorManager;
        [SerializeField] private WeaponPanelUI _weaponPanelUI;

        private DataPlayer _dataPlayer;
        private UIManager _uiManager;
        [SerializeField] private PlayerController _playerController;
        private SaveGame _saveGame;

        public static MainPlayer Instance // TODO singleton
        {
            get { return _instance; } // Возвращает текущий экземпляр
        }
        
        public PlayerController PlayerController => _playerController;
        
        public void Construct(IGame igame, DataPlayer dataPlayer, UIManager uiManager, SaveGame saveGame)
        {
            Debug.Log("Construct player");
            
            _saveGame = saveGame;
            _dataPlayer = dataPlayer;
            _uiManager = uiManager;

            _playerController.Construct(igame, _playerArmorManager, _weaponPanelUI, _saveGame, 
                _dataPlayer);
            _weaponPanelUI.Construct(_playerController, _dataPlayer);
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