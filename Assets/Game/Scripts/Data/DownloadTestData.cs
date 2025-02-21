using Saving;
using UnityEngine;
using UnityEngine.Serialization;

namespace Data
{
    public class DownloadTestData : MonoBehaviour
    {
        [FormerlySerializedAs("gameAPI")][SerializeField] private GameAPI _gameAPI;
    
        public ConversationStarter starterConversation;
        public int IDLesson;
    
        public void DownloadData()
        {
            _gameAPI = FindObjectOfType<GameAPI>(); // TODO find change
            starterConversation = FindObjectOfType<ConversationStarter>(); // TODO find change
            _gameAPI.UpdateDataTest(IDLesson, starterConversation);
       

            //ConversationManager.Instance.SetBool("TestSuccess", true); // TODO not used code
        }
    }
}
