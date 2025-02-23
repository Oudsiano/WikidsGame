using AINavigation;
using Combat;
using Core;
using Core.Camera;
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

        private FollowCamera _followCamera;
        private SceneLoader _sceneLoader;
        private SavePointsManager _savePointsManager;
        private ArrowForPlayerManager _arrowForPlayerManager;
        private FastTestsManager _fastTestsManager;
        private SaveGame _saveGame;

        [Inject]
        public void Compose(DiContainer diContainer) // TODO change 
        {
            _sceneContainer = _sceneContext.Container;

            _iGame = _sceneContainer.Resolve<IGame>();
            _followCamera = _sceneContainer.Resolve<FollowCamera>(); //
            _sceneLoader = _sceneContainer.Resolve<SceneLoader>();
            _gameAPI = _sceneContainer.Resolve<GameAPI>();

            _savePointsManager = _sceneContainer.Resolve<SavePointsManager>(); // TODO move
            _arrowForPlayerManager = _sceneContainer.Resolve<ArrowForPlayerManager>();
            _fastTestsManager = _sceneContainer.Resolve<FastTestsManager>();
            _saveGame = _sceneContainer.Resolve<SaveGame>();
            _player = _sceneContainer.Resolve<MainPlayer>(); //
            _dataPlayer = _sceneContainer.Resolve<DataPlayer>(); //
            _levelChangeObserver = _sceneContainer.Resolve<LevelChangeObserver>(); //
            _bottleManager = _sceneContainer.Resolve<BottleManager>(); // 
            _questManager = _sceneContainer.Resolve<QuestManager>(); // 
            _npcManager = _sceneContainer.Resolve<NPCManagment>(); // 
            _cursorManager = _sceneContainer.Resolve<CursorManager>(); // 
            _uiManager = _sceneContainer.Resolve<UIManager>(); //
            _coinManager = _sceneContainer.Resolve<CoinManager>(); //
            _weaponArmorManager = _sceneContainer.Resolve<WeaponArmorManager>(); //

            ConstructComponents();
        }

        private void ConstructComponents() // TODO check order
        {
            _iGame.Construct(_player, _gameAPI, _dataPlayer, _saveGame, _player.PlayerController,
                _levelChangeObserver, _savePointsManager, _arrowForPlayerManager,
                _questManager, _npcManager, _fastTestsManager,
                _cursorManager, _uiManager, _coinManager, _bottleManager,
                _weaponArmorManager);

            _savePointsManager.Construct(_dataPlayer);
            _sceneLoader.Construct(_dataPlayer, _levelChangeObserver, _savePointsManager);
            _gameAPI.Construct(_player, _sceneLoader, _dataPlayer, _saveGame, _fastTestsManager,
                _player.PlayerController,
                _weaponArmorManager, _questManager);

            _saveGame.Construct(_gameAPI, _weaponArmorManager, _coinManager,
                _dataPlayer, _uiManager);
            _uiManager.Construct(_sceneLoader, _followCamera, _gameAPI, _coinManager, _saveGame, _questManager,
                _dataPlayer, _fastTestsManager);
            _player.Construct(_iGame, _dataPlayer, _uiManager, _saveGame);
            _followCamera.Construct(_player);

            //_playerController.Construct(); -> iGame.Construct
            //_levelChangeObserver.Construct(); -> iGame.Construct
            //_arrowForPlayerManager.Construct(); -> iGame.Construct
            //_questManager.Construct(); -> iGame.Construct
            //_npcManagment.Construct(); -> iGame.Construct
            //_fastTestsManager.Construct(); -> iGame.Construct
            //_cursorManager no need construct right now
        }
    }
}