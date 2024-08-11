using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DialogueEditor;

public class ConversationOpenScene : MonoBehaviour
{
    [SerializeField] private NPCConversation openConversation;

    private void Start()
    {
        ConversationManager.Instance.StartConversation(openConversation);
    }
}
