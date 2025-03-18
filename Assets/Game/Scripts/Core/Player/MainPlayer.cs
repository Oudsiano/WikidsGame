using AINavigation;
using Combat;
using Core.Player.MovingBetweenPoints;
using Core.Quests;
using Data;
using Saving;
using SceneManagement;
using UI;
using UnityEngine;
using UnityEngine.AI;

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
        private NavMeshAgent _agent;
        private bool _ifModularCharacterCreated;
        
        public PlayerController PlayerController => _playerController;
        public NavMeshAgent Agent => _agent;
        
        public bool IfModularCharacterCreated => _ifModularCharacterCreated;
        
        public void Construct(IGame igame, DataPlayer dataPlayer, UIManager uiManager, SaveGame saveGame,
            FastTestsManager fastTestsManager, QuestManager questManager, CoinManager coinManager,
            BottleManager bottleManager, WeaponArmorManager weaponArmorManager, Timer timer)
        {
            _agent = GetComponent<NavMeshAgent>();
            
            _saveGame = saveGame;
            _dataPlayer = dataPlayer;
            _uiManager = uiManager;
            
            _playerController.Construct(igame, _weaponPanelUI, _saveGame,
                _dataPlayer, this, fastTestsManager, questManager, coinManager, bottleManager, _uiManager, weaponArmorManager, timer);
            
            _weaponPanelUI.Construct(_playerController, _dataPlayer);
            _icon.Construct(_uiManager);
        }

        public void SetArmorManager(PlayerArmorManager playerArmorManager)
        {
            _playerArmorManager = playerArmorManager;
            _ifModularCharacterCreated = true;
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