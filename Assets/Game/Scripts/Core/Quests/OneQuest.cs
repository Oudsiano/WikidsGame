using System.Collections.Generic;
using FarrokhGames.Inventory.Examples;
using UnityEngine;

namespace Core.Quests
{
    [CreateAssetMenu(fileName = "OneQuest", menuName = "OneQuest", order = 0)]
    public class OneQuest : ScriptableObject // TODO SO is DATA Rename
    { 
        [SerializeField] public string questTitle;
        [SerializeField] public string questDescription;
        [SerializeField] public string awardFirstWord;
        [SerializeField] public string awardLastWord;
        [SerializeField] public QuestType questType;
        [SerializeField] public int questTargetCount;

        [Header("toSpeekNPC block")]
        [SerializeField] public List<string> ListNeedConversationsStarter;

        [Header("killSpecialEnemy block")]
        [SerializeField] public string specialEnemyName;

        [Header("completeSpecialTest block")]
        [SerializeField] public List<string> IdTests;

        [Header("Award block")]
        [SerializeField] public QuestAwardType questAwardType;
        [SerializeField] public float countMoney;
        [SerializeField] public ItemDefinition awardItem;

        [Header("Archer Enemies")]
        [SerializeField] public List<string> archerEnemyNames;
    }
}
