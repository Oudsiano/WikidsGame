using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DialogueEditor;
using UnityEngine.Serialization;

public class ConversationOpenScene : MonoBehaviour
{
    [FormerlySerializedAs("openConversation")] [SerializeField] private NPCConversation _openConversation;
    [SerializeField] private ConversationManager _conversationManager;
    [SerializeField] private ConversationManager _conversationManagerMobile;

    private void Start() // TODO Construct
    {
        SetUI();
        ConversationManager.Instance.StartConversation(_openConversation);
    }

    private void SetUI()
    {
        _conversationManager.gameObject.SetActive(true);
        
        // if (DeviceChecker.IsMobileDevice())
        // {
        //     _conversationManagerMobile.gameObject.SetActive(true);
        // }
        // else
        // {
        //     _conversationManager.gameObject.SetActive(true);
        // }
    }
}
