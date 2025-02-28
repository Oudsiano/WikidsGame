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

        public void Construct(GameAPI gameAPI)
        {
            _gameAPI = gameAPI;
        }

        public void DownloadData() 
        {
            starterConversation = FindObjectOfType<ConversationStarter>(); // TODO find change
            _gameAPI.UpdateDataTest(IDLesson, starterConversation);
            
            //ConversationManager.Instance.SetBool("TestSuccess", true); // TODO not used code
        }
    }
}
