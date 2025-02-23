using System.Collections.Generic;
using Core.Quests.Data;
using Core.Quests.QuestsEnums;
using Data;
using SceneManagement;
using UI;
using UI.Inventory;
using UI.Inventory.Data;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

namespace Core.Quests
{
    public class QuestManager : MonoBehaviour // TODO refactor 
    {
        [FormerlySerializedAs("thisQuestsScene")] [SerializeField]
        private List<OneQuest> _questsScene;

        [FormerlySerializedAs("allQuestsItems")] [SerializeField]
        private List<ItemDefinition> _allQuestsItems;

        private List<UiOneQuestElement> _questsInScene;
        private QuestsForThisScene _questsForThisScene;
        private AllQuestsInGame _allQuestsInGame;
        private SceneWithTestsID _sceneWithTestsID;

        private List<int> _needRepeatTestQuests = new();

        private bool _alreadyDelegated;
        private UIManager _uiManager;
        private LevelChangeObserver _levelChangeObserver;
        private DataPlayer _dataPlayer;

        public List<ItemDefinition> AllQuestsItems => _allQuestsItems;

        public void Construct(DataPlayer dataPlayer, UIManager uiManager,
            LevelChangeObserver levelChangeObserver, AllQuestsInGame allQuestsInGame,
            SceneWithTestsID sceneWithTestsID)
        {
            _dataPlayer = dataPlayer;
            _uiManager = uiManager;
            _levelChangeObserver = levelChangeObserver;
            _allQuestsInGame = allQuestsInGame;
            _sceneWithTestsID = sceneWithTestsID;
            
            SceneManager.sceneLoaded += SceneLoader_LevelChanged;
            _questsInScene = new List<UiOneQuestElement>();
        }

        public bool ShowBackImgForBtn()
        {
            foreach (var item in _questsInScene)
            {
                if (item.QuestData.CompleteWaitAward)
                {
                    if (!item.QuestData.FullComplete)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public void StartNewQuest(OneQuest quest)
        {
            GameObject newQuest = Instantiate(_uiManager.OneQuestPref,
                _uiManager.QuestsContentScrollRect.content);

            var questElement = newQuest.GetComponent<UiOneQuestElement>();

            if (questElement != null)
            {
                questElement.SetQuest(quest);
                _questsInScene.Add(questElement);
            }
        }

        public void StartNewQuestIfNot(OneQuest quest)
        {
            foreach (var item in _questsInScene)
            {
                if (item.Quest.name == quest.name)
                {
                    return;
                }
            }

            StartNewQuest(quest);
        }

        public void QuestFinished(string ID) // TODO not used ARG // TODO Rename
        {
            foreach (UiOneQuestElement item in _questsInScene)
            {
                if (item.QuestType == QuestType.completeSpecialTest)
                {
                    item.CheckTestCount();
                }
            }
        }

        public void NewKill(string name = null) // TODO Rename
        {
            if (_questsInScene == null)
            {
                Debug.LogError("QuestsInScene is null in newKill");
                return;
            }

            foreach (UiOneQuestElement item in _questsInScene)
            {
                if (item == null)
                {
                    Debug.LogError("UiOneQuestElement item is null in QuestsInScene");
                    continue;
                }

                if (item.QuestType == QuestType.killEnemy)
                {
                    item.AddOneProcess();
                }

                if (item.QuestType == QuestType.killSpecialEnemy && item.Quest.specialEnemyName == name)
                {
                    item.AddOneProcess();
                }
            }
        }

        public void StartedConversation(ConversationStarter conversationStarter) // TODO Rename
        {
            if (conversationStarter == null)
            {
                Debug.LogError("conversationStarter is null in startedConversation");
                return;
            }

            if (_questsInScene == null)
            {
                Debug.LogError("QuestsInScene is null in startedConversation");
                return;
            }

            foreach (UiOneQuestElement item in _questsInScene)
            {
                if (item == null)
                {
                    Debug.LogError("UiOneQuestElement item is null in QuestsInScene");
                    continue;
                }

                if (item.QuestType == QuestType.toSpeekNPC)
                {
                    item.StartedConversation(conversationStarter);
                }
            }
        }

        private void SceneLoader_LevelChanged(Scene scene, LoadSceneMode mode)
        {
            GenerateListQuests();
        }

        private void GenerateListQuests() // TODO refactor overload method
        {
            _questsInScene = new List<UiOneQuestElement>();

            _questsScene = new List<OneQuest>();

            if (_allQuestsInGame != null)
            {
                foreach (OneSceneListQuests OneList in _allQuestsInGame.Quests)
                {
                    if (OneList.SceneId == _levelChangeObserver.GetCurrentSceneId())
                    {
                        foreach (var quest in OneList.QuestsThisScene)
                        {
                            if (_dataPlayer.PlayerData.completedQuests == null)
                            {
                                continue;
                            }

                            if (_dataPlayer.PlayerData.completedQuests.Contains(quest.name))
                            {
                                continue;
                            }

                            _questsScene.Add(quest);
                        }
                    }
                }

                foreach (int testID in _needRepeatTestQuests)
                {
                    foreach (var sceneData in _sceneWithTestsID.SceneDataList)
                    {
                        if (sceneData.numbers.Contains(testID))
                        {
                            //We need start quest with testID in scene OneSceneWithTestID.scene

                            //TODO: There we should prepare list for visible NPC with tests
                        }
                    }
                }
            }

            if (_uiManager.QuestsContentScrollRect != null &&
                _uiManager.QuestsContentScrollRect.content != null)
            {
                GameObject NotExitQuests = new GameObject();

                foreach (Transform child in _uiManager.QuestsContentScrollRect.content)
                {
                    if (child.name == "NotExitQuests") // TODO change and can be cached
                    {
                        NotExitQuests = child.gameObject;
                    }

                    if (child.name != "EmptySpace" && child.name != "NotExitQuests") // TODO change and can be cached
                    {
                        Destroy(child.gameObject);
                    }
                }

                foreach (OneQuest quest in _questsScene)
                {
                    GameObject newQuest = Instantiate(
                        _uiManager.OneQuestPref, // TODO Create factory for creating quest
                        _uiManager.QuestsContentScrollRect.content);

                    var questElement = newQuest.GetComponent<UiOneQuestElement>();

                    if (questElement != null)
                    {
                        questElement.SetQuest(quest);
                        _questsInScene.Add(questElement);
                    }
                    else
                    {
                        Debug.LogError("UiOneQuestElement component is missing on newQuest object");
                    }
                }

                if (_questsInScene.Count == 0)
                {
                    NotExitQuests.SetActive(true);
                    _uiManager.UpdateGreyBtnQuest(true);
                }
                else
                {
                    NotExitQuests.SetActive(false);
                    _uiManager.UpdateGreyBtnQuest(false);
                }
            }

            _uiManager.UpdateQuestBackImg();
        }
    }
}