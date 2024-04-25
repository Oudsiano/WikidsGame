using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DialogueEditor;


public enum InteractableType { Enemy, Item, NPC }

public class NPCInteractable : MonoBehaviour
{

    public InteractableType interactionType;

    private ConversationStarter conversationStarter;

    [SerializeField] private List<GameObject> InvisibleWhenCorrectAnswer= new List<GameObject>();

    RaycastHit hit;

    void Awake()
    {
        conversationStarter = GetComponentInParent<ConversationStarter>();

    }

    void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                if (hit.transform.gameObject == gameObject)
                    if (!ConversationManager.Instance.IsConversationActive)
                        InteractWithNPC();
            }
        }
    }

    public void MakeInvisibleWhenCorrectAnswer()
    {
        if (InvisibleWhenCorrectAnswer.Count>0)
        {
            foreach (GameObject item in InvisibleWhenCorrectAnswer)
            {
                item.SetActive(false);
            }
        }
        else
        {
            Debug.LogWarning("error. expected some elements in list");
        }
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