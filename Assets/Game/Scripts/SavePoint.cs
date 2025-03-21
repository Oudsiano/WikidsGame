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
        if (allSavePoints == null) return;
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
        IGame.Instance.dataPlayer.playerData.stateSpawnPoints = new List<bool> { false };
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

    void OnMouseEnter()
    {
        IGame.Instance.CursorManager.SetCursorSave();
    }
    private void OnMouseExit()
    {
        IGame.Instance.CursorManager.SetCursorDefault();
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

            //gameObject.GetComponent<Collider>().enabled = false;
            //if (spawnPoint!=0)
            FindAndFadeFastTextSavePoint("Активированна точка сохранения!");
        }
        else
        {
            // Если объект не найден, выводим ошибку
            Debug.LogError("DataPlayer object not found!");
        }
        // Дополнительно можно сохранить другие параметры игрока, такие как например его здоровье, экипировку и т.д.
    }


    // Метод для получения сохранённой позиции игрока

    public void FindAndFadeFastTextSavePoint(string newText)
    {
        GameObject fastTextSavePoint = FindInactiveObjectByName("FastTextSavePoint");

        if (fastTextSavePoint != null)
        {
            Transform messageTextTransform = fastTextSavePoint.transform.Find("MessageText");
            if (messageTextTransform != null)
            {
                TextMeshProUGUI textMeshPro = messageTextTransform.GetComponent<TextMeshProUGUI>();
                if (textMeshPro != null)
                {
                    textMeshPro.text = newText;
                }
            }

            CanvasGroup canvasGroup = fastTextSavePoint.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = fastTextSavePoint.AddComponent<CanvasGroup>();
            }

            canvasGroup.DOKill();

            fastTextSavePoint.SetActive(true);
            canvasGroup.alpha = 1; // Ensure alpha is reset to 1 before fading
            canvasGroup.DOFade(0, 1).SetDelay(2).OnComplete(() =>
            {
                fastTextSavePoint.SetActive(false);
                canvasGroup.alpha = 1; // Reset alpha for next use
            });
        }
    }

    private GameObject FindInactiveObjectByName(string name)
    {
        Transform[] allObjects = Resources.FindObjectsOfTypeAll<Transform>();
        foreach (Transform obj in allObjects)
        {
            if (obj.name == name && obj.hideFlags == HideFlags.None)
            {
                return obj.gameObject;
            }
        }
        return null;
    }
}
