using AINavigation;
using Data;
using DG.Tweening;
using Healths;
using SceneManagement;
using TMPro;
using UnityEngine;

namespace Saving
{
    public class SavePoint : MonoBehaviour // TODO rename
    {
        [SerializeField] private ParticleSystem savePointUpdated; // TODO rename
        [SerializeField] public GameObject ChekedSprite;
        [SerializeField] public GameObject LastSprite;
        [SerializeField] public GameObject NotActiveSprite;
        [SerializeField] public Health health;
        [SerializeField] public DataPlayer dataPlayer;
        [SerializeField] public int spawnPoint;

        //[SerializeField] private bool alreadyEnabled; // TODO not used code
        // Переменная для хранения позиции игрока

        private CursorManager _cursorManager;
        private PlayerController _playerController;
        private GameAPI _gameAPI;
        private Camera _camera;
        private Vector3 _playerPosition;

        public void Construct(DataPlayer dataPlayer, Health health, CursorManager cursorManager, PlayerController playerController, GameAPI gameAPI)
        {
            Debug.Log("SavePoint constructed");
            
            _camera = Camera.main;
            this.health = health;
            this.dataPlayer = dataPlayer;
            _cursorManager = cursorManager;
            _playerController = playerController;
            _gameAPI = gameAPI;
            
            SavePointsManager.AllSavePoints[spawnPoint] = this; // TODO change
            NotActiveSprite.SetActive(true);
            
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

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = _camera.ScreenPointToRay(Input.mousePosition); // TODO find change TODO duplicate
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.transform == transform)
                    {
                        Activate();
                    }
                }
            }
        }

        private void OnMouseEnter()
        {
            _cursorManager.SetCursorSave();
        }

        private void OnMouseExit()
        {
            _cursorManager.SetCursorDefault();
        }

        private void Activate()
        {
            //dataPlayer = FindObjectOfType<DataPlayer>(); // TODO find change
            // Сохраняем позицию игрока
            savePointUpdated.gameObject.SetActive(true);
            Debug.Log("включили партикл при сохранении");
            health.Restore();
            dataPlayer.SavePlayerPosition(spawnPoint);

            if (dataPlayer != null)
            {
                while (dataPlayer.PlayerData.stateSpawnPoints.Count < spawnPoint + 1)
                    dataPlayer.PlayerData.stateSpawnPoints.Add(false);
                {
                    dataPlayer.PlayerData.stateSpawnPoints[spawnPoint] = true;
                }

                // Если объект найден, продолжаем с сохранением игры
                _gameAPI.SaveUpdater();
                //StartCoroutine(IGame.Instance.gameAPI.SaveGameData());

                SavePointsManager.UpdateStateSpawnPointsAfterLoad(1,
                    dataPlayer); //Обновляем все метки // TODO savepoints to construct

                _playerController.GetHealth().Restore();

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

        public void SetAlreadyEnabled(bool state, bool thisLast)
        {
            //alreadyEnabled = true;

            ChekedSprite.SetActive(false);
            LastSprite.SetActive(false);
            NotActiveSprite.SetActive(false);

            if (thisLast)
            {
                LastSprite.SetActive(true);
            }
            else if (state)
            {
                ChekedSprite.SetActive(true);
            }
            else
            {
                NotActiveSprite.SetActive(true);
            }
        }

        /*private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == MainPlayer.Instance.gameObject)
        {
            Activate();
        }
    }*/

        private void FindAndFadeFastTextSavePoint(string newText)
        {
            GameObject fastTextSavePoint = FindInactiveObjectByName("FastTextSavePoint"); // TODO find change

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
}