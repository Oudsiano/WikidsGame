using Cysharp.Threading.Tasks;
using Loading;
using Loading.LoadingOperations;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils;

namespace SceneManagement
{
    public class LoadSceneAfterComix : MonoBehaviour
    {   
        private SceneLoaderService _sceneLoaderService;
        private LoadingScreenProvider _loadingScreenProvider;
        private AssetProvider _assetProvider;

        public void Construct(LoadingScreenProvider loadingScreenProvider, SceneLoaderService sceneLoaderService, AssetProvider assetProvider)
        {
            _loadingScreenProvider = loadingScreenProvider;
            _sceneLoaderService = sceneLoaderService;
            _assetProvider = assetProvider;
        }

        public void LoadSceneNext()
        {
            SceneManager.LoadScene(Constants.Scenes.MapScene);
        }
    }
}