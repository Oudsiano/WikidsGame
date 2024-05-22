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
            Debug.LogError("??????? ????????? ?????? ??? ??? ?????????? ???????. ??????????? ??????, ?? ???????");
            return;
        }

        DataPlayer playerData = FindObjectOfType<DataPlayer>();
        ConversationManager.Instance.StartConversation(myConversation);
        DialogStarted = true;
        Debug.Log("Dialog Started");
        //TODO: ??????? ?????? ? ???????? ?????? data.countSuccessAnswer ??? ????? ???????.
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
            Debug.LogError("not have second conversation");
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
}
