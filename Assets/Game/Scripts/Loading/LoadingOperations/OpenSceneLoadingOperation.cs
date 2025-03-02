using System;
using Cysharp.Threading.Tasks;
using SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Loading.LoadingOperations
{
    public sealed class OpenSceneLoadingOperation : ILoadingOperation
    {
        private readonly SceneLoaderService _sceneLoader;

        public OpenSceneLoadingOperation(SceneLoaderService sceneLoader)
        {
            _sceneLoader = sceneLoader;
        }

        public string Description => "Open Scene loading...";

        public async UniTask Load(Action<float> onProgress)
        {
            onProgress?.Invoke(0f);
            var loadOperation = SceneManager.LoadSceneAsync(_sceneLoader.OpenScene,
                LoadSceneMode.Additive);

            if (loadOperation == null)
            {
                Debug.LogError($"Failed to load scene: {_sceneLoader.OpenScene}");
                return;
            }

            while (loadOperation.isDone == false)
            {
                float progress = Mathf.Clamp01(loadOperation.progress / 0.9f);
                onProgress?.Invoke(progress);

                await UniTask.Yield();
            }

            onProgress?.Invoke(1f);
            Debug.Log($"Scene loaded: {_sceneLoader.OpenScene}");
        }
    }
}