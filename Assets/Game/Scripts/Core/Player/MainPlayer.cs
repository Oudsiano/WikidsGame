using AINavigation;
using Combat;
using Core.Quests;
using Data;
using Saving;
using SceneManagement;
using UI;
using UnityEngine;

namespace Core.Player
{
    public class MainPlayer : MonoBehaviour
    {
        [SerializeField] private PlayerController _playerController;
        [SerializeField] private IconForFarCamera _icon;
        [SerializeField] private WeaponPanelUI _weaponPanelUI;
        
        private PlayerArmorManager _playerArmorManager;
        private DataPlayer _dataPlayer;
        private UIManager _uiManager;
        private SaveGame _saveGame;

        public PlayerController PlayerController => _playerController;

        public void Construct(IGame igame, DataPlayer dataPlayer, UIManager uiManager, SaveGame saveGame,
            FastTestsManager fastTestsManager, QuestManager questManager, CoinManager coinManager,
            BottleManager bottleManager, WeaponArmorManager weaponArmorManager)
        {
            Debug.Log("----------------------------------");
            Debug.Log("Player.Construct start");

            _saveGame = saveGame;
            Debug.Log("saveGame assigned");
            _dataPlayer = dataPlayer;
            Debug.Log("dataPlayer assigned");
            _uiManager = uiManager;
            Debug.Log("uiManager assigned");
            
            _playerController.Construct(igame, _weaponPanelUI, _saveGame,
                _dataPlayer, this, fastTestsManager, questManager, coinManager, bottleManager, _uiManager, weaponArmorManager);
            Debug.Log("PlayerController constructed");
            if (_playerController == null)
            {
                Debug.Log("PlayerController is null");
            }
            else
            {
                Debug.Log("PlayerController is not null");
            }

            if (_dataPlayer == null)
            {
                Debug.Log("dataPlayer is null");
            }
            else
            {
                Debug.Log("dataPlayer is not null");
            }
            _weaponPanelUI.Construct(_playerController, _dataPlayer);
            Debug.Log("_weaponPanelUI constructed");
            _icon.Construct(_uiManager);
            Debug.Log("icon constructed");
            Debug.Log("Player.Construct finish");
            Debug.Log("----------------------------------");
        }

        public void SetArmorManager(PlayerArmorManager playerArmorManager)
        {
            _playerArmorManager = playerArmorManager;
        }

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