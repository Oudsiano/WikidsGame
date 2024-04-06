using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DialogueEditor;

public class ConversationStarter : MonoBehaviour
{
    public bool DialogStarted;

    [SerializeField] public NPCConversation myConversation;
    public void StartDialog()
    {
        DataPlayer playerData = FindObjectOfType<DataPlayer>();
        ConversationManager.Instance.StartConversation(myConversation);
        DialogStarted = true;
            Debug.Log("Dialog Started");
            ConversationManager.Instance.SetBool("TestSuccess", playerData.playerData.testSuccess);
        
    }

    public void DialogEnded()
    {
        DialogStarted = false;
    }
}
