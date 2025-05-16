using Data;
using Loading;
using Saving;
using UnityEngine;
using UnityEngine.Events;
using Web;
using Zenject;

namespace Infrastructure.Installers
{
    public class BootstrapInstaller : MonoInstaller
    {
        [SerializeField] private JavaScriptHook _javaScriptHook;
        [SerializeField] private GameAPI _gameAPI;
        [SerializeField] private DataPlayer _dataPlayer;
        [SerializeField] private ScreenOrientationChecker _screenOrientationChecker;
        [SerializeField] private SocketManager _socketManager;
        [SerializeField] private MultiplayerController _multiplayerController;
        
        public override void InstallBindings()
        {
            ProjectContext.Instance.Container.Bind<ScreenOrientationChecker>().FromComponentInNewPrefab(_screenOrientationChecker).AsSingle().NonLazy();
            ProjectContext.Instance.Container.Bind<SocketManager>().FromComponentInNewPrefab(_socketManager).AsSingle().NonLazy();
            ProjectContext.Instance.Container.Bind<MultiplayerController>().FromComponentInNewPrefab(_multiplayerController).AsSingle().NonLazy();
            
            // ProjectContext.Instance.Container.Bind<GameAPI>().FromComponentInNewPrefab(_gameAPI).AsSingle().NonLazy();
            // Container.Bind<JavaScriptHook>().FromInstance(_javaScriptHook).AsSingle().NonLazy();
            //
            // ProjectContext.Instance.Container.Bind<DataPlayer>().FromComponentInNewPrefab(_dataPlayer).AsSingle().NonLazy();
        }
    }
}