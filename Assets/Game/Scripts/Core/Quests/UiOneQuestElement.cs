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

    public QuestType QuestType { get => questType; set => questType = value; }

    public void setQuest(OneQuest quest)
    {
        textTitle.text = quest.questTitle;
        textDescription.text = quest.questDescription;

        QuestType = quest.questType;
        targetProcess = quest.questTargetCount;

        rtimgProcess = imgProcess.GetComponent<RectTransform>();
        sizeDeltaImgProcess = rtimgProcess.sizeDelta;
    }

    private void CheckUpdate()
    {
        switch (QuestType)
        {
            case QuestType.killEnemy:
                updateProcess(currentProcess, targetProcess);
                break;
            case QuestType.VisitPoints:
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

    private void updateProcess(float c, float t)
    {
        textProcess.text = c.ToString("F0") + $"/" + t.ToString("F0");
        sizeDeltaImgProcess.x = Mathf.Min(c / t * 900, 900);
        rtimgProcess.sizeDelta = sizeDeltaImgProcess;
    }
}
