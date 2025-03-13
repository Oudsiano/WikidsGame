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
            await UniTask.WaitUntil(() => _sceneContainer.Resolve<GameAPI>().GameLoad,
                cancellationToken: this.GetCancellationTokenOnDestroy());

            LaunchBootstrapFlow().Forget();
        }

        private async UniTaskVoid LaunchBootstrapFlow()
        {
            try
            {
                Debug.Log("[Bootstrap] 🚀 Starting bootstrap flow");

                var dataPlayer = _sceneContainer.Resolve<DataPlayer>();
                List<string> openedScenes;
                
                if (dataPlayer?.PlayerData?.FinishedRegionsName == null)
                {
                    Debug.LogWarning("[Bootstrap] FinishedRegionsName is null, defaulting to FirstBattleScene");
                    openedScenes = new List<string> { Constants.Scenes.FirstBattleScene };
                }
                else
                {
                    openedScenes = dataPlayer.PlayerData.FinishedRegionsName;
                    Debug.Log($"[Bootstrap] Found {openedScenes.Count()} opened scenes");
                }
                
                if (openedScenes == null || !openedScenes.Any())
                {
                    Debug.LogWarning("[Bootstrap] FinishedRegionsName is null, defaulting to FirstBattleScene");
                    openedScenes = new List<string> { Constants.Scenes.FirstBattleScene };
                }
                
                _preloader.SetSceneKeys(openedScenes);

                await _preloader.Load(progress =>
                {
                    Debug.Log($"[Bootstrap] 🎯 Preloading opened scenes... {(progress * 100f):F0}%");
                });

                Debug.Log("[Bootstrap] ✅ Opened scenes preloading complete");

                await UniTask.WaitUntil(() => _preloader.IsPreloadingComplete);

                _preloader.PreloadRemainingScenesInBackground().Forget();
            }
            catch (Exception e)
            {
                Debug.LogError($"[Bootstrap] Error in LaunchBootstrapFlow: {e.Message}\n{e.StackTrace}");
            }
        }
    }
}