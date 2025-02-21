using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Core.Quests
{
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

        public OneQuestData thisQuestData;
        public OneQuest quest;

        RectTransform rtimgProcess;
        Vector2 sizeDeltaImgProcess;

        public List<string> ListNeedStartConversations;
        private List<bool> pointSuccess;

        public QuestType QuestType { get => thisQuestData.questType; set => thisQuestData.questType = value; }

        public void setQuest(OneQuest _quest)
        {
            quest = _quest;
            RTbutton = button.GetComponent<RectTransform>();

            textTitle.text = quest.questTitle;
            textDescription.text = quest.questDescription;

            QuestType = quest.questType;
            thisQuestData.targetProcess = quest.questTargetCount;
            if (quest.questType == QuestType.completeSpecialTest)
            {
                thisQuestData.targetProcess = quest.IdTests.Count;
            }

            rtimgProcess = imgProcess.GetComponent<RectTransform>();
            sizeDeltaImgProcess = rtimgProcess.sizeDelta;

            if (quest.questType == QuestType.toSpeekNPC)
            {
                thisQuestData.targetProcess = quest.ListNeedConversationsStarter.Count;
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
            SetUnfinished();

            button.onClick.AddListener(OnClickBtn);

            if (IGame.Instance.dataPlayer.playerData.startedQuests != null && IGame.Instance.dataPlayer.playerData.startedQuests.ContainsKey(quest.name))
            {
                thisQuestData = IGame.Instance.dataPlayer.playerData.startedQuests[quest.name];
                CheckUpdateAndComplete(false);
            }
        }

        private void OnClickBtn()
        {
            if (!thisQuestData.compliteWaitAward)
            {
                RTbutton.DOShakeAnchorPos(1f, new Vector2(10, 0), vibrato: 8, randomness: 0, snapping: false, fadeOut: false)
                    .OnComplete(() => RTbutton.anchoredPosition = Vector2.zero);
                return;
            }

            if (thisQuestData.fullComplite) return;

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
            thisQuestData.fullComplite = true;
            IGame.Instance.UIManager.UpdateQuestBackImg();
        }

        private void MarkQuestAsComplete()
        {
            var dataPlayer = IGame.Instance.dataPlayer;
            if (!dataPlayer.playerData.completedQuests.Contains(quest.name))
            {
                dataPlayer.playerData.completedQuests.Add(quest.name);
                IGame.Instance.gameAPI.SaveUpdater();
            }
        }

        internal void CheckTestCount()
        {
            CheckUpdateAndComplete(false);
        }

        private void CheckUpdateAndComplete(bool withSave = true)
        {
            if (withSave)
            {
                if (IGame.Instance.dataPlayer.playerData.startedQuests == null)
                    IGame.Instance.dataPlayer.playerData.startedQuests = new Dictionary<string, OneQuestData>();
                thisQuestData.QuestName = quest.name;
                IGame.Instance.dataPlayer.playerData.startedQuests[quest.name] = thisQuestData;
                IGame.Instance.gameAPI.SaveUpdater();
            }

            switch (QuestType)
            {
                case QuestType.killEnemy:
                    updateProcess(thisQuestData.currentProcess, thisQuestData.targetProcess);
                    break;
                case QuestType.toSpeekNPC:
                    updateProcess(thisQuestData.currentProcess, thisQuestData.targetProcess);
                    break;
                case QuestType.completeSpecialTest:
                    thisQuestData.currentProcess = 0;
                    foreach (string itemId in quest.IdTests)
                    {
                        int testId;
                        if (int.TryParse(itemId, out testId))
                        {
                            if (IGame.Instance.dataPlayer.isTestComplete(testId))
                            {
                                thisQuestData.currentProcess++;
                            }
                        }
                        else
                        {
                            Debug.LogError("?????? ? ???????????? ??????");
                        }
                    }
                    updateProcess(thisQuestData.currentProcess, thisQuestData.targetProcess);
                    break;
                default:
                    updateProcess(thisQuestData.currentProcess, thisQuestData.targetProcess);
                    break;
            }

            if (thisQuestData.currentProcess >= thisQuestData.targetProcess)
            {
                SetFinished();
                IGame.Instance.UIManager.UpdateQuestBackImg();
            }
        }

        public void setProgress(int count)
        {
            if (!thisQuestData.alreadyStarted) return;
            thisQuestData.currentProcess = count;
            CheckUpdateAndComplete();
        }

        public void addOneProcess()
        {
            if (!thisQuestData.alreadyStarted) return;
            thisQuestData.currentProcess++;
            CheckUpdateAndComplete();
        }

        public void startedConversation(ConversationStarter conversationStarter)
        {
            if (!thisQuestData.alreadyStarted) return;

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

            foreach (var item in pointSuccess)
            {
                if (item) thisQuestData.currentProcess++;
            }
            CheckUpdateAndComplete();
        }

        private void updateProcess(float c, float t)
        {
            textProcess.text = c.ToString("F0") + $"/" + t.ToString("F0");
            sizeDeltaImgProcess.x = Mathf.Min(c / t * 900, 900);
            rtimgProcess.sizeDelta = sizeDeltaImgProcess;
        }

        private void SetFinished()
        {
            thisQuestData.compliteWaitAward = true;
            button.GetComponent<Image>().color = new Color(0.81f, 0.952f, 0.768f);
            imgCheckNo.gameObject.SetActive(false);
            imgCheck.gameObject.SetActive(true);
        }

        private void SetUnfinished()
        {
            thisQuestData.compliteWaitAward = false;
            button.GetComponent<Image>().color = new Color(0.9529412f, 0.8073038f, 0.7686275f);
            imgCheckNo.gameObject.SetActive(true);
            imgCheck.gameObject.SetActive(false);
            thisQuestData.fullComplite = false;
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

            sequence.OnUpdate(() =>
            {
                vertLGrroup.SetLayoutVertical();
            });
        }
    }
}
