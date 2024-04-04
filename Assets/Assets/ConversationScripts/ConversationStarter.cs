using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DialogueEditor;

public class ConversationStarter : MonoBehaviour
{
    [SerializeField] public NPCConversation myConversation;

    public void StartDialog()
    {
        ConversationManager.Instance.StartConversation(myConversation);
        Debug.Log("Dialog Started");
    }
}