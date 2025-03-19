using System.Collections.Generic;
using Core.Quests.Data;
using Core.Quests.QuestsEnums;
using Data;
using Saving;
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
        private const string NOTEXITQUESTS_NAME = "NotExitQuests";
        private const string EMPTYSPACE_NAME = "EmptySpace";

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
        private SaveGame _saveGame;
        private GameAPI _gameAPI;

        public List<ItemDefinition> AllQuestsItems => _allQuestsItems;

        public void Construct(DataPlayer dataPlayer, UIManager uiManager,
            LevelChangeObserver levelChangeObserver, AllQuestsInGame allQuestsInGame,
            SceneWithTestsID sceneWithTestsID, SaveGame saveGame, GameAPI gameAPI)
        {
            _dataPlayer = dataPlayer;
            _uiManager = uiManager;
            _levelChangeObserver = levelChangeObserver;
            _allQuestsInGame = allQuestsInGame;
            _sceneWithTestsID = sceneWithTestsID;
            _saveGame = saveGame;
            _gameAPI = gameAPI;

            SceneManager.sceneLoaded += SceneLoader_LevelChanged;
            _questsInScene = new List<UiOneQuestElement>();
        }

        public bool ShowBackImgForBtn()
        {
            foreach (var item in _questsInScene)
            {
                if (item.QuestData.CompleteWaitAward)
                {
                    if (item.QuestData.FullComplete == false)
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
                _uiManager.QuestsContentScrollRect.content); // TODO factory creator

            var questElement = newQuest.GetComponent<UiOneQuestElement>();

            if (questElement != null)
            {
                questElement.Construct(_dataPlayer, _saveGame, _uiManager, _gameAPI);
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

        public void CompleteQuest(string _)
        {
            foreach (var item in _questsInScene)
            {
                if (item.QuestType == QuestType.completeSpecialTest)
                {
                    Debug.Log("CompleteQuest " + item.QuestType);
                    
                    item.CheckTestCount();
                }
            }
        }

        public void KillNew(string _ = null)
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

        public void SetupConversation(ConversationStarter conversationStarter)
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
                    item.SetupConversation(conversationStarter);
                }
            }
        }

        private void SceneLoader_LevelChanged(Scene scene, LoadSceneMode mode)
        {
            GenerateListQuests();
        }

        private void GenerateListQuests()
        {
            _questsInScene = new List<UiOneQuestElement>();
            _questsScene = new List<OneQuest>();

            LoadQuestsForCurrentScene();
            AddRepeatTestQuests();
            ClearQuestUI();
            PopulateQuestUI();
            UpdateQuestUIState();
        }

        private void LoadQuestsForCurrentScene()
        {
            if (_allQuestsInGame == null)
            {
                return;
            }

            foreach (OneSceneListQuests oneList in _allQuestsInGame.Quests)
            {
                if (oneList.SceneId == SceneManager.GetActiveScene().name)
                {
                    foreach (var quest in oneList.QuestsThisScene)
                    {
                        if (_dataPlayer.PlayerData.completedQuests == null ||
                            _dataPlayer.PlayerData.completedQuests.Contains(quest.name))
                        {
                            continue;
                        }

                        _questsScene.Add(quest);
                    }
                }
            }
        }

        private void AddRepeatTestQuests()
        {
            foreach (int testID in _needRepeatTestQuests)
            {
                foreach (var sceneData in _sceneWithTestsID.SceneDataList)
                {
                    if (sceneData.numbers.Contains(testID))
                    {
                        // TODO: Prepare list for visible NPC with tests
                    }
                }
            }
        }

        private void ClearQuestUI()
        {
            if (_uiManager.QuestsContentScrollRect == null ||
                _uiManager.QuestsContentScrollRect.content == null)
            {
                return;
            }

            foreach (Transform child in _uiManager.QuestsContentScrollRect.content)
            {
                if (child.name != EMPTYSPACE_NAME && child.name != NOTEXITQUESTS_NAME) // TODO: Optimize caching
                {
                    Destroy(child.gameObject);
                }
            }
        }

        private void PopulateQuestUI()
        {
            if (_uiManager.QuestsContentScrollRect == null ||
                _uiManager.QuestsContentScrollRect.content == null)
            {
                return;
            }

            foreach (OneQuest quest in _questsScene)
            {
                GameObject newQuest = Instantiate(
                    _uiManager.OneQuestPref,
                    _uiManager.QuestsContentScrollRect.content);

                var questElement = newQuest.GetComponent<UiOneQuestElement>();

                if (questElement != null)
                {
                    questElement.Construct(_dataPlayer, _saveGame, _uiManager, _gameAPI);
                    questElement.SetQuest(quest);
                    _questsInScene.Add(questElement);
                }
                else
                {
                    Debug.LogError("UiOneQuestElement component is missing on newQuest object");
                }
            }
        }

        private void UpdateQuestUIState()
        {
            GameObject notExitQuests = _uiManager.QuestsContentScrollRect.content.Find(NOTEXITQUESTS_NAME)?.gameObject;

            if (notExitQuests != null)
            {
                bool noQuests = _questsInScene.Count == 0;
                notExitQuests.SetActive(noQuests);
                _uiManager.UpdateGreyBtnQuest(noQuests);
            }

            _uiManager.UpdateQuestBackImg();
        }
    }
}