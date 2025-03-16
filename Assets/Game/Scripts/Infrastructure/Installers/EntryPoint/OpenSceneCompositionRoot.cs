using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Data;
using Loading;
using Loading.LoadingOperations;
using Loading.LoadingOperations.Preloading;
using Saving;
using SceneManagement;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
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

            _loaderMapScene.Construct(
                _sceneContainer.Resolve<LoadingScreenProvider>(), _sceneContainer.Resolve<AssetProvider>(),
                _sceneContainer.Resolve<ScenePreloader>());
        }
    }
}