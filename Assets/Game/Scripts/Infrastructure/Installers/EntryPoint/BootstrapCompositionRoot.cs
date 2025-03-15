using System;
using System.Collections.Generic;
using AINavigation;
using Combat;
using Core;
using Core.Camera;
using Core.Player;
using Core.Quests;
using Cysharp.Threading.Tasks;
using Data;
using Loading;
using Loading.LoadingOperations;
using Loading.LoadingOperations.Preloading;
using Saving;
using SceneManagement;
using UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils;
using Web;
using Zenject;

namespace Infrastructure.Installers.EntryPoint
{
    public class BootstrapCompositionRoot : MonoBehaviour, ICompose
    {
        [SerializeField] private SceneContext _sceneContext;
        private DiContainer _sceneContainer;

        private JavaScriptHook _javaScriptHook;
        private MainPlayer _player;
        private IGame _iGame;

        private AudioManager _audioManager;
        private DataPlayer _dataPlayer;
        private GameAPI _gameAPI;
        private BottleManager _bottleManager;
        private QuestManager _questManager;
        private NPCManagment _npcManager;
        private CursorManager _cursorManager;
        private LevelChangeObserver _levelChangeObserver;
        private UIManager _uiManager;
        private CoinManager _coinManager;
        private WeaponArmorManager _weaponArmorManager;

        private FollowCamera _followCamera;
        private SceneLoaderService _sceneLoader;
        private SavePointsManager _savePointsManager;
        private ArrowForPlayerManager _arrowForPlayerManager;
        private FastTestsManager _fastTestsManager;
        private SaveGame _saveGame;
        private AllQuestsInGame _allQuests;
        private SceneWithTestsID _sceneWithTestsID;
        private KeyBoardsEvents _keyBoardsEvents;

        private LoadingScreenProvider _loadingProvider;
        private AssetProvider _assetProvider;

        [Inject]
        public void Compose(DiContainer diContainer)
        {
            _sceneContainer = _sceneContext.Container;
            _loadingProvider = _sceneContainer.Resolve<LoadingScreenProvider>();
            _assetProvider = _sceneContainer.Resolve<AssetProvider>();

            _keyBoardsEvents = _sceneContainer.Resolve<KeyBoardsEvents>();
            _javaScriptHook = _sceneContainer.Resolve<JavaScriptHook>();
            _iGame = _sceneContainer.Resolve<IGame>();
            _followCamera = _sceneContainer.Resolve<FollowCamera>(); //
            _sceneLoader = _sceneContainer.Resolve<SceneLoaderService>();
            _gameAPI = _sceneContainer.Resolve<GameAPI>();

            _audioManager = _sceneContainer.Resolve<AudioManager>();
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
            _allQuests = _sceneContainer.Resolve<AllQuestsInGame>();
            _sceneWithTestsID = _sceneContainer.Resolve<SceneWithTestsID>();

            ConstructComponents();
            SceneManager.LoadScene(Constants.Scenes.OpenScene);
        }

        private void ConstructComponents() // TODO check order
        {
            _iGame.Construct(_player, _gameAPI, _dataPlayer, _saveGame, _player.PlayerController,
                _levelChangeObserver, _savePointsManager, _arrowForPlayerManager,
                _questManager, _npcManager, _fastTestsManager,
                _cursorManager, _uiManager, _coinManager, _bottleManager,
                _weaponArmorManager, _allQuests, _sceneWithTestsID, _loadingProvider, _assetProvider,
                _sceneContainer.Resolve<ScenePreloadController>());


            _audioManager.Construct();
            _savePointsManager.Construct(_dataPlayer);
            _gameAPI.Construct(_player, _sceneLoader, _dataPlayer, _saveGame, _fastTestsManager,
                _player.PlayerController,
                _weaponArmorManager, _questManager);


            _saveGame.Construct(_gameAPI, _weaponArmorManager, _coinManager,
                _dataPlayer, _uiManager);
            _uiManager.Construct(_iGame, _sceneLoader, _followCamera, _gameAPI, _coinManager, _saveGame, _questManager,
                _dataPlayer, _fastTestsManager, _player.PlayerController, _weaponArmorManager, _levelChangeObserver);
            _player.Construct(_iGame, _dataPlayer, _uiManager, _saveGame, _fastTestsManager, _questManager,
                _coinManager, _bottleManager, _weaponArmorManager);
            _followCamera.Construct(_player, _player.PlayerController, _sceneLoader);
            ;
            _javaScriptHook.Construct(_dataPlayer, _sceneLoader);
            _keyBoardsEvents.Construct(_sceneLoader, _uiManager);
        }

        private async void Start()
        {
            await _sceneContainer.Resolve<ScenePreloadController>()
                .PreloadSceneInBackground(Constants.Scenes.FirstBattleScene);
        }
    }
}