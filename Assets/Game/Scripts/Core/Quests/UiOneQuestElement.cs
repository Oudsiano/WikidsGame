using System.Collections.Generic;
using Core.Quests.Data;
using Core.Quests.QuestsEnums;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Serialization;

namespace Core.Quests
{
    public class UiOneQuestElement : MonoBehaviour // TODO move to UI
    {
        [FormerlySerializedAs("textAward")] [Header("Award")] [SerializeField]
        private TMP_Text _textAward;

        [FormerlySerializedAs("textTitle")] [SerializeField]
        private TMP_Text _textTitle;

        [FormerlySerializedAs("textDescription")] [SerializeField]
        private TMP_Text _textDescription;

        [FormerlySerializedAs("textProcess")] [SerializeField]
        private TMP_Text _textProcess;

        [FormerlySerializedAs("imgProcess")] [SerializeField]
        private Image _imgProcess;

        [FormerlySerializedAs("button")] [Header("Button")] [SerializeField]
        private Button _button;

        [FormerlySerializedAs("imgCheckNo")] [SerializeField]
        private Image _imageCheckNo;

        [FormerlySerializedAs("imgCheck")] [SerializeField]
        private Image _imageCheck;

        [FormerlySerializedAs("thisQuestData")] [SerializeField]
        public OneQuestData QuestData;

        [FormerlySerializedAs("quest")] public OneQuest Quest;

        [FormerlySerializedAs("rtimgProcess")] public RectTransform RectTransformProcess;

        [FormerlySerializedAs("sizeDeltaImgProcess")]
        public Vector2 SizeDeltaImageProcess;

        public List<string> ListNeedStartConversations;

        private RectTransform _rectTransformButton;
        private List<bool> _pointSuccess;

        public QuestType QuestType
        {
            get => QuestData.QuestType;
            set => QuestData.QuestType = value;
        }

        public void SetQuest(OneQuest quest) // TODO overload method
        {
            Quest = quest;
            _rectTransformButton = _button.GetComponent<RectTransform>(); // TODO getcomp

            _textTitle.text = Quest.questTitle;
            _textDescription.text = Quest.questDescription;

            QuestType = Quest.questType;
            QuestData.TargetProcess = Quest.questTargetCount;

            if (Quest.questType == QuestType.completeSpecialTest)
            {
                QuestData.TargetProcess = Quest.IdTests.Count;
            }

            RectTransformProcess = _imgProcess.GetComponent<RectTransform>(); // TODO getcomp
            SizeDeltaImageProcess = RectTransformProcess.sizeDelta;

            if (Quest.questType == QuestType.toSpeekNPC)
            {
                QuestData.TargetProcess = Quest.ListNeedConversationsStarter.Count;
                _pointSuccess = new List<bool>();
                ListNeedStartConversations = new List<string>();

                for (int i = 0; i < Quest.ListNeedConversationsStarter.Count; i++)
                {
                    _pointSuccess.Add(false);
                    ListNeedStartConversations.Add(Quest.ListNeedConversationsStarter[i]);
                }
            }

            switch (Quest.questAwardType)
            {
                case QuestAwardType.none:
                    _textAward.text = ""; // TODO can be cached
                    break;
                case QuestAwardType.money:
                    _textAward.text =
                        $"{Quest.awardFirstWord} {Quest.countMoney} {Quest.awardLastWord}"; // TODO can be cached
                    break;
                case QuestAwardType.item:
                    _textAward.text = $"{Quest.awardFirstWord}  {Quest.awardItem.name}"; // TODO can be cached
                    break;
            }

            SetUnfinished();
            _button.onClick.AddListener(OnClickButton);

            if (IGame.Instance.dataPlayer.playerData.startedQuests != null &&
                IGame.Instance.dataPlayer.playerData.startedQuests.ContainsKey(Quest.name))
            {
                QuestData = IGame.Instance.dataPlayer.playerData.startedQuests[Quest.name];
                CheckUpdateAndComplete(false);
            }
        }

        public void SetProgress(int count)
        {
            if (QuestData.AlreadyStarted == false)
            {
                return;
            }

            QuestData.CurrentProcess = count;
            CheckUpdateAndComplete();
        }

        public void AddOneProcess()
        {
            if (QuestData.AlreadyStarted == false)
            {
                return;
            }

            QuestData.CurrentProcess++;
            CheckUpdateAndComplete();
        }

        public void StartedConversation(ConversationStarter conversationStarter)
        {
            if (QuestData.AlreadyStarted == false)
            {
                return;
            }

            for (int i = 0; i < ListNeedStartConversations.Count; i++)
            {
                if (ListNeedStartConversations[i] == conversationStarter.name)
                {
                    if (_pointSuccess[i])
                    {
                        return;
                    }
                    else // TODO not used code
                    {
                        _pointSuccess[i] = true;
                    }
                }
            }

            foreach (var item in _pointSuccess)
            {
                if (item)
                {
                    QuestData.CurrentProcess++;
                }
            }

            CheckUpdateAndComplete();
        }
        
