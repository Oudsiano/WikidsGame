using FarrokhGames.Inventory.Examples;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
[System.Serializable]
public enum QuestType
{
    killEnemy,
    toSpeekNPC,
    killSpecialEnemy,
    completeSpecialTest,

}

public enum QuestAwardType
{
    none,
    money,
    item,
}

public class QuestManager : MonoBehaviour
{
    public List<OneQuest> thisQuestsScene;
    public List<ItemDefinition> allQuestsItems;
    private List<UiOneQuestElement> QuestsInScene;

    private QuestsForThisScene _QuestsForThisScene;
    private AllQuestsInGame _AllQuestsInGame;
    private SceneWithTestsID sceneWithTestsID;

    private List<int> needRepeatTestQuests = new List<int>();

    //public static event Action KillEnemy;
    private bool alreadyDelegated;

    public void Awake()
    {
        //GenListQuests();
        //SceneManager.sceneLoaded += SceneLoader_LevelChanged;
    }
    private void OnDestroy()
    {
        //SceneManager.sceneLoaded -= SceneLoader_LevelChanged;
    }

    public void Init()
    {
        SceneManager.sceneLoaded += SceneLoader_LevelChanged;
        QuestsInScene = new List<UiOneQuestElement>();
    }

    private void SceneLoader_LevelChanged(Scene scene, LoadSceneMode mode)
    {
        GenListQuests();
    }

    public bool ShowBackImgForBtn()
    {
        foreach (var item in QuestsInScene)
        {
            if (item.thisQuestData.compliteWaitAward)
                if (!item.thisQuestData.fullComplite)
                return true;
        }

            return false;
    }

    private void GenListQuests()
    {
        QuestsInScene = new List<UiOneQuestElement>();
        //_QuestsForThisScene = FindObjectOfType<QuestsForThisScene>();

        _AllQuestsInGame = FindObjectOfType<AllQuestsInGame>();
        sceneWithTestsID = FindObjectOfType<SceneWithTestsID>();

        thisQuestsScene = new List<OneQuest>();

        if (_AllQuestsInGame != null)
        {
            foreach (OneSceneListQuests OneList in _AllQuestsInGame.AllQuests)
            {
                if (OneList.SceneId == IGame.Instance.LevelChangeObserver.GetCuurentSceneId())
                {
                    foreach (var quest in OneList.QuestsThisScene)
                    {
                        if (IGame.Instance.dataPLayer.playerData.completedQuests == null) continue;
                        if (IGame.Instance.dataPLayer.playerData.completedQuests.Contains(quest.name)) continue;
                        thisQuestsScene.Add(quest);
                    }
                }
            }

            foreach (int testID in needRepeatTestQuests)
            {
                foreach (var OneSceneWithTestID in sceneWithTestsID.sceneDataList)
                {
                    if (OneSceneWithTestID.numbers.Contains(testID))
                    {
                        //We need start quest with testID in scene OneSceneWithTestID.scene

                        //TODO: There we should prepare list for visible NPC with tests
                    }
                }
            }
        }


        /*if (_QuestsForThisScene != null)
        {
            foreach (var quest in _QuestsForThisScene.QuestsThisScene)
            {
                if (IGame.Instance.dataPLayer.playerData.completedQuests == null) continue;
                if (IGame.Instance.dataPLayer.playerData.completedQuests.Contains(quest.name)) continue;
                thisQuestsScene.Add(quest);
            }
        }*/

        if (IGame.Instance.UIManager.QuestsContentScrollRect != null && IGame.Instance.UIManager.QuestsContentScrollRect.content != null)
        {
            GameObject NotExitQuests = new GameObject();
            foreach (Transform child in IGame.Instance.UIManager.QuestsContentScrollRect.content)
            {
                if (child.name == "NotExitQuests")
                    NotExitQuests = child.gameObject;
                if ((child.name != "EmptySpace") && (child.name != "NotExitQuests"))
                {
                    Destroy(child.gameObject);
                }
            }

            foreach (OneQuest quest in thisQuestsScene)
            {
                GameObject newQuest = Instantiate(IGame.Instance.UIManager.OneQuestPref, IGame.Instance.UIManager.QuestsContentScrollRect.content);

                var questElement = newQuest.GetComponent<UiOneQuestElement>();
                if (questElement != null)
                {
                    questElement.setQuest(quest);
                    QuestsInScene.Add(questElement);
                }
                else
                {
                    Debug.LogError("UiOneQuestElement component is missing on newQuest object");
                }
            }

            if (QuestsInScene.Count == 0)
            {
                NotExitQuests.SetActive(true);
                IGame.Instance.UIManager.UpdateGreyBtnQuest(true);
            }
            else
            {
                NotExitQuests.SetActive(false);
                IGame.Instance.UIManager.UpdateGreyBtnQuest(false);
            }

        }
        IGame.Instance.UIManager.UpdateQuestBackImg();
    }


    public void StartNewQuest(OneQuest quest)
    {
        GameObject newQuest = Instantiate(IGame.Instance.UIManager.OneQuestPref, IGame.Instance.UIManager.QuestsContentScrollRect.content);

        var questElement = newQuest.GetComponent<UiOneQuestElement>();
        if (questElement != null)
        {
            questElement.setQuest(quest);
            QuestsInScene.Add(questElement);
        }
    }
    public void StartNewQuestIfNot(OneQuest quest)
    {
        //if quest not exist before
        foreach (var item in QuestsInScene)
        {
            if (item.quest.name == quest.name)
            {
                return;
            }
        }
        StartNewQuest(quest);
    }

    public void questFinished(string ID)
    {
        foreach (UiOneQuestElement item in QuestsInScene)
        {
            if (item.QuestType == QuestType.completeSpecialTest)
            {
                item.CheckTestCount();
            }
        }
    }

    public void newKill(string name = null)
    {
        if (QuestsInScene == null)
        {
            Debug.LogError("QuestsInScene is null in newKill");
            return;
        }

        foreach (UiOneQuestElement item in QuestsInScene)
        {
            if (item == null)
            {
                Debug.LogError("UiOneQuestElement item is null in QuestsInScene");
                continue;
            }

            if (item.QuestType == QuestType.killEnemy)
            {
                item.addOneProcess();
            }

            if (item.QuestType == QuestType.killSpecialEnemy)
                if (item.quest.specialEnemyName == name)
                {
                    item.addOneProcess();
                }
        }
    }

    public void startedConversation(ConversationStarter conversationStarter)
    {
        if (conversationStarter == null)
        {
            Debug.LogError("conversationStarter is null in startedConversation");
            return;
        }

        if (QuestsInScene == null)
        {
            Debug.LogError("QuestsInScene is null in startedConversation");
            return;
        }

        foreach (UiOneQuestElement item in QuestsInScene)
        {
            if (item == null)
            {
                Debug.LogError("UiOneQuestElement item is null in QuestsInScene");
                continue;
            }

            if (item.QuestType == QuestType.toSpeekNPC)
            {
                item.startedConversation(conversationStarter);
            }
        }
    }
}
