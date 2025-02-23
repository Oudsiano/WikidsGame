using System;
using Core;
using DG.Tweening;
using Healths;
using SceneManagement;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace UI
{
    public class UIFastTest : MonoBehaviour
    {
        [FormerlySerializedAs("btnClose")] [SerializeField]
        private Button _buttonClose;

        [FormerlySerializedAs("btn1")] [SerializeField]
        private Button _button1;

        [FormerlySerializedAs("btn2")] [SerializeField]
        private Button _button2;

        [FormerlySerializedAs("btn3")] [SerializeField]
        private Button _button3;

        [FormerlySerializedAs("btn4")] [SerializeField]
        private Button _button4;

        [FormerlySerializedAs("testText")] [SerializeField]
        private TMP_Text _testText;

        private Health _targetKillAfterTest;
        private OneFastTest _currentTest;
        private bool _isCorrect;
        private int _addArrows;

        private CanvasGroup _canvasGroup;

        private void Awake() // TODO construct
        {
            _buttonClose.onClick.AddListener(OnClickClose);

            _button1.onClick.AddListener(() => OnAnswerClick(1)); // TODO magic numbers
            _button2.onClick.AddListener(() => OnAnswerClick(2)); // TODO magic numbers
            _button3.onClick.AddListener(() => OnAnswerClick(3)); // TODO magic numbers
            _button4.onClick.AddListener(() => OnAnswerClick(4)); // TODO magic numbers

            _canvasGroup = gameObject.GetComponent<CanvasGroup>();

            if (_canvasGroup == null)
            {
                _canvasGroup = gameObject.AddComponent<CanvasGroup>();
            }
        }

        private void SetTexts(string mainText, string btnText1, string btnText2, string btnText3, string btnText4)
        {
            _testText.text = mainText;
            _button1.GetComponentInChildren<TMP_Text>().text = btnText1;
            _button2.GetComponentInChildren<TMP_Text>().text = btnText2;
            _button3.GetComponentInChildren<TMP_Text>().text = btnText3;
            _button4.GetComponentInChildren<TMP_Text>().text = btnText4;

            ResetButton(_button1);
            ResetButton(_button2);
            ResetButton(_button3);
            ResetButton(_button4);
        }

        private void ResetButton(Button button)
        {
            _canvasGroup.DOKill();
            _canvasGroup.alpha = 1;
            button.interactable = true;
            button.GetComponent<Image>().color = Color.white;
        }

        public void ShowTest(int stratIndexFastTests, int endIndexFastTests, int addArrows, Health targetKillAfterTest)
        {
            _isCorrect = false;
            _addArrows = addArrows;
            _targetKillAfterTest = targetKillAfterTest;

            var allTests = IGame.Instance.FastTestsManager.AvaliableTestsNow;

            if (allTests == null || allTests.Count == 0)
            {
                Debug.LogError("No tests available");
                FindAndFadeFastTextSavePoint("нет доступных тестов"); // TODO magic numbers
                return;
            }

            bool testFound = false;
            int attempts = 0;
            int maxAttemptsForFindRandomTEst = 5;

            while (testFound == false && attempts < maxAttemptsForFindRandomTEst)
            {
                int randomId = UnityEngine.Random.Range(stratIndexFastTests, endIndexFastTests);
                _currentTest = allTests[randomId];

                if (_currentTest != null)
                {
                    PauseClass.IsOpenUI = true;
                    IGame.Instance.SavePlayerPosLikeaPause(true);
                    gameObject.SetActive(true);

                    SetTexts(
                        _currentTest.QuestionText,
                        _currentTest.Answer1,
                        _currentTest.Answer2,
                        _currentTest.Answer3,
                        _currentTest.Answer4
                    );

                    testFound = true;
                }
                else
                {
                    attempts++;
                }
            }

            if (testFound == false)
            {
                Debug.Log($"No test found after {maxAttemptsForFindRandomTEst} attempts.");
            }
        }

        private void OnAnswerClick(int answerIndex)
        {
            _isCorrect = (answerIndex == _currentTest.CorrectAnswerIndex);

            Button clickedButton = GetButtonByIndex(answerIndex);
            clickedButton.GetComponent<Image>().color = _isCorrect ? Color.green : Color.red;

            Button correctButton = GetButtonByIndex(_currentTest.CorrectAnswerIndex);

            if (_isCorrect == false)
            {
                correctButton.GetComponent<Image>().color = Color.green;
            }

            DisableButtons();
            _canvasGroup.DOFade(0, 0.3f).SetDelay(2f).OnComplete(OnClickClose);

            if (_isCorrect && _addArrows > 0)
            {
                IGame.Instance._uiManager.IncreaseWeaponCharges(_addArrows); // ?????????? ????? ??? ?????????? ??????
            }
        }

        private Button GetButtonByIndex(int index)
        {
            switch (index)
            {
                case 1: return _button1;
                case 2: return _button2;
                case 3: return _button3;
                case 4: return _button4;
                default: throw new ArgumentOutOfRangeException(nameof(index), "Invalid button index");
            }
        }

        private void DisableButtons()
        {
            _button1.interactable = false;
            _button2.interactable = false;
            _button3.interactable = false;
            _button4.interactable = false;
        }

        private void OnClickClose()
        {
            gameObject.SetActive(false);
            IGame.Instance.SavePlayerPosLikeaPause(false);
            PauseClass.IsOpenUI = false;

            if (_targetKillAfterTest != null)
            {
                if (_isCorrect)
                {
                    _targetKillAfterTest.AttackFromBehind(true);
                }
                else
                {
                    _targetKillAfterTest.MissFastTest();
                }
            }
        }

        private void FindAndFadeFastTextSavePoint(string newText)
        {
            GameObject fastTextSavePoint = FindInactiveObjectByName("NotAvaibleTest"); // TODO find change

            if (fastTextSavePoint != null)
            {
                Transform messageTextTransform = fastTextSavePoint.transform.Find("MessageText"); // TODO can be cached
                
                if (messageTextTransform != null)
                {
                    TextMeshProUGUI textMeshPro = messageTextTransform.GetComponent<TextMeshProUGUI>();
                    
                    if (textMeshPro != null)
                    {
                        textMeshPro.text = newText;
                    }
                }

                CanvasGroup canvasGroup = fastTextSavePoint.GetComponent<CanvasGroup>();
                
                if (canvasGroup == null)
                {
                    canvasGroup = fastTextSavePoint.AddComponent<CanvasGroup>();
                }

                canvasGroup.DOKill();

                fastTextSavePoint.SetActive(true);
                canvasGroup.alpha = 1; // Ensure alpha is reset to 1 before fading
                canvasGroup.DOFade(0, 1).SetDelay(2).OnComplete(() => // TODO magic numbers
                {
                    fastTextSavePoint.SetActive(false);
                    canvasGroup.alpha = 1; // Reset alpha for next use
                });
            }
        }

        private GameObject FindInactiveObjectByName(string name)
        {
            Transform[] allObjects = Resources.FindObjectsOfTypeAll<Transform>(); // TODO find change
            
            foreach (Transform obj in allObjects)
            {
                if (obj.name == name && obj.hideFlags == HideFlags.None)
                {
                    return obj.gameObject;
                }
            }

            return null;
        }
    }
}