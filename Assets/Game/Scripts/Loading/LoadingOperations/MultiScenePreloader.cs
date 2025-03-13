using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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
    public class MultiScenePreloader 
    {
        private List<string> _sceneKeys;
        private int _maxConcurrentDownloads = 2; // Максимальное число параллельных загрузок

        private List<AsyncOperationHandle> _handles = new List<AsyncOperationHandle>();
        private CancellationTokenSource _cancellationToken;

        public bool IsPreloadingComplete { get; private set; }

        public void SetSceneKeys(List<string> sceneKeys) => _sceneKeys = sceneKeys;
        
        private void OnDestroy()
        {
            Cleanup();
            _cancellationToken?.Cancel();
        }

        public List<string> GetPreloadedKeys() => _sceneKeys;

        public async UniTask Load(Action<float> onProgress, CancellationToken cancellationToken = default)
        {
            IsPreloadingComplete = false;
            
            if (_sceneKeys == null || _sceneKeys.Count == 0)
            {
                Debug.LogWarning("[Preloader] No scenes to preload.");
                onProgress?.Invoke(1f);
                return;
            }

            _cancellationToken = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            var totalSize = 0L;
            var downloadedSize = 0L;

            // Предварительно вычисляем общий размер для прогресса
            var sizeTasks = new List<UniTask<long>>();
            foreach (var key in _sceneKeys)
            {
                if (string.IsNullOrWhiteSpace(key)) continue;
                sizeTasks.Add(GetDownloadSizeAsync(key, _cancellationToken.Token));
            }

            var sizes = await UniTask.WhenAll(sizeTasks);
            totalSize = sizes.Where(s => s > 0).Sum();

            if (totalSize == 0)
            {
                Debug.LogWarning("[Preloader] No downloadable content found.");
                onProgress?.Invoke(1f);
                return;
            }

            // Ограничение параллелизма с использованием SemaphoreSlim
            using var semaphore = new SemaphoreSlim(_maxConcurrentDownloads, _maxConcurrentDownloads);
            var tasks = new List<UniTask<long>>();

            foreach (var key in _sceneKeys)
            {
                if (string.IsNullOrWhiteSpace(key)) continue;

                await semaphore.WaitAsync(_cancellationToken.Token);
                tasks.Add(PreloadSceneAsync(key, semaphore, totalSize, onProgress, _cancellationToken.Token));
            }

            // Собираем размеры загруженных данных
            var downloadedSizes = await UniTask.WhenAll(tasks);
            downloadedSize = downloadedSizes.Sum();

            // Завершаем прогресс
            onProgress?.Invoke(1f);
            Debug.Log(
                $"[Preloaderv2] 🎉 Finished preloading {_handles.Count} scenes. Loaded: {_sceneKeys.Count - _handles.Count(h => h.Status != AsyncOperationStatus.Succeeded)}");

            // Очистка после завершения
            Cleanup();

            IsPreloadingComplete = true;
        }

        private async UniTask<long> GetDownloadSizeAsync(string key, CancellationToken token)
        {
            try
            {
                return await Addressables.GetDownloadSizeAsync(key).ToUniTask(cancellationToken: token);
            }
            catch (Exception e)
            {
                Debug.LogError($"[Preloader] Failed to get size for '{key}': {e.Message}");
                return 0;
            }
        }

        private async UniTask<long> PreloadSceneAsync(string key, SemaphoreSlim semaphore, long totalSize,
            Action<float> onProgress, CancellationToken token)
        {
            long size = 0;
            try
            {
                var handle = Addressables.DownloadDependenciesAsync(key, false);

                Debug.Log($"[Preloader] Started preloading '{key}'");

                await handle.ToUniTask(cancellationToken: token);

                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    size = await GetDownloadSizeAsync(key, token);
                    float progress = (float)_handles.Sum(h =>
                        h.IsValid() && h.Status == AsyncOperationStatus.Succeeded
                            ? h.GetDownloadStatus().TotalBytes
                            : 0) / totalSize;
                    onProgress?.Invoke(Mathf.Clamp01(progress));
                    Debug.Log($"[Preloader] ✅ Scene '{key}' preloaded ({size / (1024f * 1024f):F2} MB)");
                    _handles.Add(handle); //
                }
                else
                {
                    Debug.LogError($"[Preloader] ❌ Failed to preload '{key}' with status: {handle.Status}");
                }
            }
            catch (OperationCanceledException)
            {
                Debug.LogWarning($"[Preloader] Preloading '{key}' was canceled.");
            }
            catch (Exception e)
            {
                Debug.LogError($"[Preloader] Error preloading '{key}': {e.Message}");
            }
            finally
            {
                semaphore.Release();
            }

            return size;
        }

        private void Cleanup()
        {
            foreach (var handle in _handles)
            {
                if (handle.IsValid())
                    Addressables.Release(handle);
            }

            _handles.Clear();
        }

        public async UniTask PreloadRemainingScenesInBackground()
        {
            var allSceneKeys = new HashSet<string>(GetAllSceneKeysFromAddressables());
            var loadedKeys = new HashSet<string>(_sceneKeys); 
            allSceneKeys.ExceptWith(loadedKeys);

            Debug.Log($"[Preloader] ⏳ Background preload of remaining {allSceneKeys.Count} scenes...");

            foreach (var key in allSceneKeys)
            {
                var size = await Addressables.GetDownloadSizeAsync(key).ToUniTask();

                if (size == 0)
                {
                    Debug.Log($"[Preloader] ✅ Scene '{key}' already cached.");
                    continue;
                }

                var handle = Addressables.DownloadDependenciesAsync(key, true);
                await handle.Task;

                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    Debug.Log($"[Preloader] ✅ Scene '{key}' preloaded ({size / (1024f * 1024f):F2} MB)");
                    _handles.Add(handle);
                }
                else
                {
                    Debug.LogError($"[Preloader] ❌ Failed to preload scene: {key}");
                }

                await UniTask.Yield(); // освобождаем кадр
            }

            Debug.Log($"[Preloader] 🎉 Remaining scenes preloaded.");
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