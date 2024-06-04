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
        DataPlayer playerData = FindObjectOfType<DataPlayer>();
        if (playerData == null || playerData.playerData == null || playerData.playerData.progress == null)
        {
            Debug.LogError("Player data or progress data is missing.");
        }

        foreach (OneLeson lesson in playerData.playerData.progress)
        {
            if (lesson.tests != null)
            {
                foreach (OneTestQuestion test in lesson.tests)
                {
                    if (test.id == testId)
                    {
                        Debug.Log("worked true");
                        ConversationManager.Instance.SetBool("ThisTestCompleted", true);
                    }
                }
            }
        }
        Debug.Log("worked false");
        Debug.LogWarning($"Test with ID {testId} not found.");
        ConversationManager.Instance.SetBool("ThisTestCompleted", false);

    }
}
