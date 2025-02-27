using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using DG.Tweening;
using SceneManagement;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TestTableGenerator : MonoBehaviour
{
    public TMP_Text outputText;

    private FastTestsManager _fastTestsManager;
    
    public void Construct(FastTestsManager fastTestsManager)
    {
        _fastTestsManager = fastTestsManager;
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

    public void Regen()
    {
        DataTable table = new DataTable("TestTable");

        table.Columns.Add("Question", typeof(string));
        table.Columns.Add("CorrectAnswer", typeof(string));

        var allTests = _fastTestsManager.AvaliableTestsNow;

        if (allTests != null && allTests.Count > 0)
        {
            foreach (var test in allTests)
            {
                string correctAnswer = GetCorrectAnswer(test, test.CorrectAnswerIndex);
                table.Rows.Add(test.QuestionText, correctAnswer);
            }
        }
        else
        {
            Debug.Log("No tests available");
        }


        string result = "";

        foreach (DataRow row in table.Rows)
        {
            result += $"{row["Question"]}\n";
            result += $"{row["CorrectAnswer"]}\n";
            result += "-------------------\n";
        }

        outputText.text = result;
    }
}

