using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using SceneManagement;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;
using Utils;

namespace Loading.LoadingOperations
{
    public class MultiScenePreloader : ILoadingOperation
    {
        private readonly List<string> _sceneKeys;

        public string Description => "Preloading available scenes...";
        
        public MultiScenePreloader(IEnumerable<string> sceneKeys)
        {
            _sceneKeys = new List<string>(sceneKeys);
        }

        public async UniTask Load(Action<float> onProgress)
        {
            var handles = new List<AsyncOperationHandle>();

            for (int i = 0; i < _sceneKeys.Count; i++)
            {
                string key = _sceneKeys[i];

                if (string.IsNullOrWhiteSpace(key))
                {
                    Debug.LogWarning($"[Preloader] ❌ Empty scene key at index {i}");
                    continue;
                }

                var locations = await Addressables.LoadResourceLocationsAsync(key).ToUniTask();
                if (locations == null || locations.Count == 0)
                {
                    Debug.LogWarning($"[Preloader] ❌ Scene '{key}' not found in Addressables.");
                    continue;
                }

                var size = await Addressables.GetDownloadSizeAsync(key).ToUniTask();
                if (size == 0)
                {
                    Debug.Log($"[Preloader] ✅ Scene '{key}' has no content to download (cached or in build).");
                    continue;
                }

                var handle = Addressables.DownloadDependenciesAsync(key, true);
                await handle.Task;

                if (!handle.IsValid() || handle.Status == AsyncOperationStatus.Failed)
                {
                    Debug.LogError($"[Preloader] ❌ Failed to preload scene '{key}'");
                    continue;
                }

                float sizeMB = size / (1024f * 1024f);
                Debug.Log($"[Preloader] ✅ Scene '{key}' preloaded successfully ({sizeMB:F2} MB)");

                handles.Add(handle);
            }

            onProgress?.Invoke(1f);

            Debug.Log($"[Preloader] 🎉 Finished preloading {_sceneKeys.Count} scenes. Loaded: {handles.Count}");
        }

        public async void PreloadScenesInBackground()
        {
            var openedScenes = _sceneKeys;

            if (openedScenes.Count == 0)
                return;

            Debug.Log($"[MapRoot] ⏳ Starting background preload of {openedScenes.Count} scenes...");

            await Load(progress =>
            {
                Debug.Log($"[MapRoot] 📦 Background preload progress: {(progress * 100f):F0}%");
            });

            Debug.Log("[MapRoot] ✅ Background scene preloading complete!");
        }

        public async void PreloadAllScenesInBackground()
        {
            var allSceneKeys = GetAllSceneKeysFromAddressables();

            Debug.Log($"[MapRoot] 🎯 Starting background preload of {allSceneKeys.Count} scenes...");

            foreach (var sceneKey in allSceneKeys)
            {
                var size = await Addressables.GetDownloadSizeAsync(sceneKey).ToUniTask();

                if (size == 0)
                {
                    Debug.Log($"[Preloader] ✅ Scene '{sceneKey}' is already cached or in build.");
                    continue;
                }

                var handle = Addressables.DownloadDependenciesAsync(sceneKey, true);
                await handle.Task;

                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    Debug.Log($"[Preloader] ✅ Scene '{sceneKey}' preloaded ({size / (1024f * 1024f):F2} MB)");
                }
                else
                {
                    Debug.LogError($"[Preloader] ❌ Failed to preload scene: {sceneKey}");
                }

                await UniTask.Yield(); // освободим кадр, чтоб не фризило
            }

            Debug.Log($"[MapRoot] ✅ All background scene preloads complete.");
        }

        private List<string> GetAllSceneKeysFromAddressables()
        {
            return new List<string>
            {
                Constants.Scenes.FirstBattleScene,
                Constants.Scenes.SecondBattleScene,
                Constants.Scenes.ThirdBattleScene,
                Constants.Scenes.FourthBattleSceneDark,
                Constants.Scenes.FifthBattleSceneKingdom,
                Constants.Scenes.SixthBattleSceneKingdom,
                Constants.Scenes.SeventhBattleSceneViking,
                
                Constants.Scenes.FirstTownScene,
                Constants.Scenes.BossFightDarkScene,
                Constants.Scenes.BossFightKingdom1Scene,
                Constants.Scenes.BossFightKingdom2Scene,
                Constants.Scenes.BossFightViking1Scene,
                Constants.Scenes.LibraryScene,
                Constants.Scenes.HollScene,
                
                Constants.Scenes.EndScene,
            };
        }
    }
}