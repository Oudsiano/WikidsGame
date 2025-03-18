using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;

namespace Loading.LoadingOperations.Preloading
{
    public class ScenePreloader
    {
        private readonly Dictionary<string, AsyncOperationHandle<SceneInstance>> _loadedScenes = new();

        public async UniTask PreloadSceneInBackground(string sceneKey)
        {
            if (_loadedScenes.ContainsKey(sceneKey))
                return;

            var handle = Addressables.LoadSceneAsync(
                sceneKey,
                activateOnLoad: false
            );

            await handle.ToUniTask();

            _loadedScenes[sceneKey] = handle;
            Debug.Log($"[ScenePreloadController] Preloaded scene: {sceneKey}");
        }

        public async UniTask ActivateScene(string sceneKey)
        {
            if (_loadedScenes.TryGetValue(sceneKey, out var handle) == false)
            {
                Debug.LogWarning($"[ScenePreloader] Scene {sceneKey} not preloaded. Loading directly...");
                handle = Addressables.LoadSceneAsync(sceneKey);
                await handle.ToUniTask();
            }
            else
            {
                Debug.Log($"[ScenePreloader] Activating scene: {sceneKey}");
                await handle.Result.ActivateAsync();
            }
        }
    }
}