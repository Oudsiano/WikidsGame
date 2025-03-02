using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Zenject;

namespace Loading
{
    public class LocalAssetLoader
    {
        private GameObject _cachedObject;

        public async UniTask<T> Load<T>(string assetId) where T : Component
        {
            var handle = Addressables.LoadAssetAsync<GameObject>(assetId);
            GameObject prefab = await handle.Task;

            if (prefab == null)
            {
                throw new NullReferenceException($"Failed to load prefab with assetId: {assetId}");
            }

            if (!prefab.TryGetComponent(out T component))
            {
                throw new NullReferenceException($"Object of type {typeof(T)} is null on " +
                                                 "attempt to load it from addressables");
            }

            return component;
        }

        public async UniTask<T> LoadAndInstantiate<T>(string assetId, Transform parent = null)
        {
            var handle = Addressables.InstantiateAsync(assetId, parent);
            _cachedObject = await handle.Task;

            if (_cachedObject.TryGetComponent(out T component) == false)
            {
                throw new NullReferenceException($"Object of type {typeof(T)} is null on " +
                                                 "attempt to load it from addressables");
            }

            return component;
        }

        public async UniTask<Disposable<T>> LoadDisposable<T>(string assetId, Transform parent = null)
        {
            var component = await LoadAndInstantiate<T>(assetId, parent);

            return Disposable<T>.Borrow(component, _ => Unload());
        }

        public void Unload()
        {
            if (_cachedObject == null)
                return;

            _cachedObject.SetActive(false);
            Addressables.ReleaseInstance(_cachedObject);
            _cachedObject = null;
        }
    }
}