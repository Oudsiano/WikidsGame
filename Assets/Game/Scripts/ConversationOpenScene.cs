using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DialogueEditor;
using UnityEngine.Serialization;

public class ConversationOpenScene : MonoBehaviour
{
    [FormerlySerializedAs("openConversation")] [SerializeField] private NPCConversation _openConversation;

    private void Start() // TODO Construct
    {
        ConversationManager.Instance.StartConversation(_openConversation);
    }
}
