using System;
using Cysharp.Threading.Tasks;

namespace Loading.LoadingOperations
{
    public interface ILoadingOperation
    {
        public string Description { get; }

        public UniTask Load(Action<float> progress);
    }
}