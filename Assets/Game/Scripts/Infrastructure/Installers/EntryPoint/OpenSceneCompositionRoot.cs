using UnityEngine;
using Zenject;

namespace Infrastructure.Installers.EntryPoint
{
    public class OpenSceneCompositionRoot : MonoBehaviour, ICompose
    {
        [SerializeField] private SceneContext _sceneContext;
        
        private DiContainer _sceneContainer; 
        
        public void Compose(DiContainer diContainer)
        {
            Debug.Log("Open Scene compose");
        }
    }
}