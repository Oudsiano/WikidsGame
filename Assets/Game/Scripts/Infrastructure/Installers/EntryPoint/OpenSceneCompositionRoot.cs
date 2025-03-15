using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Data;
using Loading;
using Loading.LoadingOperations;
using Saving;
using SceneManagement;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;
using Utils;
using Zenject;

namespace Infrastructure.Installers.EntryPoint
{
    public class OpenSceneCompositionRoot : MonoBehaviour, ICompose
    {
        [SerializeField] private SceneContext _sceneContext;
        [SerializeField] private LoadSceneAfterComix _loaderMapScene;

        private DiContainer _sceneContainer;
        private MultiScenePreloader _preloader;

        [Inject]
        public void Compose(DiContainer diContainer)
        {
            _sceneContainer = _sceneContext.Container;

            _loaderMapScene.Construct(_sceneContainer.Resolve<LoadingScreenProvider>(),
                _sceneContainer.Resolve<SceneLoaderService>(), _sceneContainer.Resolve<AssetProvider>());

            _preloader = _sceneContainer.Resolve<MultiScenePreloader>();
        }

        private async void Start()
        {
            ScenePreloader preloader = new ScenePreloader();
            
            await preloader.PreloadSceneAsync("FirstBattleScene", progress =>
            {
                Debug.Log($"Preload Progress: {progress * 100:F2}%");
                // Здесь можно обновить UI прогресс-бара
            });
            
            var sceneHandle = Addressables.LoadSceneAsync("FirstBattleScene", LoadSceneMode.Single, true);
            await sceneHandle.ToUniTask();
            if (sceneHandle.Status == AsyncOperationStatus.Succeeded)
            {
                Debug.Log("Scene loaded!");
            }
        }
    }
}