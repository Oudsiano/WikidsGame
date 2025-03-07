using Data;
using Saving;
using SceneManagement;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject;

namespace Infrastructure.Installers.EntryPoint
{
    public class MapCompositionRoot : MonoBehaviour, ICompose
    {
        [SerializeField] private SceneContext _sceneContext;

        [SerializeField] private CanvasSwitcher _canvasSwitcher;
        [FormerlySerializedAs("_locationChange")] [SerializeField] private LocationChange _locationChangeDesktop;
        
        [FormerlySerializedAs("_locationChangeMobie")] [SerializeField] private LocationChange _locationChangeMobile;
        
        private DiContainer _sceneContainer;
        
        private LocationChange _locationChange;
        

        [Inject]
        public void Compose(DiContainer diContainer)
        {
            _sceneContainer = _sceneContext.Container;

            SetLocationChange();
            ConstructComponents();
        }

        private void ConstructComponents()
        {
            _locationChange.Construct(_sceneContainer.Resolve<DataPlayer>(),
                _sceneContainer.Resolve<LevelChangeObserver>(), 
                _sceneContainer.Resolve<SceneLoaderService>(),
                _sceneContainer.Resolve<GameAPI>());
        }

        private void SetLocationChange()
        {
            bool isMobile = DeviceChecker.IsMobileDevice();

            _locationChange = isMobile ? _locationChangeMobile : _locationChangeDesktop;
            
            _canvasSwitcher.SetCanvasChange(isMobile);
        }
    }
}