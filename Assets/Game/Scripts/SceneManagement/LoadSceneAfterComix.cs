using Cysharp.Threading.Tasks;
using Loading;
using Loading.LoadingOperations;
using UnityEngine;

namespace SceneManagement
{
    public class LoadSceneAfterComix : MonoBehaviour
    {
        private SceneLoaderService _sceneLoaderService;
        private LoadingScreenProvider _loadingScreenProvider;

        public void Construct(LoadingScreenProvider loadingScreenProvider, SceneLoaderService sceneLoaderService)
        {
            _loadingScreenProvider = loadingScreenProvider;
            _sceneLoaderService = sceneLoaderService;
        }

        public void LoadSceneNext()
        {
            _loadingScreenProvider.LoadAndDestroy(new MapSceneOperation(_sceneLoaderService)).Forget();
        }
    }
}