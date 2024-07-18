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

    private void Awake()
    {
        btnClose.onClick.AddListener(OnClickClose);

        btn1.onClick.AddListener(() => OnAnswerClick(1));
        btn2.onClick.AddListener(() => OnAnswerClick(2));
        btn3.onClick.AddListener(() => OnAnswerClick(3));
        btn4.onClick.AddListener(() => OnAnswerClick(4));
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
        button.interactable = true;
        button.GetComponent<Image>().color = Color.white;
    }

    public void ShowTest(int stratIndexFastTests, int endIndexFastTests, Health targetKillAfterTest)
    {
        pauseClass.IsOpenUI = true;
        IGame.Instance.SavePlayerPosLikeaPause(true);
        gameObject.SetActive(true);

        this.targetKillAfterTest = targetKillAfterTest;

        var allTests = IGame.Instance.FastTestsManager.AllFastTests;
        int randomIndex = UnityEngine.Random.Range(stratIndexFastTests, endIndexFastTests + 1);
        currentTest = allTests[randomIndex];

        SetTexts(
            currentTest.QuestionText,
            currentTest.Answer1,
            currentTest.Answer2,
            currentTest.Answer3,
            currentTest.Answer4
        );
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
}
