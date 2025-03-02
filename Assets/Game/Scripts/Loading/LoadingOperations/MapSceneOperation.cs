using System;
using Cysharp.Threading.Tasks;
using SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Loading.LoadingOperations
{
    public sealed class MapSceneOperation : ILoadingOperation
    {
        private SceneLoaderService _sceneLoader;

        public MapSceneOperation(SceneLoaderService sceneLoader)
        {
            _sceneLoader = sceneLoader;
        }

        public string Description => "Map Scene Loading...";

        public async UniTask Load(Action<float> onProgress)
        {
            onProgress?.Invoke(0f);
            var loadOperation = SceneManager.LoadSceneAsync(_sceneLoader.MapScene,
                LoadSceneMode.Single);

            while (loadOperation.isDone == false)
            {
                float progress = Mathf.Clamp01(loadOperation.progress);
                onProgress?.Invoke(progress);

                await UniTask.Yield();
            }

            onProgress?.Invoke(1f);
        }
    }
}