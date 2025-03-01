using System;
using Cysharp.Threading.Tasks;
using SceneManagement;
using UnityEngine.SceneManagement;

namespace Loading
{
    public sealed class OpenSceneLoadingOperation : ILoadingOperation
    {
        private SceneLoaderService _sceneLoader;

        public OpenSceneLoadingOperation(SceneLoaderService sceneLoader)
        {
            _sceneLoader = sceneLoader;
        }

        public string Description => "Open Scene loading...";

        public async UniTask Load(Action<float> onProgress)
        {
            onProgress?.Invoke(0.5f);
            var loadOp = SceneManager.LoadSceneAsync(_sceneLoader.OpenScene,
                LoadSceneMode.Additive);

            while (loadOp.isDone == false)
            {
                await UniTask.Yield();
            }

            onProgress?.Invoke(1f);
        }
    }
}