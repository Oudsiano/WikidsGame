using DialogueEditor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DownloadTestData : MonoBehaviour
{
    [SerializeField] private GameAPI gameAPI;
    public ConversationStarter starterConversation;
    public void DownloadData()
    {
        gameAPI = FindObjectOfType<GameAPI>();
        starterConversation = FindObjectOfType<ConversationStarter>();
        gameAPI.UpdataDataTest();
       

        //ConversationManager.Instance.SetBool("TestSuccess", true);
    }
}
