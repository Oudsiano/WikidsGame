using System;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace Loading.LoadingOperations
{
    public class AssetProvider : ILoadingOperation
    {
        private MultiScenePreloader _preloader;
        
        private bool _isReady;

        public string Description => "Assets Initialization...";

        public AssetProvider(MultiScenePreloader preloader)
        {
            _preloader = preloader;
        }
        
        public async UniTask Load(Action<float> progress)
        {
            _isReady = true;
        }

        public async UniTask<SceneInstance> LoadSceneAdditive(string nameScene)
        {
            await WaitUntilReady();

            var operation = Addressables.LoadSceneAsync(nameScene, LoadSceneMode.Additive);

            return await operation.Task;
        }

        public async UniTask<SceneInstance> LoadScene(string nameScene)
        {
            await WaitUntilScenePreloaded(nameScene);

            Debug.Log($"[AssetProvider] Trying to load scene: {nameScene}");

            var locations = await Addressables.LoadResourceLocationsAsync(nameScene).ToUniTask();

            if (locations == null || locations.Count == 0)
            {
                Debug.LogError($"[AssetProvider] Scene '{nameScene}' not found in Addressables.");
                throw new ArgumentException($"Scene '{nameScene}' not found in Addressables.");
            }

            var operation = Addressables.LoadSceneAsync(nameScene);
            await operation.Task;

            if (operation.Status == AsyncOperationStatus.Failed)
            {
                throw new ArgumentException($"Scene '{nameScene}' failed to load.");
            }

            return operation.Result;
        }

        public async UniTask UnloadAdditiveScene(SceneInstance scene)
        {
            await WaitUntilReady();

            var operation = Addressables.UnloadSceneAsync(scene);

            await operation.Task;
        }

        private async UniTask WaitUntilReady()
        {
            while (_isReady == false)
            {
                await UniTask.Yield();
            }
        }
        
        private async UniTask WaitUntilScenePreloaded(string nameScene)
        {
            if (_preloader.WasSceneSuccessfullyPreloaded(nameScene))
            {
                Debug.Log($"[AssetProvider] ✅ Scene '{nameScene}' was successfully preloaded.");
                return;
            }

            Debug.Log($"[AssetProvider] ⏳ Waiting for scene '{nameScene}' to be preloaded...");
            // try
            // {
            //     await UniTask.WaitUntil(() => _preloader.IsPreloadingComplete && _preloader.WasSceneSuccessfullyPreloaded(nameScene))
            //         .Timeout(TimeSpan.FromSeconds(30)); // Тайм-аут 30 секунд
            //     Debug.Log($"[AssetProvider] ✅ Scene '{nameScene}' is now preloaded.");
            // }
            // catch (TimeoutException)
            // {
            //     Debug.LogError($"[AssetProvider] ❌ Timeout waiting for scene '{nameScene}' to preload. Forcing load anyway.");
            //     // Принудительно считаем сцену готовой, если предзагрузка зависла
            // }
        }
    }
}