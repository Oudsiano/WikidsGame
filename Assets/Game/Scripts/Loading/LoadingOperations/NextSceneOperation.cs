using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Loading.LoadingOperations
{
    public sealed class NextSceneOperation : ILoadingOperation
    {
        private readonly int _index;

        public NextSceneOperation(int index)
        {
            _index = index;
        }

        public string Description => "Next Scene Loading...";

        public async UniTask Load(Action<float> onProgress)
        {
            onProgress?.Invoke(0f);
            var loadOperation = SceneManager.LoadSceneAsync(_index,
                LoadSceneMode.Single);

            while (loadOperation.isDone == false)
            {
                float progress = Mathf.Clamp01(loadOperation.progress);
                onProgress?.Invoke(progress);

                await UniTask.Yield();
            }

            onProgress?.Invoke(1f);
        }
    }
}