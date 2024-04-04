using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DialogueEditor;


public enum InteractableType { Enemy, Item, NPC }

public class NPCInteractable : MonoBehaviour
{

    public InteractableType interactionType;

    private ConversationStarter conversationStarter;



    void Awake()
    {
        conversationStarter = GetComponentInParent<ConversationStarter>();

    }

    public void InteractWithNPC()
    {
        Debug.Log("Interacting with NPC");

        // Вызов метода StartDialog() из ConversationStarter, если компонент присутствует 
        if (conversationStarter != null)
        {
            conversationStarter.StartDialog();
        }
    }

    public void InteractWithItem()
    {
        // Pickup Item 
        Destroy(gameObject);
    }
}