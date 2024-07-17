using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using UnityEngine;

public class OneFastTest
{
    public int Id { get; set; }
    public string QuestionText { get; set; }
    public string Answer1 { get; set; }
    public string Answer2 { get; set; }
    public string Answer3 { get; set; }
    public string Answer4 { get; set; }
    public int CorrectAnswerIndex { get; set; }

    // Конструктор
    public OneFastTest(int id, string questionText, string answer1, string answer2, string answer3, string answer4, int correctAnswerIndex)
    {
        Id = id;
        QuestionText = questionText;
        Answer1 = answer1;
        Answer2 = answer2;
        Answer3 = answer3;
        Answer4 = answer4;
        CorrectAnswerIndex = correctAnswerIndex;
    }
}

public class FastTestsManager
{

    public List<OneFastTest> AllFastTests;

    public void init()
    {
        AllFastTests = new List<OneFastTest>();
        OneFastTest currentTest;

        currentTest = new OneFastTest(
            1,
            "Текст вопроса",
            "ответ1",
            "ответ2",
            "ответ3",
            "ответ4",
            2 //Correct answer
            );
        AllFastTests.Add(currentTest);

        currentTest = new OneFastTest(
            2,
            "Текст вопроса",
            "ответ1",
            "ответ2",
            "ответ3",
            "ответ4",
            2 //Correct answer
            );
        AllFastTests.Add(currentTest);

    }

}
