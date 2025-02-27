using AINavigation;
using Combat;
using Core;
using Core.NPC;
using Core.Player;
using Core.Quests;
using Data;
using DialogueEditor;
using Saving;
using SceneManagement;
using UI;
using UnityEngine;
using Zenject;

namespace Infrastructure.Installers.EntryPoint
{
    public class GamePlayCompositionRoot : MonoBehaviour, ICompose
    {
        [SerializeField] private SceneContext _sceneContext;
        [SerializeField] private ConversationManager _conversationManager; //
        [SerializeField] private SceneComponent _sceneComponent; //

        [SerializeField] private ConversationMarket _conversationMarket;
        [SerializeField] private NPCInteractable[] _npcInteractable;
        [SerializeField] private AnswerHandler[] _answerHandlers;
        [SerializeField] private ArrowForPlayer[] _arrowsForPlayer;
        [SerializeField] private PickableEquip[] _pickableEquip;
        [SerializeField] private GiveItem[] _giveItems;
        [SerializeField] private CheckItem[] _checkItems;
        [SerializeField] private IconForFarCamera[] _iconsForCamera;
        [SerializeField] private AIController[] _aiControllers;
        [SerializeField] private NPC_for_testID[] _npcForTestID;
        [SerializeField] private BossNPC[] _bosses;
        [SerializeField] private SavePoint[] _savePoints;
        [SerializeField] private Portal[] _portals;

        private DiContainer _sceneContainer;

        [Inject]
        public void Compose(DiContainer diContainer)
        {
            _sceneContainer = _sceneContext.Container;

            ConstructComponents();
        }

        private void ConstructComponents()
        {
            _conversationMarket.Construct(_sceneContainer.Resolve<UIManager>());

            foreach (NPCInteractable npc in _npcInteractable)
            {
                npc.Construct(_sceneContainer.Resolve<CursorManager>());
            }

            foreach (AnswerHandler answerHandler in _answerHandlers)
            {
                answerHandler.Construct(_sceneContainer.Resolve<SaveGame>());
            }

            foreach (ArrowForPlayer arrow in _arrowsForPlayer)
            {
                arrow.Construct(_sceneContainer.Resolve<ArrowForPlayerManager>(),
                    _sceneContainer.Resolve<MainPlayer>().PlayerController);
            }

            foreach (PickableEquip item in _pickableEquip)
            {
                item.Construct(_sceneContainer.Resolve<UIManager>(), _sceneContainer.Resolve<WeaponArmorManager>());
            }

            foreach (GiveItem item in _giveItems)
            {
                item.Construct(_sceneContainer.Resolve<UIManager>());
            }

            foreach (CheckItem item in _checkItems)
            {
                item.Construct(_sceneContainer.Resolve<UIManager>());
            }

            foreach (BossNPC boss in _bosses)
            {
                boss.Construct();
            }

            // foreach (IconForFarCamera icon in _iconsForCamera)
            // {
            //     icon.Construct(_sceneContainer.Resolve<UIManager>()); // TODO Player 
            // }

            foreach (AIController aiController in _aiControllers)
            {
                aiController.Construct(_sceneContainer.Resolve<MainPlayer>(), _sceneContainer.Resolve<IGame>());
            }

            foreach (NPC_for_testID npc in _npcForTestID) // TODO MOVE TO NPC script
            {
                npc.Construct(_sceneContainer.Resolve<GameAPI>(), _sceneContainer.Resolve<DataPlayer>(),
                    _sceneContainer.Resolve<FastTestsManager>(), _sceneContainer.Resolve<SaveGame>());
            }

            foreach (SavePoint savePoint in _savePoints)
            {
                savePoint.Construct(_sceneContainer.Resolve<DataPlayer>(),
                    _sceneContainer.Resolve<MainPlayer>().PlayerController.GetHealth());
            }

            foreach (Portal portal in _portals)
            {
                portal.Construct(_sceneContainer.Resolve<MainPlayer>(), _sceneContainer.Resolve<DataPlayer>(),
                    _sceneComponent);
            }
        }
    }
}