        private void OnClickButton() // TODO magic numbers
        {
            if (QuestData.CompleteWaitAward == false)
            {
                _rectTransformButton.DOShakeAnchorPos(1f, new Vector2(10, 0), vibrato: 8, randomness: 0,
                        snapping: false, fadeOut: false)
                    .OnComplete(() => _rectTransformButton.anchoredPosition = Vector2.zero);

                return;
            }

            if (QuestData.FullComplete)
            {
                return;
            }

            switch (Quest.questAwardType)
            {
                case QuestAwardType.none:
                    break;
                case QuestAwardType.money:
                    IGame.Instance.saveGame.Coins += Quest.countMoney;
                    break;
                case QuestAwardType.item:
                    IGame.Instance.UIManager.uIBug.TryAddEquipToBug(Quest.awardItem);
                    break;
            }

            MarkQuestAsComplete();
            FadeOutAndShrinkUIElement(gameObject);
            QuestData.FullComplete = true;
            IGame.Instance.UIManager.UpdateQuestBackImg();
        }

        private void MarkQuestAsComplete()
        {
            var dataPlayer = IGame.Instance.dataPlayer;

            if (dataPlayer.playerData.completedQuests.Contains(Quest.name) == false)
            {
                dataPlayer.playerData.completedQuests.Add(Quest.name);
                IGame.Instance.gameAPI.SaveUpdater();
            }
        }

        private void CheckUpdateAndComplete(bool withSave = true) // TODO overload method
        {
            if (withSave)
            {
                if (IGame.Instance.dataPlayer.playerData.startedQuests == null)
                {
                    IGame.Instance.dataPlayer.playerData.startedQuests = new Dictionary<string, OneQuestData>();
                }

                QuestData.QuestName = Quest.name;
                IGame.Instance.dataPlayer.playerData.startedQuests[Quest.name] = QuestData;
                IGame.Instance.gameAPI.SaveUpdater();
            }

            switch (QuestType)
            {
                case QuestType.killEnemy:
                    UpdateProcess(QuestData.CurrentProcess, QuestData.TargetProcess);
                    break;
                case QuestType.toSpeekNPC:
                    UpdateProcess(QuestData.CurrentProcess, QuestData.TargetProcess);
                    break;
                case QuestType.completeSpecialTest:
                    QuestData.CurrentProcess = 0;
                    foreach (string itemId in Quest.IdTests)
                    {
                        int testId;

                        if (int.TryParse(itemId, out testId))
                        {
                            if (IGame.Instance.dataPlayer.isTestComplete(testId))
                            {
                                QuestData.CurrentProcess++;
                            }
                        }
                        else
                        {
                            Debug.LogError("?????? ? ???????????? ??????"); // TODO ??
                        }
                    }

                    UpdateProcess(QuestData.CurrentProcess, QuestData.TargetProcess);
                    break;

                default:
                    UpdateProcess(QuestData.CurrentProcess, QuestData.TargetProcess);
                    break;
            }

            if (QuestData.CurrentProcess >= QuestData.TargetProcess)
            {
                SetFinished();
                IGame.Instance.UIManager.UpdateQuestBackImg();
            }
        }

        private void UpdateProcess(float current, float target) // TODO magic numbers
        {
            _textProcess.text = current.ToString("F0") + $"/" + target.ToString("F0");
            SizeDeltaImageProcess.x = Mathf.Min(current / target * 900, 900);
            RectTransformProcess.sizeDelta = SizeDeltaImageProcess;
        }

        private void SetFinished()
        {
            QuestData.CompleteWaitAward = true;
            _button.GetComponent<Image>().color = new Color(0.81f, 0.952f, 0.768f); // TODO magic numbers
            _imageCheckNo.gameObject.SetActive(false);
            _imageCheck.gameObject.SetActive(true);
        }

        private void SetUnfinished()
        {
            QuestData.CompleteWaitAward = false;
            _button.GetComponent<Image>().color = new Color(0.9529412f, 0.8073038f, 0.7686275f); // TODO magic numbers
            _imageCheckNo.gameObject.SetActive(true);
            _imageCheck.gameObject.SetActive(false);
            QuestData.FullComplete = false;
        }

        private void FadeOutAndShrinkUIElement(GameObject uiElement)
        {
            VerticalLayoutGroup vertLGrroup = IGame.Instance.UIManager.QuestsContentScrollRect.content
                .GetComponent<VerticalLayoutGroup>();

            CanvasGroup canvasGroup = uiElement.AddComponent<CanvasGroup>();
            RectTransform rectTransform = uiElement.GetComponent<RectTransform>();

            Sequence sequence = DOTween.Sequence();

            sequence.Append(canvasGroup.DOFade(0, 1f));
            sequence.Join(rectTransform.DOSizeDelta(new Vector2(rectTransform.sizeDelta.x, 0), 1f));

            sequence.OnComplete(() => { uiElement.SetActive(false); });

            sequence.OnUpdate(() => { vertLGrroup.SetLayoutVertical(); });
        }
        
        internal void CheckTestCount() // TODO why internal
        {
            CheckUpdateAndComplete(false);
        }
    }
}