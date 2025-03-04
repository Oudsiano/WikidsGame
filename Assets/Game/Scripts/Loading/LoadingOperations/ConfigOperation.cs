using System;
using Cysharp.Threading.Tasks;
using UnityEngine.AddressableAssets;

namespace Loading.LoadingOperations
{
    public sealed class ConfigOperation : ILoadingOperation
    {
        public string Description => "Config Loading...";

        public async UniTask Load(Action<float> onProgress)
        {
            onProgress?.Invoke(0f);
            
            await UniTask.Yield();
            
            onProgress?.Invoke(1f);
        }
    }
}