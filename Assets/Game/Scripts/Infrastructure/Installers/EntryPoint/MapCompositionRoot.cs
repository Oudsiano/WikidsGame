using Data;
using Saving;
using SceneManagement;
using UnityEngine;
using Zenject;

namespace Infrastructure.Installers.EntryPoint
{
    public class MapCompositionRoot : MonoBehaviour, ICompose
    {
        [SerializeField] private SceneContext _sceneContext;

        [SerializeField] private LocationChange _locationChange;
        
        private DiContainer _sceneContainer;

        [Inject]
        public void Compose(DiContainer diContainer)
        {
            _sceneContainer = _sceneContext.Container;

            ConstructComponents();
        }

        private void ConstructComponents()
        {
            _locationChange.Construct(_sceneContainer.Resolve<DataPlayer>(),
                _sceneContainer.Resolve<LevelChangeObserver>(), 
                _sceneContainer.Resolve<SceneLoaderService>(),
                _sceneContainer.Resolve<GameAPI>());
        }
    }
}