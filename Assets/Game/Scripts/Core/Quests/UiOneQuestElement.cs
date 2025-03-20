using System.Collections.Generic;
using Core.Quests.Data;
using Core.Quests.QuestsEnums;
using Data;
using DG.Tweening;
using Saving;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UI;
using UnityEngine.Serialization;

namespace Core.Quests
{
    public class UiOneQuestElement : MonoBehaviour // TODO move to UI
    {
        private const float SIZE_DELTA_VALUE = 900;
        private const string FORMAT = "F0";

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
        private OneQuestData _questData;

        [FormerlySerializedAs("quest")] [SerializeField]
        private OneQuest _quest;

        [FormerlySerializedAs("rtimgProcess")] [SerializeField]
        private RectTransform _rectTransformProcess;

        [FormerlySerializedAs("sizeDeltaImgProcess")] [SerializeField]
        private Vector2 _sizeDeltaImageProcess;

        [FormerlySerializedAs("ListNeedStartConversations")] [SerializeField]
        private List<string> _listNeedStartConversations;
        
        private readonly Color _pastelGreenColor = new Color(0.81f, 0.952f, 0.768f);
        private readonly Color _pastelPinkColor = new Color(0.9529412f, 0.8073038f, 0.7686275f);
        private RectTransform _rectTransformButton;
        private List<bool> _pointSuccess;

        private DataPlayer _dataPlayer;
        private SaveGame _saveGame;
        private UIManager _uiManager;
        private GameAPI _gameAPI;
        private Sequence _fadeOutSequence;

        public OneQuestData QuestData => _questData;
        public OneQuest Quest => _quest;

        public QuestType QuestType
        {
            get => _questData.QuestType;
            set => _questData.QuestType = value;
        }

        public void Construct(DataPlayer dataPlayer, SaveGame saveGame, UIManager uiManager, GameAPI gameAPI)
        {
            _dataPlayer = dataPlayer;
            _saveGame = saveGame;
            _uiManager = uiManager;
            _gameAPI = gameAPI;
        }

        public void SetProgress(int count) // TODO not used code
        {
            if (_questData.AlreadyStarted == false)
            {
                return;
            }

            _questData.CurrentProcess = count;
            CheckUpdateAndComplete();
        }

        public void AddOneProcess()
        {
            if (_questData.AlreadyStarted == false)
            {
                return;
            }

            _questData.CurrentProcess++;
            CheckUpdateAndComplete();
        }

        public void SetupConversation(ConversationStarter conversationStarter)
        {
            if (_questData.AlreadyStarted == false)
            {
                return;
            }

            for (int i = 0; i < _listNeedStartConversations.Count; i++)
            {
                if (_listNeedStartConversations[i] == conversationStarter.name)
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
                    _questData.CurrentProcess++;
                }
            }

            CheckUpdateAndComplete();
        }

        public void SetQuest(OneQuest quest)
        {
            _quest = quest;

            InitializeQuestProperties();
            ProcessQuestType();
            SetQuestAwardText();
            SetUnfinished();

            _button.onClick.AddListener(OnClickButton);
            LoadStartedQuestData();
        }
        
        internal void CheckTestCount()
        {
            Debug.Log("CheckTestCount");
            CheckUpdateAndComplete(false);
        }
        
        private void InitializeQuestProperties()
        {
            _rectTransformButton = _button.GetComponent<RectTransform>();
            _textTitle.text = _quest.questTitle;
            _textDescription.text = _quest.questDescription;
            QuestType = _quest.questType;
            _questData.TargetProcess = _quest.questTargetCount;
        }

        private void ProcessQuestType()
        {
            if (_quest.questType == QuestType.completeSpecialTest)
            {
                _questData.TargetProcess = _quest.IdTests.Count;
            }

            _rectTransformProcess = _imgProcess.GetComponent<RectTransform>();
            _sizeDeltaImageProcess = _rectTransformProcess.sizeDelta;

            if (_quest.questType == QuestType.toSpeekNPC)
            {
                _questData.TargetProcess = _quest.ListNeedConversationsStarter.Count;
                _pointSuccess = new List<bool>();
                _listNeedStartConversations = new List<string>();

                foreach (var conversation in _quest.ListNeedConversationsStarter)
                {
                    _pointSuccess.Add(false);
                    _listNeedStartConversations.Add(conversation);
                }
            }
        }

        private void SetQuestAwardText()
        {
            switch (_quest.questAwardType)
            {
                case QuestAwardType.none:
                    _textAward.text = "";
                    break;
                case QuestAwardType.money:
                    _textAward.text = $"{_quest.awardFirstWord} {_quest.countMoney} {_quest.awardLastWord}";
                    break;
                case QuestAwardType.item:
                    _textAward.text = $"{_quest.awardFirstWord}  {_quest.awardItem.name}";
                    break;
            }
        }

