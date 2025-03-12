using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Utils;

namespace Loading
{
    public class AssetPreloader
    {
        private readonly Dictionary<string, AsyncOperationHandle<GameObject>> _loadedAssets = new();
        private bool _isLoadingInProgress = false;

        public bool IsLoadingInProgress => _isLoadingInProgress;

        public async UniTask LoadAllAssets()
        {
            if (_isLoadingInProgress)
            {
                Debug.LogWarning("Загрузка уже выполняется");
                return;
            }

            _isLoadingInProgress = true;
            _loadedAssets.Clear();

            var assetNames = GetAllAssetNames();

            try
            {
                foreach (var assetId in assetNames)
                {
                    await LoadAsset(assetId);
                }

                Debug.Log($"[LocalAssetLoader] Все {assetNames.Count} ассетов загружены.");
            }
            catch (Exception ex)
            {
                Debug.LogError($"[LocalAssetLoader] Ошибка при загрузке: {ex.Message}");
                ReleaseAllAssets();
            }
            finally
            {
                _isLoadingInProgress = false;
            }
        }

        private async UniTask LoadAsset(string assetId)
        {
            if (_loadedAssets.ContainsKey(assetId))
                return;

            var handle = Addressables.LoadAssetAsync<GameObject>(assetId);
            await handle.Task;

            if (handle.Status != AsyncOperationStatus.Succeeded)
                throw new Exception($"Не удалось загрузить ассет {assetId}");

            _loadedAssets[assetId] = handle;
        }

        public GameObject GetLoadedAsset(string assetId)
        {
            if (_loadedAssets.TryGetValue(assetId, out var handle))
            {
                return handle.Result;
            }

            Debug.LogWarning($"[LocalAssetLoader] Ассет {assetId} не найден в кэше.");
            return null;
        }

        public GameObject InstantiateLoadedAsset(string assetId, Transform parent = null)
        {
            var prefab = GetLoadedAsset(assetId);

            if (prefab != null)
            {
                return UnityEngine.Object.Instantiate(prefab, parent);
            }

            throw new NullReferenceException($"[LocalAssetLoader] Ассет {assetId} не найден в кэше для инстанциации.");
        }

        public void ReleaseAllAssets()
        {
            foreach (var kvp in _loadedAssets)
            {
                Addressables.Release(kvp.Value);
            }

            _loadedAssets.Clear();
            Debug.Log("[LocalAssetLoader] Все ассеты выгружены.");
        }

        private List<string> GetAllAssetNames()
        {
            return new List<string>
            {
                Constants.Scenes.FirstBattleScene,
                // Constants.Scenes.SecondBattleScene,
                // Constants.Scenes.ThirdBattleScene,
                // Constants.Scenes.FourthBattleSceneDark,
                // Constants.Scenes.FifthBattleSceneKingdom,
                // Constants.Scenes.SixthBattleSceneKingdom,
                // Constants.Scenes.SeventhBattleSceneViking,
                // Constants.Scenes.FirstTownScene,
                // Constants.Scenes.BossFightDarkScene,
                // Constants.Scenes.BossFightKingdom1Scene,
                // Constants.Scenes.BossFightKingdom2Scene,
                // Constants.Scenes.BossFightViking1Scene,
                // Constants.Scenes.LibraryScene,
                // Constants.Scenes.HollScene,
                // Constants.Scenes.EndScene
            };
        }
    }
}