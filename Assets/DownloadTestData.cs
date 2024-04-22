using DialogueEditor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DownloadTestData : MonoBehaviour
{
    [SerializeField] private GameAPI gameAPI;
    public ConversationStarter starterConversation;
    public int countSuccessAnswers;
    public void DownloadData()
    {
        gameAPI = FindObjectOfType<GameAPI>();
        starterConversation = FindObjectOfType<ConversationStarter>();
        gameAPI.UpdataDataTest();

        int countSuccessAnswer = 0;

        if (IGame.Instance.dataPLayer.playerData.progress != null)
        {
            foreach (OneLeson item in IGame.Instance.dataPLayer.playerData.progress)
            {
                if (item != null)
                    foreach (OneTestQuestion item2 in item.tests)
                    {
                        if (item2.completed)
                            countSuccessAnswer++;
                    }
            }

        }
        countSuccessAnswers = countSuccessAnswer;
        RPG.Core.MainPlayer.Instance.ChangeCountEnegry(countSuccessAnswer);
        starterConversation.SetTestSuccess();
        Debug.Log("?????????? ??????? " + countSuccessAnswer);

        //ConversationManager.Instance.SetBool("TestSuccess", true);
    }
}
