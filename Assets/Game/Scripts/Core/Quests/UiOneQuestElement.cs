using DialogueEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UiOneQuestElement : MonoBehaviour
{
    [SerializeField] private TMPro.TMP_Text textTitle;
    [SerializeField] private TMPro.TMP_Text textDescription;
    [SerializeField] private TMPro.TMP_Text textProcess;
    [SerializeField] private Image imgProcess;

    [Header("Button")]
    [SerializeField] private Button button;
    private RectTransform RTbutton;
    [SerializeField] private Image imgCheckNo;
    [SerializeField] private Image imgCheck;

    [Header("Award")]
    [SerializeField] private TMPro.TMP_Text textAward;

    private bool compliteWaitAward = false;
    private bool alreadyStarted = true; //?????? ?? ??? ?????
    private bool fullComplite = false; //???????? ?????????? ?????

    private float currentProcess;
    private float targetProcess;

    private QuestType questType;
    public OneQuest quest;



    RectTransform rtimgProcess;
    Vector2 sizeDeltaImgProcess;

    //??? ??????? ????? ???????
    public List<string> ListNeedStartConversations;
    //??? ????????? ????????? ???? ?????????? ???????
    private List<bool> pointSuccess;

    public QuestType QuestType { get => questType; set => questType = value; }

    public void setQuest(OneQuest _quest)
    {
        quest = _quest;
        RTbutton = button.GetComponent<RectTransform>();

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

        switch (quest.questAwardType)
        {
            case QuestAwardType.none:
                textAward.text = "";
                break;
            case QuestAwardType.money:
                textAward.text = $"{quest.awardFirstWord} {quest.countMoney} {quest.awardLastWord}";
                break;
            case QuestAwardType.item:
                textAward.text = $"{quest.awardFirstWord}  {quest.awardItem.name}";
                break;
            default:
                break;
        }

        SetUnfinushed();

        button.onClick.AddListener(OnClickBtn);
    }

    private void OnClickBtn()
    {
        if (!compliteWaitAward)
        {
            RTbutton.DOShakeAnchorPos(1f, new Vector2(10, 0), vibrato: 8, randomness: 0, snapping: false, fadeOut: false)
            .OnComplete(() => RTbutton.anchoredPosition = Vector2.zero);
            return;
        }

        if (fullComplite) return;

        switch (quest.questAwardType)
        {
            case QuestAwardType.none:
                break;
            case QuestAwardType.money:
                IGame.Instance.saveGame.Coins += quest.countMoney;
                break;
            case QuestAwardType.item:
                IGame.Instance.UIManager.uIBug.TryAddEquipToBug(quest.awardItem);
                break;
        }

        MarkQuestAsComplete();
        FadeOutAndShrinkUIElement(this.gameObject);
        fullComplite = true;
    }

    private void MarkQuestAsComplete()
    {
        var dataPlayer = IGame.Instance.dataPLayer;
        if (!dataPlayer.playerData.completedQuests.Contains(quest.name))
        {
            dataPlayer.playerData.completedQuests.Add(quest.name);
            StartCoroutine(IGame.Instance.gameAPI.SaveGameData());
        }
    }


    private void CheckUpdateAndComplite()
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
                updateProcess(currentProcess, targetProcess);
                break;
        }


        if (currentProcess >= targetProcess)
        {
            SetFinished();
        }
        //TODO ??????? ??????????
    }

    public void addOneProcess()
    {
        if (!alreadyStarted) return;
        currentProcess++;
        CheckUpdateAndComplite();
    }
    public void startedConversation(ConversationStarter conversationStarter)
    {
        if (!alreadyStarted) return;

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
        CheckUpdateAndComplite();
    }

    private void updateProcess(float c, float t)
    {
        textProcess.text = c.ToString("F0") + $"/" + t.ToString("F0");
        sizeDeltaImgProcess.x = Mathf.Min(c / t * 900, 900);
        rtimgProcess.sizeDelta = sizeDeltaImgProcess;
    }
    private void SetFinished()
    {
        compliteWaitAward = true;
        button.GetComponent<Image>().color = new Color(0.81f, 0.952f, 0.768f);
        imgCheckNo.gameObject.SetActive(false);
        imgCheck.gameObject.SetActive(true);
    }

    private void SetUnfinushed()
    {
        compliteWaitAward = false;
        button.GetComponent<Image>().color = new Color(0.9529412f, 0.8073038f, 0.7686275f);
        imgCheckNo.gameObject.SetActive(true);
        imgCheck.gameObject.SetActive(false);
        fullComplite = false;
    }

    public void FadeOutAndShrinkUIElement(GameObject uiElement)
    {
        VerticalLayoutGroup vertLGrroup = IGame.Instance.UIManager.QuestsContentScrollRect.content.GetComponent<VerticalLayoutGroup>();

        CanvasGroup canvasGroup = uiElement.AddComponent<CanvasGroup>();
        RectTransform rectTransform = uiElement.GetComponent<RectTransform>();

        Sequence sequence = DOTween.Sequence();

        sequence.Append(canvasGroup.DOFade(0, 1f));
        sequence.Join(rectTransform.DOSizeDelta(new Vector2(rectTransform.sizeDelta.x, 0), 1f));

        sequence.OnComplete(() =>
        {
            uiElement.SetActive(false);
        });

        sequence.OnUpdate(()=>
        {
            vertLGrroup.SetLayoutVertical();
        });
    }


}
