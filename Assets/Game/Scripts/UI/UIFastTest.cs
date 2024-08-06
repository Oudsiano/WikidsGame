using DG.Tweening;
using RPG.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIFastTest : MonoBehaviour
{
    [SerializeField] Button btnClose;

    [SerializeField] Button btn1;
    [SerializeField] Button btn2;
    [SerializeField] Button btn3;
    [SerializeField] Button btn4;

    [SerializeField] TMP_Text testText;

    private Health targetKillAfterTest;
    private OneFastTest currentTest;
    private bool isCorrect;

    private UIManager uiManager; // ?????? ?? UIManager

    CanvasGroup canvasGroup;

    private void Awake()
    {
        btnClose.onClick.AddListener(OnClickClose);

        btn1.onClick.AddListener(() => OnAnswerClick(1));
        btn2.onClick.AddListener(() => OnAnswerClick(2));
        btn3.onClick.AddListener(() => OnAnswerClick(3));
        btn4.onClick.AddListener(() => OnAnswerClick(4));

        canvasGroup = gameObject.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
    }

    // ????? ??? ????????? UIManager
    public void SetUIManager(UIManager manager)
    {
        uiManager = manager;
    }

    public void SetTexts(string mainText, string btnText1, string btnText2, string btnText3, string btnText4)
    {
        testText.text = mainText;
        btn1.GetComponentInChildren<TMP_Text>().text = btnText1;
        btn2.GetComponentInChildren<TMP_Text>().text = btnText2;
        btn3.GetComponentInChildren<TMP_Text>().text = btnText3;
        btn4.GetComponentInChildren<TMP_Text>().text = btnText4;

        ResetButton(btn1);
        ResetButton(btn2);
        ResetButton(btn3);
        ResetButton(btn4);
    }

    private void ResetButton(Button button)
    {
        canvasGroup.alpha = 1;
        button.interactable = true;
        button.GetComponent<Image>().color = Color.white;
    }

    public void ShowTestForAddedArrow(int stratIndexFastTests, int endIndexFastTests, int arrowCountToAdd)
    {
        if (IGame.Instance == null)
        {
            Debug.LogError("IGame.Instance is null");
            return;
        }

        if (IGame.Instance.FastTestsManager == null)
        {
            Debug.LogError("FastTestsManager is null");
            return;
        }

        var allTests = IGame.Instance.FastTestsManager.AvaliableTestsNow;
        if (allTests == null || allTests.Count == 0)
        {
            Debug.LogError("No tests available");
            FindAndFadeFastTextSavePoint("нет доступных тестов");
            return;
        }

        bool testFound = false;
        int attempts = 0;
        int maxAttempts = 5;

        while (!testFound && attempts < maxAttempts)
        {
            int randomId = UnityEngine.Random.Range(stratIndexFastTests, endIndexFastTests);
            currentTest = allTests[randomId];

            if (currentTest != null)
            {
                pauseClass.IsOpenUI = true;
                IGame.Instance.SavePlayerPosLikeaPause(true);
                gameObject.SetActive(true);

                SetTexts(
                    currentTest.QuestionText,
                    currentTest.Answer1,
                    currentTest.Answer2,
                    currentTest.Answer3,
                    currentTest.Answer4
                );

                testFound = true;
            }
            else
            {
                attempts++;
            }
        }

        if (!testFound)
        {
            Debug.Log($"No test found after {maxAttempts} attempts.");
        }
    }

    private void AddArrows(int count)
    {
        if (uiManager != null)
        {
            uiManager.IncreaseWeaponCharges(); // ????? ?????? ????? ????????? UIManager
        }
        else
        {
            Debug.LogError("uiManager is null");
        }

        Debug.Log($"{count} arrows added to the inventory.");
    }


    public void ShowTest(int stratIndexFastTests, int endIndexFastTests, Health targetKillAfterTest)
    {
        isCorrect = false;

        this.targetKillAfterTest = targetKillAfterTest;

        var allTests = IGame.Instance.FastTestsManager.AvaliableTestsNow;
        bool testFound = false;
        int attempts = 0;
        int maxAttempts = 5;

        while (!testFound && attempts < maxAttempts)
        {
            int randomId = UnityEngine.Random.Range(stratIndexFastTests, endIndexFastTests);
            currentTest = allTests[randomId];

            if (currentTest != null)
            {
                pauseClass.IsOpenUI = true;
                IGame.Instance.SavePlayerPosLikeaPause(true);
                gameObject.SetActive(true);

                SetTexts(
                    currentTest.QuestionText,
                    currentTest.Answer1,
                    currentTest.Answer2,
                    currentTest.Answer3,
                    currentTest.Answer4
                );

                testFound = true;
            }
            else
            {
                attempts++;
            }
        }

        if (!testFound)
        {
            Debug.Log($"No test found after {maxAttempts} attempts.");
            // ????? ???????? ?????? ?????????, ???? ???? ?? ?????? ????? ?????????? ???????
        }
    }

    private void OnAnswerClick(int answerIndex)
    {
        isCorrect = (answerIndex == currentTest.CorrectAnswerIndex);

        Button clickedButton = GetButtonByIndex(answerIndex);
        clickedButton.GetComponent<Image>().color = isCorrect ? Color.green : Color.red;

        Button correctButton = GetButtonByIndex(currentTest.CorrectAnswerIndex);
        if (!isCorrect)
        {
            correctButton.GetComponent<Image>().color = Color.green;
        }

        DisableButtons();
        canvasGroup.DOFade(0, 0.3f).SetDelay(2f).OnComplete(OnClickClose);

        if (isCorrect)
        {
            AddArrows(1); // ?????????? ????? ??? ?????????? ??????
        }
    }

    private Button GetButtonByIndex(int index)
    {
        switch (index)
        {
            case 1: return btn1;
            case 2: return btn2;
            case 3: return btn3;
            case 4: return btn4;
            default: throw new ArgumentOutOfRangeException(nameof(index), "Invalid button index");
        }
    }

    private void DisableButtons()
    {
        btn1.interactable = false;
        btn2.interactable = false;
        btn3.interactable = false;
        btn4.interactable = false;
    }

    private void OnClickClose()
    {
        gameObject.SetActive(false);
        IGame.Instance.SavePlayerPosLikeaPause(false);
        pauseClass.IsOpenUI = false;
        if (targetKillAfterTest != null)
        {
            if (isCorrect)
                targetKillAfterTest.AttackFromBehind(true);
            else
                targetKillAfterTest.MissFastTest();
        }
    }
    public void FindAndFadeFastTextSavePoint(string newText)
    {
        GameObject fastTextSavePoint = FindInactiveObjectByName("NotAvaibleTest");

        if (fastTextSavePoint != null)
        {
            Transform messageTextTransform = fastTextSavePoint.transform.Find("MessageText");
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
            canvasGroup.DOFade(0, 1).SetDelay(2).OnComplete(() =>
            {
                fastTextSavePoint.SetActive(false);
                canvasGroup.alpha = 1; // Reset alpha for next use
            });
        }
    }

    private GameObject FindInactiveObjectByName(string name)
    {
        Transform[] allObjects = Resources.FindObjectsOfTypeAll<Transform>();
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
