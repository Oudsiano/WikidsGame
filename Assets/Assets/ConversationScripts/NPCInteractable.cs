using System.Collections;
using System.Collections.Generic;
using Core.Quests;
using Data;
using UnityEngine;
using DialogueEditor;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;
using Saving;
using SceneManagement;

public enum InteractableType { Enemy, Item, NPC }

public class NPCInteractable : MonoBehaviour
{
    public InteractableType interactionType;
    public bool posibleInteract = true;
    [HideInInspector]
    public bool NeedKillEnemies = false;
    private ConversationStarter conversationStarter;

    [SerializeField] private List<GameObject> InvisibleWhenCorrectAnswer = new List<GameObject>();

    private RaycastHit hit;
    private CursorManager _cursorManager;
    private Camera _camera;

    public void Construct(CursorManager cursorManager, QuestManager questManager, DataPlayer dataPlayer, GameAPI gameAPI)
    {
        _cursorManager = cursorManager;
        _camera = Camera.main;
        conversationStarter = GetComponentInParent<ConversationStarter>();
    }

    private void OnMouseEnter()
    {
        _cursorManager.SetCursorManuscript();
    }
    
    private void OnMouseExit()
    {
        _cursorManager.SetCursorDefault();
    }

    private void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
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
        if (InvisibleWhenCorrectAnswer.Count > 0)
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
        if (!posibleInteract) return;

        if (NeedKillEnemies)
        {
            ShowNeedKillEnemies("Победите врагов (на них указывает линия), чтобы начать разговор");
            return;
        }

        Debug.Log("Interacting with NPC");

        // Вызов метода StartDialog() из ConversationStarter, если компонент присутствует 
        if (conversationStarter != null)
        {
            conversationStarter.StartDialog();

            NPC_for_testID _npc = conversationStarter.myConversation.GetComponent<NPC_for_testID>();
            if (_npc != null)
                _npc.SetParent(gameObject);
        }
    }

    public void InteractWithItem()
    {
        // Pickup Item 
        Destroy(gameObject);
    }

    private void ShowNeedKillEnemies(string text)
    {

        TextMeshProUGUI messageText;
        Canvas canvas;
        GameObject panel;

        // Создание Canvas
        GameObject canvasObj = new GameObject("Canvas");
        canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 100; // Установка sortOrder
        canvasObj.AddComponent<CanvasScaler>();
        canvasObj.AddComponent<GraphicRaycaster>();

        // Создание панели
        panel = new GameObject("MessagePanel");
        panel.transform.SetParent(canvas.transform, false);
        RectTransform rectTransform = panel.AddComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(800, 300);
        panel.AddComponent<CanvasRenderer>();
        Image image = panel.AddComponent<Image>();
        image.color = new Color(0.5f, 0.5f, 0.5f, 0.5f); // Серый полупрозрачный цвет

        // Создание текста
        GameObject textObj = new GameObject("MessageText");
        textObj.transform.SetParent(panel.transform, false);
        messageText = textObj.AddComponent<TextMeshProUGUI>();
        messageText.text = text;
        messageText.alignment = TextAlignmentOptions.Center;
        messageText.color = Color.black;
        RectTransform textRectTransform = textObj.GetComponent<RectTransform>();
        textRectTransform.sizeDelta = new Vector2(500, textRectTransform.sizeDelta.y);
        textRectTransform.anchoredPosition = Vector2.zero;

        // Запуск анимации для исчезновения и удаления панели
        DOVirtual.DelayedCall(2, () =>
        {
            // Плавное исчезновение за 2 секунды
            Image panelImage = panel.GetComponent<Image>();
            TextMeshProUGUI textMesh = messageText;

            // Анимация исчезновения панели
            panelImage.DOFade(0, 1);

            // Анимация исчезновения текста
            textMesh.DOFade(0, 1).OnComplete(() =>
            {
                // Удаление панели после завершения анимации
                Destroy(panel);
            });
        });
    }
}