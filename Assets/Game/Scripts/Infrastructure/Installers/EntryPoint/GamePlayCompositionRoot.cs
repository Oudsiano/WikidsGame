using AINavigation;
using Combat;
using Core;
using Core.NPC;
using Core.Player;
using Core.Player.MovingBetweenPoints;
using Core.Quests;
using Data;
using DialogueEditor;
using Loading.LoadingOperations.Preloading;
using Saving;
using SceneManagement;
using UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace Infrastructure.Installers.EntryPoint
{
    public class GamePlayCompositionRoot : MonoBehaviour, ICompose
    {
        [SerializeField] private SceneContext _sceneContext;
        
        [SerializeField] private SceneComponent _sceneComponent;
        [SerializeField] private Map _map;
        
        [SerializeField] private ConversationStarter[] _conversationStarters;
        [SerializeField] private DownloadTestData[] _downloadTestData;
        [SerializeField] private SetStudyStep[] _setStudySteps;
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
        [SerializeField] private EnemySpawner _enemySpawner;

        private DiContainer _sceneContainer;

        [Inject]
        public void Compose(DiContainer diContainer)
        {
            _sceneContainer = _sceneContext.Container;

            ConstructComponents();
        }

        private void ConstructComponents()
        {
            Debug.Log("Начало ConstructComponents");
            
            var uiManager = _sceneContainer.Resolve<UIManager>();
            var container = uiManager.PointViewContainer;
            container.ClearButtons();
            
            if (_map != null)
            {
                var handler = _sceneContainer.Resolve<PointClickHandler>();
                _map.Construct(handler);
                
                if (container != null)
                {
                    container.Construct(_map.GetPointsView, handler, uiManager);
                }

                uiManager.SetMapImage(_map.Image);
            }

            Debug.Log("Map constructed");
            
            if (_sceneComponent != null)
            {
                _sceneComponent.Construct(_sceneContainer.Resolve<LevelChangeObserver>());
            }

            Debug.Log("_sceneComponent constructed");
            
            if (_conversationStarters.Length > 0)
            {
                foreach (ConversationStarter starter in _conversationStarters)
                {
                    starter.Construct(_sceneContainer.Resolve<QuestManager>(), _sceneContainer.Resolve<DataPlayer>(),
                        _sceneContainer.Resolve<GameAPI>());
                }
            }
            
            Debug.Log("_conversationStarters constructed");

            if (_downloadTestData.Length > 0)
            {
                foreach (DownloadTestData data in _downloadTestData)
                {
                    data.Construct(_sceneContainer.Resolve<GameAPI>());
                }
            }
            
            Debug.Log("_downloadTestData constructed");

            if (_setStudySteps.Length > 0)
            {
                foreach (SetStudyStep step in _setStudySteps)
                {
                    step.Construct(_sceneContainer.Resolve<UIManager>());
                }
            }
            
            Debug.Log("_setStudySteps constructed");

            if (_npcInteractable.Length > 0)
            {
                foreach (NPCInteractable npc in _npcInteractable)
                {
                    npc.Construct(_sceneContainer.Resolve<CursorManager>(), _sceneContainer.Resolve<QuestManager>(),
                        _sceneContainer.Resolve<DataPlayer>(), _sceneContainer.Resolve<GameAPI>());
                }
            }
            
            Debug.Log("_npcInteractable constructed");

            if (_answerHandlers.Length > 0)
            {
                foreach (AnswerHandler answerHandler in _answerHandlers)
                {
                    answerHandler.Construct(_sceneContainer.Resolve<SaveGame>());
                }
            }
            
            Debug.Log("_answerHandlers constructed");

            if (_arrowsForPlayer.Length > 0)
            {
                foreach (ArrowForPlayer arrow in _arrowsForPlayer)
                {
                    arrow.Construct(_sceneContainer.Resolve<ArrowForPlayerManager>(),
                        _sceneContainer.Resolve<MainPlayer>().PlayerController);
                }
            }
            
            Debug.Log("_arrowsForPlayer constructed");

            if (_pickableEquip.Length > 0)
            {
                foreach (PickableEquip item in _pickableEquip)
                {
                    item.Construct(_sceneContainer.Resolve<UIManager>(),
                        _sceneContainer.Resolve<WeaponArmorManager>());
                }
            }
            
            Debug.Log("_pickableEquip constructed");

            if (_giveItems.Length > 0)
            {
                foreach (GiveItem item in _giveItems)
                {
                    item.Construct(_sceneContainer.Resolve<UIManager>());
                }
            }
            
            Debug.Log("_giveItems constructed");

            if (_checkItems.Length > 0)
            {
                foreach (CheckItem item in _checkItems)
                {
                    item.Construct(_sceneContainer.Resolve<UIManager>());
                }
            }
            
            Debug.Log("_checkItems constructed");

            // if (_bosses.Length > 0)
            // {
            //     foreach (BossNPC boss in _bosses)
            //     {
            //         boss.Construct();
            //     }
            // }

            if (_iconsForCamera.Length > 0)
            {
                foreach (IconForFarCamera icon in _iconsForCamera)
                {
                    icon.Construct(_sceneContainer.Resolve<UIManager>());
                }
            }
            
            
            Debug.Log("_iconsForCamera constructed");

            // if (_aiControllers.Length > 0)
            // {
            //     foreach (AIController aiController in _aiControllers)
            //     {
            //         aiController.Construct(_sceneContainer.Resolve<MainPlayer>().PlayerController,
            //             _sceneContainer.Resolve<MainPlayer>(), _sceneContainer.Resolve<IGame>(),
            //             _sceneContainer.Resolve<FastTestsManager>(),
            //             _sceneContainer.Resolve<QuestManager>(), _sceneContainer.Resolve<CoinManager>(),
            //             _sceneContainer.Resolve<BottleManager>());
            //     }
            // }
            
            Debug.Log("_aiControllers constructed");

            if (_npcForTestID.Length > 0)
            {
                foreach (NPC_for_testID npc in _npcForTestID)
                {
                    npc.Construct(_sceneContainer.Resolve<GameAPI>(), _sceneContainer.Resolve<DataPlayer>(),
                        _sceneContainer.Resolve<FastTestsManager>(), _sceneContainer.Resolve<SaveGame>());
                }
            }
            
            Debug.Log("_npcForTestID constructed");

            if (_savePoints.Length > 0)
            {
                foreach (SavePoint savePoint in _savePoints)
                {
                    savePoint.Construct(_sceneContainer.Resolve<DataPlayer>(),
                        _sceneContainer.Resolve<MainPlayer>().PlayerController.GetHealth(),
                        _sceneContainer.Resolve<CursorManager>(),
                        _sceneContainer.Resolve<MainPlayer>().PlayerController,
                        _sceneContainer.Resolve<GameAPI>());
                }
            }
            
            Debug.Log("_savePoints constructed");

            if (_portals.Length > 0)
            {
                foreach (Portal portal in _portals)
                {
                    portal.Construct(_sceneContainer.Resolve<MainPlayer>(), _sceneContainer.Resolve<DataPlayer>(),
                        _sceneComponent, _sceneContainer.Resolve<CursorManager>(),
                        _sceneContainer.Resolve<UIManager>(),
                        _sceneContainer.Resolve<NPCManagment>(), _sceneContainer.Resolve<SaveGame>(),
                        _sceneContainer.Resolve<CoinManager>(),
                        _sceneContainer.Resolve<LevelChangeObserver>());
                }
            }
            
            Debug.Log("_portals constructed");
            
            _enemySpawner.Construct(_sceneContainer.Resolve<MainPlayer>().PlayerController, 
                _sceneContainer.Resolve<MainPlayer>(), 
                _sceneContainer.Resolve<IGame>(), 
                _sceneContainer.Resolve<FastTestsManager>(), 
                _sceneContainer.Resolve<QuestManager>(), _sceneContainer.Resolve<CoinManager>(), _sceneContainer.Resolve<BottleManager>());
            
            Debug.Log("_enemySpawner constructed");
        }
    }
}