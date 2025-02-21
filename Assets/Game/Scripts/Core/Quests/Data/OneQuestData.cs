using Core.Quests.QuestsEnums;
using Newtonsoft.Json;
using UnityEngine.Serialization;

namespace Core.Quests.Data
{
    [System.Serializable]
    public class OneQuestData // TODO its a data 
    {
        [FormerlySerializedAs("compliteWaitAward")] public bool CompleteWaitAward = false;
        [FormerlySerializedAs("alreadyStarted")] public bool AlreadyStarted = true; 
        [FormerlySerializedAs("fullComplite")] public bool FullComplete = false; 

        [FormerlySerializedAs("currentProcess")] public float CurrentProcess;
        [FormerlySerializedAs("targetProcess")] public float TargetProcess;

        [FormerlySerializedAs("questType")] public QuestType QuestType;
        public string QuestName;
        
        [JsonConstructor]
        public OneQuestData()
        {

        }
    }
}
