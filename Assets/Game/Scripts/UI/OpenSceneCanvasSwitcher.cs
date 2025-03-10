using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DialogueEditor;


public class OpenSceneCanvasSwitcher : MonoBehaviour
{
    [SerializeField] private Button _startButton;
    [SerializeField] private Button _skipButton;
    [SerializeField] private NPCConversation _startConversation;
    
    // private void Awake()
    // {
    //     var conversation = _startConversation.Deserialize();
    //     var root = conversation.Root;
    //
    //     Debug.Log("Root Speech: " + root.Text);
    //
    //     foreach (var conn in root.Connections)
    //     {
    //         if (conn is SpeechConnection speechConn)
    //             Debug.Log(" -> Speech: " + speechConn.SpeechNode.Text);
    //         else if (conn is OptionConnection optionConn)
    //             Debug.Log(" -> Option: " + optionConn.OptionNode.Text);
    //     }
    // }
    
    private void OnEnable()
    {
        _startButton.onClick.AddListener(StartDialogue);
        _skipButton.onClick.AddListener(SkipDialogue);
    }

    private void OnDisable()
    {
        _startButton.onClick.RemoveListener(StartDialogue);
        _skipButton.onClick.RemoveListener(SkipDialogue);
    }

    private void StartDialogue()
    {
        // ConversationManager.Instance.StartConversation(_startConversation);
        ConversationManager.Instance.SelectNextOption();
        // ConversationManager.Instance.PressSelectedOption();
        ConversationManager.Instance.OptionSelected(GetSkipOption(_startConversation, "Начать"));
        ConversationManager.Instance.PressSelectedOption();
    }

    private void SkipDialogue()
    {
        ConversationManager.Instance.OptionSelected(GetSkipOption(_startConversation, "Скип"));
        ConversationManager.Instance.PressSelectedOption();
    }
    
    private OptionNode GetSkipOption(NPCConversation npcConversation, string text)
    {
        var conversation = npcConversation.Deserialize();
        var root = conversation.Root;

        foreach (var conn in root.Connections)
        {
            if (conn is SpeechConnection speechConn)
                Debug.Log(" -> Speech: " + speechConn.SpeechNode.Text);
            if (conn is OptionConnection optionConn)
            {
                Debug.Log(" -> Option: " + optionConn.OptionNode.Text);
                if (optionConn.OptionNode.Text == text)
                {
                    return optionConn.OptionNode;
                }
            }
        }
        return null;
    }
}
