using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Loading.LoadingOperations
{
    public class ScenePreloader
    {
        private AsyncOperationHandle _downloadHandle;
        private CancellationTokenSource _cancellationTokenSource;
        private bool _isInitialized;

        public bool IsPreloadingComplete { get; private set; }
        public float PreloadProgress { get; private set; }

        public ScenePreloader()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            InitializeAddressablesAsync().Forget();
        }

        ~ScenePreloader()
        {
            _cancellationTokenSource?.Dispose();

            if (_downloadHandle.IsValid())
            {
                Addressables.Release(_downloadHandle);
            }
        }

        private async UniTask InitializeAddressablesAsync()
        {
            var initHandle = Addressables.InitializeAsync();
            await initHandle.ToUniTask();
            if (initHandle.Status == AsyncOperationStatus.Succeeded)
            {
                _isInitialized = true;
                Debug.Log("[ScenePreloader] Addressables initialized successfully.");
            }
            else
            {
                Debug.LogError($"[ScenePreloader] Failed to initialize Addressables: {initHandle.OperationException}");
            }
        }

        public async UniTask PreloadSceneAsync(string sceneKey, Action<float> onProgress,
            CancellationToken cancellationToken = default)
        {
            if (!_isInitialized)
            {
                Debug.LogWarning("[ScenePreloader] Addressables not initialized yet. Waiting...");
                await UniTask.WaitUntil(() => _isInitialized, cancellationToken: cancellationToken);
            }

            if (string.IsNullOrWhiteSpace(sceneKey))
            {
                Debug.LogWarning("[ScenePreloader] Scene key is empty or null.");
                onProgress?.Invoke(1f);
                IsPreloadingComplete = true;
                return;
            }

            IsPreloadingComplete = false;
            PreloadProgress = 0f;

            try
            {
                // Шаг 1: Проверяем размер загрузки
                long downloadSize = await GetDownloadSizeAsync(sceneKey, cancellationToken);
                if (downloadSize <= 0)
                {
                    Debug.Log(
                        $"[ScenePreloader] Scene '{sceneKey}' is already cached or has no dependencies to download.");
                    onProgress?.Invoke(1f);
                    IsPreloadingComplete = true;
                    return;
                }

                // Шаг 2: Предзагрузка зависимостей
                await PreloadDependenciesAsync(sceneKey, downloadSize, onProgress, cancellationToken);

                Debug.Log($"[ScenePreloader] Dependencies for '{sceneKey}' preloaded successfully!");
                onProgress?.Invoke(1f);
                IsPreloadingComplete = true;
            }
            catch (Exception e)
            {
                Debug.LogError($"[ScenePreloader] Failed to preload scene '{sceneKey}': {e.Message}");
                onProgress?.Invoke(1f);
                IsPreloadingComplete = true;
            }
        }

        private async UniTask<long> GetDownloadSizeAsync(string key, CancellationToken cancellationToken)
        {
            try
            {
                var sizeHandle = Addressables.GetDownloadSizeAsync(key);
                long size = await sizeHandle.ToUniTask(cancellationToken: cancellationToken);
                Debug.Log($"[ScenePreloader] Download size for '{key}': {size} bytes");
                return size;
            }
            catch (Exception e)
            {
                Debug.LogError($"[ScenePreloader] Failed to get download size for '{key}': {e.Message}");
                return 0;
            }
        }

        private async UniTask PreloadDependenciesAsync(string key, long totalSize, Action<float> onProgress,
            CancellationToken cancellationToken)
        {
            try
            {
                Debug.Log($"[ScenePreloader] Starting preload for scene '{key}'...");

                _downloadHandle = Addressables.DownloadDependenciesAsync(key, false);

                if (!_downloadHandle.IsValid())
                {
                    Debug.LogError($"[ScenePreloader] Invalid handle for '{key}'");
                    return;
                }

                while (!_downloadHandle.IsDone)
                {
                    PreloadProgress = _downloadHandle.PercentComplete;
                    onProgress?.Invoke(PreloadProgress);
                    Debug.Log($"[ScenePreloader] Preloading '{key}'... Progress: {PreloadProgress * 100:F2}%");
                    await UniTask.Yield(cancellationToken);
                }

                await _downloadHandle.ToUniTask(cancellationToken: cancellationToken);

                if (_downloadHandle.Status == AsyncOperationStatus.Succeeded)
                {
                    Debug.Log($"[ScenePreloader] Dependencies for '{key}' preloaded successfully!");
                }
                else
                {
                    Debug.LogError(
                        $"[ScenePreloader] Failed to preload dependencies for '{key}': {_downloadHandle.OperationException}");
                    throw new Exception($"Preload failed: {_downloadHandle.OperationException}");
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"[ScenePreloader] Error during preload of '{key}': {e.Message}");
                throw;
            }
        }

        public void CancelPreloading()
        {
            _cancellationTokenSource?.Cancel();
            Debug.Log("[ScenePreloader] Preloading cancelled.");

            if (_downloadHandle.IsValid())
            {
                Addressables.Release(_downloadHandle);
            }
        }

        public void Cleanup()
        {
            CancelPreloading();
            _cancellationTokenSource?.Dispose();
        }
    }
}