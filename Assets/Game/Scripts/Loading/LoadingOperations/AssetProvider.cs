using System;
using Cysharp.Threading.Tasks;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace Loading.LoadingOperations
{
    public class AssetProvider : ILoadingOperation
    {
        private bool _isReady;

        public string Description => "Assets Initialization...";

        public async UniTask Load(Action<float> progress)
        {
            var operation = Addressables.InitializeAsync();

            await operation.Task;

            _isReady = true;
        }

        public async UniTask<SceneInstance> LoadSceneAdditive(int sceneID)
        {
            await WaitUntilReady();

            var operation = Addressables.LoadSceneAsync(sceneID, LoadSceneMode.Additive);

            return await operation.Task;
        }

        public async UniTask UnloadAdditiveScene(SceneInstance scene)
        {
            await WaitUntilReady();

            var operation = Addressables.UnloadSceneAsync(scene);

            await operation.Task;
        }

        private async UniTask WaitUntilReady()
        {
            while (_isReady == false)
            {
                await UniTask.Yield();
            }
        }
    }
}