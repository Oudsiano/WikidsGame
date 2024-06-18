using DialogueEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

[System.Serializable]
public class OneQuestData
{

    public bool compliteWaitAward = false;
    public bool alreadyStarted = true; //?????? ?? ??? ?????
    public bool fullComplite = false; //???????? ?????????? ?????

    public float currentProcess;
    public float targetProcess;

    public QuestType questType;
    public string QuestName;

}
