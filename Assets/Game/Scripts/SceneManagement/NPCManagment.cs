using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCManagment : MonoBehaviour
{
    private Dictionary<int, GameObject> _dicNPCTests;

    // Start is called before the first frame update
    void Start()
    {
        //Need find all NPC

        ConversationStarter[] conversationStarters = FindObjectsOfType<ConversationStarter>();

        foreach (ConversationStarter item in conversationStarters)
        {
            if (item.TestID>0)
            {
                _dicNPCTests[item.TestID] = item.gameObject;
            }
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
