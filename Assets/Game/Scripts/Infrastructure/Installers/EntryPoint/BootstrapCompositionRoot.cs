using AINavigation;
using Combat;
using Core;
using Core.Player;
using Core.Quests;
using Data;
using Saving;
using SceneManagement;
using UI;
using UnityEngine;
using Zenject;

namespace Infrastructure.Installers.EntryPoint
{
    public class BootstrapCompositionRoot : MonoBehaviour, ICompose
    {
        [SerializeField] private SceneContext _sceneContext;
        private DiContainer _sceneContainer;

        //UI
        private WeaponPanelUI _weaponPanelUI;

        private MainPlayer _player;
        private IGame _iGame;

        private DataPlayer _dataPlayer;
        private GameAPI _gameAPI;
        private PlayerController _playerController;
        private BottleManager _bottleManager;
        private QuestManager _questManager;
        private NPCManagment _npcManager;
        private CursorManager _cursorManager;
        private LevelChangeObserver _levelChangeObserver;
        private UIManager _uiManager;
        private CoinManager _coinManager;
        private WeaponArmorManager _weaponArmorManager;
        private PlayerArmorManager _playerArmorManager;
        
        private SceneLoader _sceneLoader;
        private SavePointsManager _savePointsManager;
        private ArrowForPlayerManager _arrowForPlayerManager;
        private FastTestsManager _fastTestsManager;
        private SaveGame _saveGame;

        [Inject]
        public void Compose(DiContainer diContainer) // TODO change 
        {
            _sceneContainer = _sceneContext.Container;

            _savePointsManager = diContainer.Resolve<SavePointsManager>(); // TODO move
            _arrowForPlayerManager = diContainer.Resolve<ArrowForPlayerManager>();
            _fastTestsManager = diContainer.Resolve<FastTestsManager>();
            _saveGame = diContainer.Resolve<SaveGame>();

            _player = diContainer.Resolve<MainPlayer>(); //
            _dataPlayer = diContainer.Resolve<DataPlayer>(); //
            _playerController = diContainer.Resolve<PlayerController>(); //
            _levelChangeObserver = diContainer.Resolve<LevelChangeObserver>();
            _bottleManager = diContainer.Resolve<BottleManager>(); // 
            _questManager = diContainer.Resolve<QuestManager>(); // 
            _npcManager = diContainer.Resolve<NPCManagment>(); // 
            _cursorManager = diContainer.Resolve<CursorManager>(); // 
            _uiManager = diContainer.Resolve<UIManager>(); //
            _coinManager = diContainer.Resolve<CoinManager>(); //
            _weaponArmorManager = diContainer.Resolve<WeaponArmorManager>(); //
            _playerArmorManager = diContainer.Resolve<PlayerArmorManager>(); //
            _weaponPanelUI = diContainer.Resolve<WeaponPanelUI>();// Остановился здесь на прокидывании зависимостей в playerController -> WeaponPanelUI
            
            _iGame = diContainer.Resolve<IGame>();
            _sceneLoader = diContainer.Resolve<SceneLoader>();
            _gameAPI = diContainer.Resolve<GameAPI>();

            ConstructComponents();

            _iGame.Construct(_gameAPI, _dataPlayer, _saveGame, _playerController,
                _levelChangeObserver, _savePointsManager, _arrowForPlayerManager,
                _questManager, _npcManager, _fastTestsManager,
                _cursorManager, _uiManager, _coinManager, _bottleManager,
                _weaponArmorManager, _playerArmorManager , _weaponPanelUI);
        }

        private void ConstructComponents()
        {
            _player.Construct();
            _sceneLoader.Construct();
            _gameAPI.Construct(_player, _sceneLoader, _dataPlayer, _saveGame, _fastTestsManager, _playerController,
                _weaponArmorManager, _questManager);

            _saveGame.Construct(_gameAPI, _weaponArmorManager, _coinManager,
                _dataPlayer, _uiManager);
        }
    }
}