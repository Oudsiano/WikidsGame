using DialogueEditor;
using FarrokhGames.Inventory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "OneQuest", menuName = "OneQuest", order = 0)]
public class OneQuest : ScriptableObject
{
    [SerializeField] public string questTitle;
    [SerializeField] public string questDescription;
    [SerializeField] public string awardFirstWord;
    [SerializeField] public string awardLastWord;
    [SerializeField] public QuestType questType;
    [SerializeField] public int questTargetCount;


    [Header("toSpeekNPC block")]
    ////Специальное условие на необходимость поговорить с определенным количеством НПС
    [SerializeField] public List<string> ListNeedConversationsStarter;


    [Header("killSpecialEnemy block")]
    [SerializeField] public string specialEnemyName;


    [Header("completeSpecialTest block")]
    [SerializeField] public string IdTest;

    [Header("Award block")]
    [SerializeField] public QuestAwardType questAwardType;
    [SerializeField] public float countMoney;
    [SerializeField] public ItemDefinition awardItem;



}
