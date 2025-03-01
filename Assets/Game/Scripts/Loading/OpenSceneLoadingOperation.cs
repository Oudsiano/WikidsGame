using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Loading
{
    public sealed class OpenSceneLoadingOperation : ILoadingOperation
    {
        public string Description => "Open Scene loading...";

        public async UniTask Load(Action<float> onProgress)
        {
            onProgress?.Invoke(0.5f);
            var loadOp = SceneManager.LoadSceneAsync("OpenScene",
                LoadSceneMode.Additive);

            while (loadOp.isDone == false)
            {
                await UniTask.Yield();
            }

            onProgress?.Invoke(1f);
        }
    }
}