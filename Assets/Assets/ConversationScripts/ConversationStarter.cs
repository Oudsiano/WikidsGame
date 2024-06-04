using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DialogueEditor;

public class ConversationStarter : MonoBehaviour
{
    public bool DialogStarted;
    [SerializeField] public DownloadTestData data;
    [SerializeField] public NPCConversation myConversation;
    [SerializeField] public NPCConversation SecondConversation;

    public bool waitStartSecondDialog = false;

    public void StartDialog()
    {
        data = FindObjectOfType<DownloadTestData>();
        if (ConversationManager.Instance.IsConversationActive)
        {
            Debug.LogError("A conversation is already active. Please wait for it to finish before starting a new one.");
            return;
        }

        DataPlayer playerData = FindObjectOfType<DataPlayer>();
        ConversationManager.Instance.StartConversation(myConversation);
        DialogStarted = true;
        Debug.Log("Dialog Started");
        //TODO: Uncomment this and display the success count once the data object is properly initialized.
        //Debug.Log(data.countSuccessAnswers);
    }

    private void Update()
    {
        if (waitStartSecondDialog)
        {
            if (!ConversationManager.Instance.IsConversationActive)
                StartSecondDialog();
        }
    }

    public void StartSecondDialog()
    {
        waitStartSecondDialog = false;
        if (SecondConversation != null)
            ConversationManager.Instance.StartConversation(SecondConversation);
        else
        {
            Debug.LogError("No second conversation available.");
        }
    }

    public void StopDialogForce()
    {
        if (ConversationManager.Instance.IsConversationActive)
        {
            ConversationManager.Instance.EndButtonSelected();
        }
    }

    public void DialogEnded()
    {
        DialogStarted = false;
    }

    public void UpdateSuccessState()
    {
        ConversationManager.Instance.SetBool("TestSuccess", FindObjectOfType<GameAPI>().TestSuccessKey);
    }

    public void IsTestCompleted(int testId)
    {
        if (FindObjectOfType<GameAPI>().IsTestCompleted(testId))
        {
            Debug.Log("test completed znachenie update");
             ConversationManager.Instance.SetBool("ThisTestCompleted", FindObjectOfType<GameAPI>().IsTestCompleted(testId));
        }
        else
        {
            Debug.Log("test not completed znachenie update");

             ConversationManager.Instance.SetBool("ThisTestCompleted", FindObjectOfType<GameAPI>().IsTestCompleted(testId));
        }
    }
}