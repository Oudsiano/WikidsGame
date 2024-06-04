using DialogueEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiOneQuestElement : MonoBehaviour
{
    [SerializeField] private TMPro.TMP_Text textTitle;
    [SerializeField] private TMPro.TMP_Text textDescription;
    [SerializeField] private TMPro.TMP_Text textProcess;
    [SerializeField] private Image imgProcess;
    [SerializeField] private Image imgCheck;

    private float currentProcess;
    private float targetProcess;

    private QuestType questType;

    RectTransform rtimgProcess;
    Vector2 sizeDeltaImgProcess;

    //ƒл€ начатых бесед услови€
    public List<string> ListNeedStartConversations;
    //дл€ различных элементов лист выполнени€ услови€
    private List<bool> pointSuccess;

    public QuestType QuestType { get => questType; set => questType = value; }

    public void setQuest(OneQuest quest)
    {
        textTitle.text = quest.questTitle;
        textDescription.text = quest.questDescription;

        QuestType = quest.questType;
        targetProcess = quest.questTargetCount;

        rtimgProcess = imgProcess.GetComponent<RectTransform>();
        sizeDeltaImgProcess = rtimgProcess.sizeDelta;

        if (quest.questType == QuestType.toSpeekNPC)
        {
            targetProcess = quest.ListNeedConversationsStarter.Count;
            pointSuccess = new List<bool>();
            ListNeedStartConversations = new List<string>();
            for (int i = 0; i < quest.ListNeedConversationsStarter.Count; i++)
            {
                pointSuccess.Add(false);
                ListNeedStartConversations.Add(quest.ListNeedConversationsStarter[i]);
            }
        }
    }

    private void CheckUpdate()
    {
        switch (QuestType)
        {
            case QuestType.killEnemy:
                updateProcess(currentProcess, targetProcess);
                break;
            case QuestType.toSpeekNPC:
                updateProcess(currentProcess, targetProcess);
                break;
            default:
                break;
        }
    }



    public void addOneProcess()
    {
        currentProcess++;
        CheckUpdate();

        //TODO чекнуть завершение
    }
    public void startedConversation(ConversationStarter conversationStarter)
    {
        for (int i = 0; i < ListNeedStartConversations.Count; i++)
        {
            if (ListNeedStartConversations[i] == conversationStarter.name)
            {
                if (pointSuccess[i]) return;
                else
                {
                    pointSuccess[i] = true;
                }
            }
        }
        currentProcess = 0;
        foreach (var item in pointSuccess)
        {
            if (item) currentProcess++;
        }
        CheckUpdate();
    }

    private void updateProcess(float c, float t)
    {
        textProcess.text = c.ToString("F0") + $"/" + t.ToString("F0");
        sizeDeltaImgProcess.x = Mathf.Min(c / t * 900, 900);
        rtimgProcess.sizeDelta = sizeDeltaImgProcess;
    }
}
