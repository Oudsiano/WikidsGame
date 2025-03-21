using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DialogueEditor;

public class ConversationStarter : MonoBehaviour
{
    public static bool IsDialogActive;
    public bool DialogStarted;
    [SerializeField] public DownloadTestData data;
    [SerializeField] public NPCConversation myConversation;
    [SerializeField] public NPCConversation SecondConversation;

    [Header("TestID")]
    [SerializeField] public int TestID;

    public bool waitStartSecondDialog = false;

    public void StartDialog()
    {
        data = FindObjectOfType<DownloadTestData>();
        if (ConversationManager.Instance.IsConversationActive)
        {
            Debug.LogError("A conversation is already active. Please wait for it to finish before starting a new one.");
            return;
        }

        IGame.Instance.QuestManager.startedConversation(this);

        DataPlayer playerData = FindObjectOfType<DataPlayer>();
        ConversationManager.OnConversationEnded += DialogEnded;  // ????????????? ?? ??????? ????????? ???????
        ConversationManager.Instance.StartConversation(myConversation);
        DialogStarted = true;
        IsDialogActive = true;
        pauseClass.IsDialog = true;
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
        IsDialogActive = false;
        pauseClass.IsDialog = false;
        ConversationManager.OnConversationEnded -= DialogEnded;  // ???????????? ?? ??????? ????????? ???????
    }

    public void UpdateSuccessState()
    {
        ConversationManager.Instance.SetBool("TestSuccess", FindObjectOfType<GameAPI>().TestSuccessKey);
    }

    public void IsTestCompleted(int testId)
    {
        if (TestID==0)
        {
            Debug.LogWarning("Not have TestID in inspector");
            TestID = testId;
        }

        FindObjectOfType<GameAPI>().IsTestCompleted(TestID, (isCompleted) =>
        {
            if (isCompleted)
            {
                Debug.Log("test completed znachenie update");
                ConversationManager.Instance.SetBool("ThisTestCompleted", true);
            }
            else
            {
                Debug.Log("test not completed znachenie update");
                ConversationManager.Instance.SetBool("ThisTestCompleted", false);
            }
        });
    }

    public void AnywayStartNewQuest(OneQuest quest)
    {
        IGame.Instance.QuestManager.StartNewQuest(quest);
    }

    public void OnlyOneTimeStartNewQuest(OneQuest quest)
    {
        IGame.Instance.QuestManager.StartNewQuestIfNot(quest);
    }
}
