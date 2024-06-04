using DialogueEditor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckItem : MonoBehaviour
{
    [SerializeField] public string nameItem;
    [SerializeField] public string nameParamInNPC;

    public void ChekItem()
    {
        if (IGame.Instance.UIManager.uIBug.TryTakeQuestItem(nameItem))
        {
            ConversationManager.Instance.SetBool(nameParamInNPC, true);
        }
    }
}
