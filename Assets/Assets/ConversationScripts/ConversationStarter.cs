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
        if (ConversationManager.Instance.IsConversationActive)
        {
            Debug.LogError("Попытка запустить диалог при уже запущенном диалоге. Некритичная ошибка, но косячок");
            return;
        }

        DataPlayer playerData = FindObjectOfType<DataPlayer>();
        ConversationManager.Instance.StartConversation(myConversation);
        DialogStarted = true;
        Debug.Log("Dialog Started");

    }

    public void DialogEnded()
    {
        DialogStarted = false;
    }
}
