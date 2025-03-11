using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DialogueEditor;

public class DialogChanger : MonoBehaviour
{
    [SerializeField] private Button _nextButton;
    [SerializeField] private Button _previousButton;
    [SerializeField] private NPCConversation _startConversation;
    
    private int index = 2;
    
    private void OnEnable()
    {
        _nextButton.onClick.AddListener(GoNext);
        _previousButton.onClick.AddListener(GoBack);
    }

    private void OnDisable()
    {
        _nextButton.onClick.RemoveListener(GoNext);
        _previousButton.onClick.RemoveListener(GoBack);
    }

    private void GoNext()
    {
        index++; 
        
        if (index == 4)
        {
            index = 5;
        }

        if (index > 5)
        {
            ConversationManager.Instance.SpeechSelected(GetSpeechNodeByText(_startConversation,"Игра позволяет вам не только погрузиться в захватывающий мир магии и приключений, но и развивать свой интеллект, проходя разнообразные образовательные тесты. Ваша мудрость и сила станут ключом к победе над злом и спасению мира от нависшей угрозы."));
            ConversationManager.Instance.PressSelectedOption();
        }
        else
        {
            ConversationManager.Instance.SpeechSelected(GetSpeechNodeByIndex(index));
            ConversationManager.Instance.PressSelectedOption();
        }

        Debug.Log("Index"+ index);
    }

    private void GoBack()
    {
        index--;
        
        if (index == 4)
        {
            index = 3;
        }
        
        Debug.Log("Index"+ index);
        ConversationManager.Instance.SpeechSelected(GetSpeechNodeByIndex(index));
        ConversationManager.Instance.PressSelectedOption();
    }
    
    private SpeechNode GetSpeechNodeByIndex( int targetIndex)
    {
        var conversation = _startConversation.Deserialize();
        var root = conversation.Root;

        Queue<ConversationNode> queue = new Queue<ConversationNode>();
        HashSet<ConversationNode> visited = new HashSet<ConversationNode>();
        List<SpeechNode> speechNodes = new List<SpeechNode>();

        queue.Enqueue(root);
        visited.Add(root);

        while (queue.Count > 0)
        {
            ConversationNode current = queue.Dequeue();

            if (current is SpeechNode speechNode)
            {
                speechNodes.Add(speechNode);
            }

            foreach (var conn in current.Connections)
            {
                ConversationNode next = null;

                if (conn is SpeechConnection speechConn)
                    next = speechConn.SpeechNode;
                else if (conn is OptionConnection optionConn)
                    next = optionConn.OptionNode;

                if (next != null && !visited.Contains(next))
                {
                    visited.Add(next);
                    queue.Enqueue(next);
                }
            }
        }

        if (targetIndex >= 0 && targetIndex < speechNodes.Count)
        {
            Debug.Log(" -> Speech: " + speechNodes[targetIndex].Text);
            return speechNodes[targetIndex];
        }

        return null;
    }
    
    private SpeechNode GetSpeechNodeByText(NPCConversation npcConversation, string text)
    {
        var conversation = npcConversation.Deserialize();
        var root = conversation.Root;

        foreach (var conn in root.Connections)
        {
            if (conn is SpeechConnection speechConn )
            {
                Debug.Log(" -> Option: " + speechConn.SpeechNode.Text);
                if (speechConn.SpeechNode.Text == text)
                {
                    return speechConn.SpeechNode;
                }
            }
        }
        return null;
    }
}
