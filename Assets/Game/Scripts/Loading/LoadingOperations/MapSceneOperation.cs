using System;
using Cysharp.Threading.Tasks;
using SceneManagement;
using Utils;

namespace Loading.LoadingOperations
{
    public sealed class MapSceneOperation : ILoadingOperation
    {
        private SceneLoaderService _sceneLoader;
        private readonly AssetProvider _assetProvider;

        public MapSceneOperation(SceneLoaderService sceneLoader, AssetProvider assetProvider)
        {
            _sceneLoader = sceneLoader;
            _assetProvider = assetProvider;
        }

        public string Description => "Map Scene Loading...";

        public async UniTask Load(Action<float> onProgress)
        {
            onProgress?.Invoke(0f);
            var handle = await _assetProvider.LoadScene(Constants.Scenes.MapScene);

            onProgress?.Invoke(0.85f);
            await UniTask.Yield();
            onProgress?.Invoke(1.0f);
        }
    }
}