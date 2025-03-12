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
        private readonly HashSet<string> _alreadyPreloaded = new();
        private Dictionary<string, AsyncOperationHandle> _handles = new();

        // --- Прогресс и статус ---
        public float Progress { get; private set; }
        public bool IsPreloading { get; private set; }
        public bool IsDone { get; private set; }

        public string Description => "Preloading available scenes...";

        public MultiScenePreloader(IEnumerable<string> sceneKeys)
        {
            _sceneKeys = new List<string>(sceneKeys);
            foreach (var scene in _sceneKeys)
                _alreadyPreloaded.Add(scene);
        }

        public async UniTask Load(Action<float> onProgress)
        {
            Progress = 0f;
            IsPreloading = true;
            IsDone = false;

            var handles = new List<AsyncOperationHandle>();

            for (int i = 0; i < _sceneKeys.Count; i++)
            {
                string key = _sceneKeys[i];
                if (await TryDownloadScene(key))
                    handles.Add(_handles[key]);

                Progress = (float)(i + 1) / _sceneKeys.Count;
                onProgress?.Invoke(Progress);

                await UniTask.Yield(); // отдаём кадр
            }

            Debug.Log($"[Preloader] 🎉 Finished preloading {_sceneKeys.Count} scenes. Loaded: {handles.Count}");

            IsPreloading = false;
            IsDone = true;
            Progress = 1f;
        }

        /// <summary>
        /// Запускает полную фоновую предзагрузку всех сцен
        /// </summary>
        public async void StartBackgroundPreloadAllScenes()
        {
            var allSceneKeys = GetAllSceneKeysFromAddressables();
            IsPreloading = true;
            IsDone = false;

            int total = allSceneKeys.Count;
            int loaded = 0;

            foreach (var sceneKey in allSceneKeys)
            {
                if (_alreadyPreloaded.Contains(sceneKey))
                {
                    Debug.Log($"[Preloader] 🔁 Scene '{sceneKey}' already preloaded, skipping.");
                    loaded++;
                    UpdateProgress(loaded, total);
                    continue;
                }

                await TryDownloadScene(sceneKey);
                loaded++;
                UpdateProgress(loaded, total);

                await UniTask.Yield(); // разгрузим кадр
            }

            Debug.Log("[Preloader] ✅ All background scenes preloaded.");
            IsPreloading = false;
            IsDone = true;
        }

        private async UniTask<bool> TryDownloadScene(string sceneKey)
        {
            if (string.IsNullOrWhiteSpace(sceneKey))
                return false;

            var locations = await Addressables.LoadResourceLocationsAsync(sceneKey).ToUniTask();
            if (locations == null || locations.Count == 0)
            {
                Debug.LogWarning($"[Preloader] ❌ Scene '{sceneKey}' not found in Addressables.");
                return false;
            }

            var size = await Addressables.GetDownloadSizeAsync(sceneKey).ToUniTask();
            if (size == 0)
            {
                Debug.Log($"[Preloader] ✅ Scene '{sceneKey}' already cached or in build.");
                return true;
            }

            var handle = Addressables.DownloadDependenciesAsync(sceneKey, true);
            await handle.Task;

            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                _handles[sceneKey] = handle;
                Debug.Log($"[Preloader] ✅ Scene '{sceneKey}' downloaded ({size / (1024f * 1024f):F2} MB)");
                return true;
            }
            else
            {
                Debug.LogError($"[Preloader] ❌ Failed to download scene '{sceneKey}'");
                return false;
            }
        }

        private void UpdateProgress(int loaded, int total)
        {
            Progress = (float)loaded / total;
            Debug.Log($"[Preloader] 📦 Progress: {(Progress * 100f):F0}%");
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