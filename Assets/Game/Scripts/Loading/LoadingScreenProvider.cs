using System.Collections.Generic;
using Utils;
using Cysharp.Threading.Tasks;
using Loading.LoadingOperations;

namespace Loading
{
    public sealed class LoadingScreenProvider : LocalAssetLoader
    {
        public async UniTask LoadAndDestroy(ILoadingOperation loadingOperation)
        {
            var operations = new Queue<ILoadingOperation>();
            operations.Enqueue(loadingOperation);
            
            await LoadAndDestroy(operations);
        }

        public async UniTask LoadAndDestroy(Queue<ILoadingOperation> loadingOperations)
        {
            var loadingScreen = await LoadAndInstantiate<LoadingScreen>(Constants.Assets.LoadingScreen);

            await loadingScreen.Load(loadingOperations);

            Unload();
        }
    }
}