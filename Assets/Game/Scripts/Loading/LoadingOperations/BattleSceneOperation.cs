using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils;

namespace Loading.LoadingOperations
{
    public sealed class BattleSceneOperation : ILoadingOperation
    {
        private readonly string _nameScene;
        private AssetProvider _assetProvider;

        public BattleSceneOperation(string nameScene, AssetProvider assetProvider)
        {
            _nameScene = nameScene;
            _assetProvider = assetProvider;
        }

        public string Description => "Next Scene Loading...";

        public async UniTask Load(Action<float> onProgress)
        {
            onProgress?.Invoke(0f);
            var handle = await _assetProvider.LoadScene(_nameScene);

            onProgress?.Invoke(0.85f);
            await UniTask.Yield();
            onProgress?.Invoke(1.0f);
        }
    }
}