using Cysharp.Threading.Tasks;
using Loading;
using Loading.LoadingOperations;
using Loading.LoadingOperations.Preloading;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils;

namespace SceneManagement
{
    public class LoadSceneAfterComix : MonoBehaviour
    {
        private LoadingScreenProvider _loadingScreenProvider;
        private AssetProvider _assetProvider;
    private ScenePreloader _scenePreloader;
    
        public void Construct(LoadingScreenProvider loadingScreenProvider, AssetProvider assetProvider, ScenePreloader scenePreloader)
        {
            _loadingScreenProvider = loadingScreenProvider;
            _assetProvider = assetProvider;
            _scenePreloader = scenePreloader;
        }

        public void LoadSceneNext()
        {
            SceneManager.LoadScene(Constants.Scenes.MapScene);
        }
    }
}