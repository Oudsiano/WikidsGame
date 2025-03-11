using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Data;
using Loading;
using Loading.LoadingOperations;
using Saving;
using SceneManagement;
using UnityEngine;
using Utils;
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
        private MultiScenePreloader _multiScenePreloader;
        private LoadingScreenProvider _loadingProvider;
        private SceneLoaderService _sceneLoader;


        [Inject]
        public void Compose(DiContainer diContainer)
        {
            _sceneContainer = _sceneContext.Container;

            ConstructComponents();
        }

        private void ConstructComponents()
        {
            _assetProvider = _sceneContainer.Resolve<AssetProvider>();
            _loadingProvider = _sceneContainer.Resolve<LoadingScreenProvider>();
            _sceneLoader = _sceneContainer.Resolve<SceneLoaderService>();
            
            _locationChange.Construct(_sceneContainer.Resolve<DataPlayer>(),
                _sceneContainer.Resolve<LevelChangeObserver>(),
                _sceneContainer.Resolve<SceneLoaderService>(),
                _sceneContainer.Resolve<GameAPI>());

            // _locationChangeMobie.Construct(_sceneContainer.Resolve<DataPlayer>(),
            //     _sceneContainer.Resolve<LevelChangeObserver>(), 
            //     _sceneContainer.Resolve<SceneLoaderService>(),
            //     _sceneContainer.Resolve<GameAPI>());
            
            var loadingOperations = new Queue<ILoadingOperation>();
            loadingOperations.Enqueue(_assetProvider);
            
            _multiScenePreloader = new MultiScenePreloader(_locationChange.GetOpenedScenesInReverseOrder());
            _multiScenePreloader.PreloadScenesInBackground();
            _multiScenePreloader.PreloadAllScenesInBackground();
            
            loadingOperations.Enqueue(_multiScenePreloader);
            _loadingProvider.LoadAndDestroy(loadingOperations).Forget();
        }

        // private void LoadingOperations(LocationChange locationChange)
        // {
        //     var loadingOperations = new Queue<ILoadingOperation>();
        //
        //     loadingOperations.Enqueue(_assetProvider);
        //     
        //     var openedScenes = locationChange.GetOpenedScenesInReverseOrder();
        //
        //     if (openedScenes.Count > 0)
        //     {
        //         _multiScenePreloader = new MultiScenePreloader(openedScenes);
        //         loadingOperations.Enqueue(_multiScenePreloader);
        //     }
        //
        //     loadingOperations.Enqueue(_multiScenePreloader);
        //
        //     _loadingProvider.LoadAndDestroy(loadingOperations).Forget();
        // }
    }
}