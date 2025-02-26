using AINavigation;
using Core.Player;
using Data;
using DialogueEditor;
using Saving;
using UnityEngine;
using Zenject;

namespace Infrastructure.Installers.EntryPoint
{
    public class GamePlayCompositionRoot : MonoBehaviour, ICompose
    {
        [SerializeField] private SceneContext _sceneContext;
        [SerializeField] private ConversationManager _conversationManager; //
        [SerializeField] private AIController[] _aiControllers;
        [SerializeField] private SavePoint[] _savePoints;

        private DiContainer _sceneContainer;

        [Inject]
        public void Compose(DiContainer diContainer)
        {
            _sceneContainer = _sceneContext.Container;

            ConstructComponents();
        }

        private void ConstructComponents()
        {
            foreach (AIController aiController in _aiControllers)
            {
                aiController.Construct(_sceneContainer.Resolve<MainPlayer>(), _sceneContainer.Resolve<IGame>());
            }           
            
            foreach (SavePoint savePoint in _savePoints)
            {
                savePoint.Construct(_sceneContainer.Resolve<DataPlayer>(),
                    _sceneContainer.Resolve<MainPlayer>().PlayerController.GetHealth());
            }
        }
    }
}