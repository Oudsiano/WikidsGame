using FarrokhGames.Inventory.Examples;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum QuestType
{
    killEnemy,
    toSpeekNPC,
}

public class QuestManager : MonoBehaviour
{
    public List<OneQuest> thisQuestsScene;
    public List<ItemDefinition> allQuestsItems;
    private List<UiOneQuestElement> QuestsInScene;

    private QuestsForThisScene _QuestsForThisScene;

    public static event Action KillEnemy;

    public void Init()
    {
        QuestsInScene = new List<UiOneQuestElement>();
        SceneManager.sceneLoaded += SceneLoader_LevelChanged;
    }

    private void SceneLoader_LevelChanged(Scene scene, LoadSceneMode mode)
    {
        QuestsInScene = new List<UiOneQuestElement>();
        _QuestsForThisScene = FindObjectOfType<QuestsForThisScene>();
        thisQuestsScene = new List<OneQuest>();

        if (_QuestsForThisScene != null)
        {
            thisQuestsScene = new List<OneQuest>(_QuestsForThisScene.QuestsThisScene);
        }

        if (IGame.Instance.UIManager.QuestsContentScrollRect != null && IGame.Instance.UIManager.QuestsContentScrollRect.content != null)
        {
            foreach (Transform child in IGame.Instance.UIManager.QuestsContentScrollRect.content)
            {
                if (child.name != "EmptySpace")
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
        }
    }

    public void newKill()
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
