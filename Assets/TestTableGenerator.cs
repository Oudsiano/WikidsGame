using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TestTableGenerator : MonoBehaviour
{
    public TMP_Text outputText; // TMP_Text ??? ??????????? ???????
    public ScrollRect scrollRect; // ScrollRect ??? ????????? ?????????
    private UIManager uiManager;

    private void Start()
    {
        // ???????? ?????? ?? UIManager
        uiManager = FindObjectOfType<UIManager>();

        // ????????? ???????? ???????
        DataTable testTable = GenerateTestTable();

        // ??????????? ???????? ??????? ? ????????? ??????????
        DisplayTestTable(testTable);
    }

    private DataTable GenerateTestTable()
    {
        DataTable table = new DataTable("TestTable");

        // ?????????? ???????? ??? ???????? ? ?????????? ???????
        table.Columns.Add("Question", typeof(string));
        table.Columns.Add("CorrectAnswer", typeof(string));

        // ????????? ?????? ????????? ??????
        var allTests = IGame.Instance.FastTestsManager.AvaliableTestsNow;

        if (allTests != null && allTests.Count > 0)
        {
            foreach (var test in allTests)
            {
                // ????????? ??????????? ?????? ?? ?????? ???????
                string correctAnswer = GetCorrectAnswer(test, test.CorrectAnswerIndex);

                // ?????????? ??????? ? ??????????? ?????? ? ???????
                table.Rows.Add(test.QuestionText, correctAnswer);
            }
        }
        else
        {
            Debug.LogError("No tests available");
            FindAndFadeFastTextSavePoint("??? ????????? ??????");
            Debug.Log("?????");
        }

        return table;
    }

    private string GetCorrectAnswer(OneFastTest test, int correctAnswerIndex)
    {
        switch (correctAnswerIndex)
        {
            case 1: return test.Answer1;
            case 2: return test.Answer2;
            case 3: return test.Answer3;
            case 4: return test.Answer4;
            default: throw new ArgumentOutOfRangeException(nameof(correctAnswerIndex), "Invalid answer index");
        }
    }

    private void DisplayTestTable(DataTable table)
    {
        if (outputText == null)
        {
            Debug.LogError("Output Text is not assigned.");
            return;
        }

        // ?????????? ?????? ??? ???????????
        string result = "";

        foreach (DataRow row in table.Rows)
        {
            result += $"{row["Question"]}\n";
            result += $"{row["CorrectAnswer"]}\n";
            result += "-------------------\n";
        }

        // ????????? ??????????? ?????? ? ????????? ?????????
        outputText.text = result;
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

