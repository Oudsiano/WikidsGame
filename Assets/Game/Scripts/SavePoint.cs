using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using RPG.Core;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SavePointsManager
{
    private static Dictionary<int, SavePoint> allSavePoints;
    public static Dictionary<int, SavePoint> AllSavePoints
    {
        get
        {
            if (allSavePoints == null) allSavePoints = new Dictionary<int, SavePoint>();
            return allSavePoints;
        }
        set => allSavePoints = value;
    }

    public static void UpdateStateSpawnPointsAfterLoad(DataPlayer dataPlayer, bool reset = false)
    {

        for (int i = 0; i < dataPlayer.playerData.stateSpawnPoints.Count; i++)
        {
            bool thisLast = i == dataPlayer.playerData.spawnPoint;
            if (allSavePoints.Count > i)
                if (allSavePoints[i] != null)
                    allSavePoints[i].SetAlreadyEnabled(dataPlayer.playerData.stateSpawnPoints[i], thisLast);
        }
    }

    public static void ResetDict()
    {
        allSavePoints = new Dictionary<int, SavePoint>();
        IGame.Instance.dataPLayer.playerData.stateSpawnPoints = new List<bool> { false };
    }

}

public class SavePoint : MonoBehaviour
{
    [SerializeField] ParticleSystem savePointUpdated;
    [SerializeField] public GameObject ChekedSprite;
    [SerializeField] public GameObject LastSprite;
    [SerializeField] public GameObject NotActiveSprite;
    [SerializeField] public Health health;
    [SerializeField] public DataPlayer dataPlayer;
    [SerializeField] public int spawnPoint;
    //[SerializeField] private bool alreadyEnabled;
    // Переменная для хранения позиции игрока
    private Vector3 playerPosition;

    public void SetAlreadyEnabled(bool state, bool thisLast)
    {
        //alreadyEnabled = true;

        ChekedSprite.SetActive(false);
        LastSprite.SetActive(false);
        NotActiveSprite.SetActive(false);

        if (thisLast)
            LastSprite.SetActive(true);
        else if (state)
            ChekedSprite.SetActive(true);
        else
            NotActiveSprite.SetActive(true);
    }

    private void Awake()
    {
        SavePointsManager.AllSavePoints[spawnPoint] = this;
        NotActiveSprite.SetActive(true);
        health = FindObjectOfType<Health>();

        if (transform.localScale != Vector3.one)
        {
            Debug.LogError("Scale is not (1, 1, 1)");
        }

        BoxCollider collider = GetComponent<BoxCollider>();
        if (collider != null && collider.size != Vector3.one)
        {
            Debug.LogError("Box Collider size is not (1, 1, 1)");
        }
    }
    void Update()
    {
        // Проверяем, был ли произведен клик
        if (Input.GetMouseButtonDown(0))
        {
            // Создаем луч из точки на экране в мир
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // Проверяем, попал ли луч в какой-то объект с коллайдером
            if (Physics.Raycast(ray, out hit))
            {
                // Проверяем, этот ли объект был нажат
                if (hit.transform == transform)
                {
                    // Действия при клике на объект
                    Activate();
                }
            }
        }
    }



    /*private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == MainPlayer.Instance.gameObject)
        {
            Activate();
        }
    }*/


    private void Activate()
    {
        dataPlayer = FindObjectOfType<DataPlayer>(); // Находит объект DataPlayer в сцене
                                                     // Сохраняем позицию игрока
        savePointUpdated.gameObject.SetActive(true);
        Debug.Log("включили партикл при сохранении");
        health.Restore();
        dataPlayer.SavePlayerPosition(spawnPoint);

        if (dataPlayer != null)
        {
            while (dataPlayer.playerData.stateSpawnPoints.Count < spawnPoint + 1) dataPlayer.playerData.stateSpawnPoints.Add(false);
            dataPlayer.playerData.stateSpawnPoints[spawnPoint] = true;

            // Если объект найден, продолжаем с сохранением игры
            IGame.Instance.gameAPI.SaveUpdater();
            //StartCoroutine(IGame.Instance.gameAPI.SaveGameData());

            SavePointsManager.UpdateStateSpawnPointsAfterLoad(dataPlayer); //Обновляем все метки

            IGame.Instance.playerController.GetHealth().Restore();

            gameObject.GetComponent<Collider>().enabled = false;
            showText("Активированна точка сохранения!");
        }
        else
        {
            // Если объект не найден, выводим ошибку
            Debug.LogError("DataPlayer object not found!");
        }
        // Дополнительно можно сохранить другие параметры игрока, такие как например его здоровье, экипировку и т.д.
    }


    // Метод для получения сохранённой позиции игрока

    private void showText(string text)
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
        rectTransform.anchoredPosition = new Vector2(250, -25);
        rectTransform.sizeDelta = new Vector2(300, 150);
        panel.AddComponent<CanvasRenderer>();
        Image image = panel.AddComponent<Image>();
        image.color = new Color(0.6745f, 0.6980f, 0.5569f, 0.5f);

        // Создание текста
        GameObject textObj = new GameObject("MessageText");
        textObj.transform.SetParent(panel.transform, false);
        messageText = textObj.AddComponent<TextMeshProUGUI>();
        messageText.text = text;
        messageText.fontSize = 20;
        messageText.alignment = TextAlignmentOptions.Center;
        messageText.color = Color.black;
        RectTransform textRectTransform = textObj.GetComponent<RectTransform>();
        textRectTransform.sizeDelta = new Vector2(250, textRectTransform.sizeDelta.y);
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
