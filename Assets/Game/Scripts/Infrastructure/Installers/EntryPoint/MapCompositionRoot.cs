using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Data;
using Loading;
using Loading.LoadingOperations;
using Saving;
using SceneManagement;
using UnityEngine;
using Zenject;

namespace Infrastructure.Installers.EntryPoint
{
    public class MapCompositionRoot : MonoBehaviour, ICompose
    {
        [SerializeField] private SceneContext _sceneContext;

        [SerializeField] private LocationChange _locationChange;

        [SerializeField] private LocationChange _locationChangeMobie;

        private DiContainer _sceneContainer;
        private AssetProvider _assetProvider;
        private LoadingScreenProvider _loadingProvider;
        private SceneLoaderService _sceneLoader;
        
        [Inject]
        public void Compose(DiContainer diContainer)
        {
            _sceneContainer = _sceneContext.Container;

            ConstructComponents();
        }

        private async void ConstructComponents()
        {
            _assetProvider = _sceneContainer.Resolve<AssetProvider>();
            _loadingProvider = _sceneContainer.Resolve<LoadingScreenProvider>();
            _sceneLoader = _sceneContainer.Resolve<SceneLoaderService>();

            _locationChange.Construct(_sceneContainer.Resolve<DataPlayer>(),
                _sceneContainer.Resolve<LevelChangeObserver>(),
                _sceneContainer.Resolve<SceneLoaderService>(),
                _sceneContainer.Resolve<GameAPI>());
            
            var loadingOperations = new Queue<ILoadingOperation>();
            loadingOperations.Enqueue(_assetProvider);

            await _loadingProvider.LoadAndDestroy(loadingOperations);

            await _sceneContainer.Resolve<AssetPreloader>().LoadRemainingScenes();
        }
    }
}