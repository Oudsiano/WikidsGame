using Data;
using Saving;
using UnityEngine;
using Web;
using Zenject;

namespace Infrastructure.Installers
{
    public class BootstrapInstaller : MonoInstaller
    {
        [SerializeField] private JavaScriptHook _javaScriptHook;
        [SerializeField] private GameAPI _gameAPI;
        [SerializeField] private DataPlayer _dataPlayer;
        
        public override void InstallBindings()
        {
            ProjectContext.Instance.Container.Bind<GameAPI>().FromComponentInNewPrefab(_gameAPI).AsSingle().NonLazy();
            Container.Bind<JavaScriptHook>().FromComponentInNewPrefab(_javaScriptHook).AsSingle().NonLazy();
            
            ProjectContext.Instance.Container.Bind<DataPlayer>().FromComponentInNewPrefab(_dataPlayer).AsSingle().NonLazy();
        }
    }
}