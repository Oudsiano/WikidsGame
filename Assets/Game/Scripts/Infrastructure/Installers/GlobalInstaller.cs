using AINavigation;
using Core;
using Core.Player;
using Core.Quests;
using Data;
using Saving;
using SceneManagement;
using UI;
using UnityEngine;
using Zenject;

namespace Infrastructure.Installers
{
    public class GlobalInstaller : MonoInstaller
    {
        [SerializeField] private SceneLoader _sceneLoaderPrefab;
        [SerializeField] private GameAPI _gameAPIPrefab;
        [SerializeField] private IGame _iGamePrefab;

        [SerializeField] private MainPlayer _playerPrefab;
        [SerializeField] private PlayerController _playerControllerPrefab;
        [SerializeField] private DataPlayer _dataPlayerPrefab;
        [SerializeField] private LevelChangeObserver _levelChangeObserverPrefab;
        [SerializeField] private BottleManager _bottleManagerPrefab;
        [SerializeField] private QuestManager _questManagerPrefab;
        [SerializeField] private CursorManager _cursorManagerPrefab;
        [SerializeField] private NPCManagment _npcManagerPrefab;
        [SerializeField] private UIManager _uiManagerPrefab;
        [SerializeField] private CoinManager _coinManagerPrefab;
        [SerializeField] private WeaponArmorManager _weaponArmorManagerPrefab;
        
        public override void InstallBindings()
        {
            Container.Bind<MainPlayer>().FromComponentInNewPrefab(_playerPrefab).AsSingle().NonLazy();
            Container.Bind<IGame>().FromComponentInNewPrefab(_iGamePrefab).AsSingle().NonLazy();
            Container.Bind<GameAPI>().FromComponentInNewPrefab(_gameAPIPrefab).AsSingle().NonLazy();
            Container.Bind<SceneLoader>().FromComponentInNewPrefab(_sceneLoaderPrefab).AsSingle().NonLazy();

            Container.Bind<SavePointsManager>().AsSingle().NonLazy();
            Container.Bind<ArrowForPlayerManager>().AsSingle().NonLazy();
            Container.Bind<FastTestsManager>().AsSingle().NonLazy();
            Container.Bind<SaveGame>().AsSingle().NonLazy();
            
            Container.Bind<DataPlayer>().FromComponentInNewPrefab(_dataPlayerPrefab).AsSingle().NonLazy();
            Container.Bind<PlayerController>().FromComponentInNewPrefab(_playerControllerPrefab).AsSingle().NonLazy();
            Container.Bind<LevelChangeObserver>().FromComponentInNewPrefab(_levelChangeObserverPrefab).AsSingle().NonLazy();
            Container.Bind<BottleManager>().FromComponentInNewPrefab(_bottleManagerPrefab).AsSingle().NonLazy();
            Container.Bind<QuestManager>().FromComponentInNewPrefab(_questManagerPrefab).AsSingle().NonLazy();
            Container.Bind<NPCManagment>().FromComponentInNewPrefab(_npcManagerPrefab).AsSingle().NonLazy();
            Container.Bind<CursorManager>().FromComponentInNewPrefab(_cursorManagerPrefab).AsSingle().NonLazy();
            Container.Bind<UIManager>().FromComponentInNewPrefab(_uiManagerPrefab).AsSingle().NonLazy();
            Container.Bind<CoinManager>().FromComponentInNewPrefab(_coinManagerPrefab).AsSingle().NonLazy();
            Container.Bind<WeaponArmorManager>().FromComponentInNewPrefab(_weaponArmorManagerPrefab).AsSingle().NonLazy();
        }
    }
}