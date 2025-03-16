using System.Collections.Generic;
using Core;
using Core.Camera;
using Core.Player;
using Core.Quests;
using Data;
using Infrastructure.Installers.EntryPoint;
using Loading;
using Loading.LoadingOperations;
using Loading.LoadingOperations.Preloading;
using Saving;
using SceneManagement;
using UI;
using UnityEngine;
using Web;
using Zenject;

namespace Infrastructure.Installers
{
    public class GlobalInstaller : MonoInstaller
    {
        [SerializeField] private GameAPI _gameAPIPrefab;
        [SerializeField] private IGame _iGamePrefab;

        [SerializeField] private FollowCamera _followCameraPrefab;
        [SerializeField] private AudioManager _audioManagerPrefab;
        [SerializeField] private MainPlayer _playerPrefab;
        [SerializeField] private DataPlayer _dataPlayerPrefab;
        [SerializeField] private LevelChangeObserver _levelChangeObserverPrefab;
        [SerializeField] private BottleManager _bottleManagerPrefab;
        [SerializeField] private QuestManager _questManagerPrefab;
        [SerializeField] private CursorManager _cursorManagerPrefab;
        [SerializeField] private NPCManagment _npcManagerPrefab;
        [SerializeField] private UIManager _uiManagerPrefab;
        [SerializeField] private UIManager _uiManagerMobilePrefab;
        [SerializeField] private CoinManager _coinManagerPrefab;
        [SerializeField] private WeaponArmorManager _weaponArmorManagerPrefab;
        [SerializeField] private AllQuestsInGame _allQuests;
        [SerializeField] private SceneWithTestsID _sceneWithTestsID;
        [SerializeField] private KeyBoardsEvents _keyBoardsEvents;
        [SerializeField] private JavaScriptHook _javaScriptHook;

        public override void InstallBindings()
        {
            Container.Bind<LoadingScreenProvider>().AsSingle().NonLazy();
            Container.Bind<ScenePreloader>().AsSingle().NonLazy();
            Container.Bind<AssetProvider>().AsSingle().NonLazy();

            Container.Bind<MainPlayer>().FromComponentInNewPrefab(_playerPrefab).AsSingle().NonLazy();
            Container.Bind<JavaScriptHook>().FromComponentInNewPrefab(_javaScriptHook).AsSingle().NonLazy();
            Container.Bind<IGame>().FromComponentInNewPrefab(_iGamePrefab).AsSingle().NonLazy();
            Container.Bind<GameAPI>().FromComponentInNewPrefab(_gameAPIPrefab).AsSingle().NonLazy();

            Container.Bind<SavePointsManager>().AsSingle().NonLazy();
            Container.Bind<ArrowForPlayerManager>().AsSingle().NonLazy();
            Container.Bind<FastTestsManager>().AsSingle().NonLazy();
            Container.Bind<SaveGame>().AsSingle().NonLazy();
            Container.Bind<LocalAssetLoader>().AsSingle();

            BindingComponents();
        }

        private void BindingComponents()
        {
            Container.Bind<KeyBoardsEvents>().FromComponentInNewPrefab(_keyBoardsEvents).AsSingle().NonLazy();
            Container.Bind<AllQuestsInGame>().FromComponentInNewPrefab(_allQuests).AsSingle().NonLazy();
            Container.Bind<SceneWithTestsID>().FromComponentInNewPrefab(_sceneWithTestsID).AsSingle().NonLazy();
            Container.Bind<AudioManager>().FromComponentInNewPrefab(_audioManagerPrefab).AsSingle().NonLazy();
            Container.Bind<FollowCamera>().FromComponentInNewPrefab(_followCameraPrefab).AsSingle().NonLazy();
            Container.Bind<DataPlayer>().FromComponentInNewPrefab(_dataPlayerPrefab).AsSingle().NonLazy();
            Container.Bind<LevelChangeObserver>().FromComponentInNewPrefab(_levelChangeObserverPrefab).AsSingle()
                .NonLazy();
            Container.Bind<BottleManager>().FromComponentInNewPrefab(_bottleManagerPrefab).AsSingle().NonLazy();
            Container.Bind<QuestManager>().FromComponentInNewPrefab(_questManagerPrefab).AsSingle().NonLazy();
            Container.Bind<NPCManagment>().FromComponentInNewPrefab(_npcManagerPrefab).AsSingle().NonLazy();
            Container.Bind<CursorManager>().FromComponentInNewPrefab(_cursorManagerPrefab).AsSingle().NonLazy();
            BindingUI();
            // Container.Bind<UIManager>().FromComponentInNewPrefab(_uiManagerPrefab).AsSingle().NonLazy();
            // Container.Bind<UIManager>().FromComponentInNewPrefab(_uiManagerMobilePrefab).AsSingle().NonLazy();
            Container.Bind<CoinManager>().FromComponentInNewPrefab(_coinManagerPrefab).AsSingle().NonLazy();
            Container.Bind<WeaponArmorManager>().FromComponentInNewPrefab(_weaponArmorManagerPrefab).AsSingle()
                .NonLazy();
        }

        private void BindingUI()
        {
            Container.Bind<UIManager>().FromComponentInNewPrefab(_uiManagerPrefab).AsSingle().NonLazy();

            // if (DeviceChecker.IsMobileDevice())
            // {
            //     Container.Bind<UIManager>().FromComponentInNewPrefab(_uiManagerPrefab).AsSingle().NonLazy(); // TODO mobileManager
            // }
            // else
            // {
            //     Container.Bind<UIManager>().FromComponentInNewPrefab(_uiManagerPrefab).AsSingle().NonLazy();
            // }

            Debug.Log("IsMobile " + DeviceChecker.IsMobileDevice());
        }
    }
}