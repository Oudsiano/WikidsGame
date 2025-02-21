using Newtonsoft.Json;

namespace Core.Quests
{
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


        [JsonConstructor]
        public OneQuestData()
        {

        }
    }
}
