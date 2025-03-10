using System.Collections;
using System.Collections.Generic;
using Core;
using Core.Quests;
using Core.Quests.Data;
using Data;
using UnityEngine;
using DialogueEditor;
using Saving;

public class ConversationStarter : MonoBehaviour
{
    private const string TEST_SUCCESS_NAME = "TestSuccess";
    private const string THIS_TEST_COMPLETED_NAME = "ThisTestCompleted";
    public static bool IsDialogActive;
    public bool DialogStarted;
    [SerializeField] public DownloadTestData data;
    [SerializeField] public NPCConversation myConversation;
    [SerializeField] public NPCConversation SecondConversation;

    [Header("TestID")]
    [SerializeField] public int TestID;

    public bool waitStartSecondDialog = false;

    private QuestManager _questManager;
    private DataPlayer _dataPlayer;
    private GameAPI _gameAPI;
    private DownloadTestData _downloadData;
    
    public void Construct(QuestManager questManager, DataPlayer dataPlayer, GameAPI gameAPI)
    {
        _questManager = questManager;
        _dataPlayer = dataPlayer;
        _gameAPI = gameAPI;
        _downloadData = FindObjectOfType<DownloadTestData>();
    }
    
    public void StartDialog()
    {
        if (ConversationManager.Instance.IsConversationActive)
        {
            Debug.LogError("A conversation is already active. Please wait for it to finish before starting a new one.");
            return;
        }

        _questManager.SetupConversation(this);
        
        ConversationManager.OnConversationEnded += DialogEnded;  // ????????????? ?? ??????? ????????? ???????
        ConversationManager.Instance.StartConversation(myConversation);
        DialogStarted = true;
        IsDialogActive = true;
        PauseClass.IsDialog = true;
        Debug.Log("Dialog Started");
        //TODO: Uncomment this and display the success count once the data object is properly initialized.
        //Debug.Log(data.countSuccessAnswers);
    }

    private void Update()
    {
        if (waitStartSecondDialog && ConversationManager.Instance != null)
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
        PauseClass.IsDialog = false;
        ConversationManager.OnConversationEnded -= DialogEnded; 
    }

    public void UpdateSuccessState()
    {
        ConversationManager.Instance.SetBool(TEST_SUCCESS_NAME, _gameAPI.TestSuccessKey);
    }

    public void IsTestCompleted(int testId)
    {
        if (TestID==0)
        {
            Debug.LogWarning("Not have TestID in inspector");
            TestID = testId;
        }

        _gameAPI.IsTestCompleted(TestID, (isCompleted) =>
        {
            if (isCompleted)
            {
                Debug.Log("test completed znachenie update");
                ConversationManager.Instance.SetBool(THIS_TEST_COMPLETED_NAME, true);
            }
            else
            {
                Debug.Log("test not completed znachenie update");
                ConversationManager.Instance.SetBool(THIS_TEST_COMPLETED_NAME, false);
            }
        });
    }

    public void AnywayStartNewQuest(OneQuest quest)
    {
        _questManager.StartNewQuest(quest);
    }

    public void OnlyOneTimeStartNewQuest(OneQuest quest)
    {
        _questManager.StartNewQuestIfNot(quest);
    }
}
