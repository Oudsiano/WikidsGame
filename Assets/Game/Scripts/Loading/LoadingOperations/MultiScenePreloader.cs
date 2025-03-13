using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Utils;

namespace Loading.LoadingOperations
{
    public class MultiScenePreloader
    {
        private List<string> _sceneKeys;
        private int _maxConcurrentDownloads = 2;

        private List<AsyncOperationHandle> _handles = new();
        private HashSet<string> _successfullyPreloaded = new();
        private CancellationTokenSource _cancellationToken;

        public bool IsPreloadingComplete { get; private set; }

        public void SetSceneKeys(List<string> sceneKeys) => _sceneKeys = sceneKeys;

        public bool WasSceneSuccessfullyPreloaded(string sceneKey) => _successfullyPreloaded.Contains(sceneKey);

        public List<string> GetPreloadedKeys() => _sceneKeys;

        public void Cleanup()
        {
            foreach (var handle in _handles)
            {
                if (handle.IsValid())
                    Addressables.Release(handle);
            }

            _handles.Clear();
            _successfullyPreloaded.Clear();
        }

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

            var sizeTasks = new List<UniTask<long>>();
            foreach (var key in _sceneKeys)
            {
                if (!string.IsNullOrWhiteSpace(key))
                    sizeTasks.Add(GetDownloadSizeAsync(key, _cancellationToken.Token));
            }

            var sizes = await UniTask.WhenAll(sizeTasks);
            totalSize = sizes.Where(s => s > 0).Sum();

            if (totalSize == 0)
            {
                Debug.LogWarning("[Preloader] No downloadable content found. Marking as preloaded.");

                foreach (var key in _sceneKeys)
                {
                    if (!string.IsNullOrWhiteSpace(key))
                    {
                        _successfullyPreloaded.Add(key);
                    }
                }

                onProgress?.Invoke(1f);
                IsPreloadingComplete = true;
                return;
            }

            using var semaphore = new SemaphoreSlim(_maxConcurrentDownloads, _maxConcurrentDownloads);
            var tasks = new List<UniTask<long>>();

            foreach (var key in _sceneKeys)
            {
                if (string.IsNullOrWhiteSpace(key)) continue;

                await semaphore.WaitAsync(_cancellationToken.Token);
                tasks.Add(PreloadSceneAsync(key, semaphore, totalSize, onProgress, _cancellationToken.Token));
            }

            await UniTask.WhenAll(tasks);

            onProgress?.Invoke(1f);
            Debug.Log($"[Preloader] 🎉 Finished preloading. Loaded scenes: {_successfullyPreloaded.Count}");

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
                Debug.Log($"[Preloader] ⏳ Starting download for: {key}");

                // Используем `true` чтобы точно кэшировалось в WebGL
                var handle = Addressables.DownloadDependenciesAsync(key, true);
                await handle.ToUniTask(cancellationToken: token);

                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    size = await GetDownloadSizeAsync(key, token);

                    if (size == 0)
                    {
                        _successfullyPreloaded.Add(key);
                        _handles.Add(handle);

                        float progress = (float)_successfullyPreloaded.Count / _sceneKeys.Count;
                        onProgress?.Invoke(Mathf.Clamp01(progress));

                        Debug.Log($"✅ [Preloader] Scene '{key}' fully preloaded & cached");
                    }
                    else
                    {
                        Debug.LogWarning(
                            $"⚠️ [Preloader] Scene '{key}' marked as succeeded but still has remaining data: {size} bytes");
                    }
                }
                else
                {
                    Debug.LogError($"❌ [Preloader] Failed to preload '{key}' (Status: {handle.Status})");
                }
            }
            catch (OperationCanceledException)
            {
                Debug.LogWarning($"❌ [Preloader] Canceled preloading '{key}'");
            }
            catch (Exception e)
            {
                Debug.LogError($"❌ [Preloader] Error preloading '{key}': {e.Message}");
            }
            finally
            {
                semaphore.Release();
            }

            return size;
        }

        public async UniTask PreloadRemainingScenesInBackground()
        {
            var allSceneKeys = new HashSet<string>(GetAllSceneKeysFromAddressables());
            allSceneKeys.ExceptWith(_successfullyPreloaded);

            Debug.Log($"[Preloader] 💤 Background preload of {allSceneKeys.Count} scenes...");

            foreach (var key in allSceneKeys)
            {
                if (string.IsNullOrWhiteSpace(key)) continue;

                try
                {
                    var initialSize = await Addressables.GetDownloadSizeAsync(key).ToUniTask();
                    if (initialSize == 0)
                    {
                        Debug.Log($"[Preloader] ✅ Scene '{key}' already cached (initial check).");
                        _successfullyPreloaded.Add(key);
                        continue;
                    }

                    var handle = Addressables.DownloadDependenciesAsync(key, true);
                    await handle.ToUniTask();

                    if (handle.Status == AsyncOperationStatus.Succeeded)
                    {
                        var remainingSize = await Addressables.GetDownloadSizeAsync(key).ToUniTask();
                        if (remainingSize == 0)
                        {
                            Debug.Log(
                                $"[Preloader] ✅ Scene '{key}' background preloaded ({initialSize / (1024f * 1024f):F2} MB)");
                            _handles.Add(handle);
                            _successfullyPreloaded.Add(key);
                        }
                        else
                        {
                            Debug.LogWarning(
                                $"⚠️ [Preloader] Scene '{key}' loaded but not fully cached. Remaining: {remainingSize} bytes");
                        }
                    }
                    else
                    {
                        Debug.LogError($"❌ [Preloader] Failed to background preload scene: {key}");
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError($"❌ [Preloader] Exception during background preload of '{key}': {e.Message}");
                }

                await UniTask.Yield(); 
            }

            Debug.Log(
                $"[Preloader] ✅ Background preload complete. Total cached scenes: {_successfullyPreloaded.Count}");
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