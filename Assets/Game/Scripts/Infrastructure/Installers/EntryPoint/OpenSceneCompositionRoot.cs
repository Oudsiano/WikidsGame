using Loading;
using Loading.LoadingOperations;
using SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils;
using Zenject;

namespace Infrastructure.Installers.EntryPoint
{
    public class OpenSceneCompositionRoot : MonoBehaviour, ICompose
    {
        [SerializeField] private SceneContext _sceneContext;
        [SerializeField] private LoadSceneAfterComix _loaderMapScene;

        private DiContainer _sceneContainer;

        [Inject]
        public void Compose(DiContainer diContainer)
        {
            _sceneContainer = _sceneContext.Container;

            _loaderMapScene.Construct(_sceneContainer.Resolve<LoadingScreenProvider>(),
                _sceneContainer.Resolve<SceneLoaderService>(), _sceneContainer.Resolve<AssetProvider>());
        }
    }
}