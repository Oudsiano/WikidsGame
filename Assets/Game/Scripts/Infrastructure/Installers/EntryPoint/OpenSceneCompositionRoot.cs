using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Data;
using Loading;
using Loading.LoadingOperations;
using Saving;
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
        private MultiScenePreloader _preloader;

        [Inject]
        public void Compose(DiContainer diContainer)
        {
            _sceneContainer = _sceneContext.Container;

            _loaderMapScene.Construct(_sceneContainer.Resolve<LoadingScreenProvider>(),
                _sceneContainer.Resolve<SceneLoaderService>(), _sceneContainer.Resolve<AssetProvider>());

            _preloader = _sceneContainer.Resolve<MultiScenePreloader>();
        }

        private void Start()
        {
            _sceneContainer.Resolve<BootstrapFlowService>()
                .RunBootstrapFlow(_sceneContainer.Resolve<GameAPI>(), _sceneContainer.Resolve<MultiScenePreloader>(),
                    _sceneContainer.Resolve<DataPlayer>())
                .Forget();
        }
    }
}