        private void LoadStartedQuestData()
        {
            if (_dataPlayer.PlayerData.startedQuests != null &&
                _dataPlayer.PlayerData.startedQuests.TryGetValue(_quest.name, out var quest))
            {
                _questData = quest;
                CheckUpdateAndComplete(false);
            }
        }

        private void OnClickButton() // TODO magic numbers
        {
            if (_questData.CompleteWaitAward == false)
            {
                int x = 10;
                int y = 0;

                _rectTransformButton.DOShakeAnchorPos(1f, new Vector2(x, y), vibrato: 8, randomness: 0,
                        snapping: false, fadeOut: false)
                    .OnComplete(() => _rectTransformButton.anchoredPosition = Vector2.zero);

                return;
            }

            if (_questData.FullComplete)
            {
                return;
            }

            switch (_quest.questAwardType)
            {
                case QuestAwardType.none:
                    break;

                case QuestAwardType.money:
                    _saveGame.Coins += _quest.countMoney;
                    break;

                case QuestAwardType.item:
                    _uiManager.uIBug.TryAddEquipToBug(_quest.awardItem);
                    break;
            }

            MarkQuestAsComplete();
            FadeOutAndShrinkUIElement(gameObject);
            _questData.FullComplete = true;
            _uiManager.UpdateQuestBackImg();
        }

        private void MarkQuestAsComplete()
        {
            if (_dataPlayer.PlayerData.completedQuests.Contains(_quest.name) == false)
            {
                _dataPlayer.PlayerData.completedQuests.Add(_quest.name);
                _gameAPI.SaveUpdater();
            }
        }

        private void CheckUpdateAndComplete(bool withSave = true)
        {
            if (withSave)
            {
                _dataPlayer.PlayerData.startedQuests ??= new Dictionary<string, OneQuestData>();

                _questData.QuestName = _quest.name;
                _dataPlayer.PlayerData.startedQuests[_quest.name] = _questData;
                _gameAPI.SaveUpdater();
            }

            switch (QuestType)
            {
                case QuestType.killEnemy:
                    UpdateProcess(_questData.CurrentProcess, _questData.TargetProcess);
                    break;
                
                case QuestType.toSpeekNPC:
                    UpdateProcess(_questData.CurrentProcess, _questData.TargetProcess);
                    break;
                
                case QuestType.completeSpecialTest:
                    _questData.CurrentProcess = 0;
                    
                    foreach (string itemId in _quest.IdTests)
                    {
                        if (int.TryParse(itemId, out int testId))
                        {
                            if (_dataPlayer.IsTestComplete(testId))
                            {
                                _questData.CurrentProcess++;
                            }
                        }
                        else
                        {
                            Debug.LogError("?????? ? ???????????? ??????"); // TODO ??
                        }
                    }

                    UpdateProcess(_questData.CurrentProcess, _questData.TargetProcess);
                    break;

                default:
                    UpdateProcess(_questData.CurrentProcess, _questData.TargetProcess);
                    break;
            }

            if (_questData.CurrentProcess >= _questData.TargetProcess)
            {
                SetComplete();
                _uiManager.UpdateQuestBackImg();
            }
        }

        private void UpdateProcess(float current, float target)
        {
            _textProcess.text = current.ToString(FORMAT) + $"/" + target.ToString(FORMAT);
            _sizeDeltaImageProcess.x = Mathf.Min(current / target * SIZE_DELTA_VALUE, SIZE_DELTA_VALUE);
            _rectTransformProcess.sizeDelta = _sizeDeltaImageProcess;
        }

        private void SetComplete()
        {
            _questData.CompleteWaitAward = true;
            _button.GetComponent<Image>().color = _pastelGreenColor; // TODO magic numbers
            _imageCheckNo.gameObject.SetActive(false);
            _imageCheck.gameObject.SetActive(true);
        }

        private void SetUnfinished()
        {
            _questData.CompleteWaitAward = false;
            _button.GetComponent<Image>().color = _pastelPinkColor; // TODO magic numbers
            _imageCheckNo.gameObject.SetActive(true);
            _imageCheck.gameObject.SetActive(false);
            _questData.FullComplete = false;
        }

        private void FadeOutAndShrinkUIElement(GameObject uiElement)
        {
            float duration = 1f;
            
            VerticalLayoutGroup vertLGrroup = _uiManager.QuestsContentScrollRect.content
                .GetComponent<VerticalLayoutGroup>();

            CanvasGroup canvasGroup = uiElement.AddComponent<CanvasGroup>();
            RectTransform rectTransform = uiElement.GetComponent<RectTransform>();

            _fadeOutSequence = DOTween.Sequence();
            
            _fadeOutSequence.Append(canvasGroup.DOFade(0, duration));
            _fadeOutSequence.Join(rectTransform.DOSizeDelta(new Vector2(rectTransform.sizeDelta.x, 0), duration));

            _fadeOutSequence.OnComplete(() => { uiElement.SetActive(false); });

            _fadeOutSequence.OnUpdate(() => { vertLGrroup.SetLayoutVertical(); });
        }
    }
}