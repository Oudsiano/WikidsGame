using System;
using Cysharp.Threading.Tasks;
using SceneManagement;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Utils;

namespace Loading.LoadingOperations
{
    public sealed class OpenSceneLoadingOperation : ILoadingOperation
    {
        private readonly AssetProvider _assetProvider;

        public OpenSceneLoadingOperation(AssetProvider assetProvider)
        {
            _assetProvider = assetProvider;
        }

        public string Description => "Open Scene loading...";

        public async UniTask Load(Action<float> onProgress)
        {
            onProgress?.Invoke(0f);

            var handle = await _assetProvider.LoadScene(Constants.Scenes.OpenScene);

            onProgress?.Invoke(0.85f);
            await UniTask.Yield();
            onProgress?.Invoke(1.0f);
        }
    }
}