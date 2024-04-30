using System.Collections;
using System.Collections.Generic;
using RPG.Core;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

namespace RPG.SceneManagement
{
    public class SavePointsManager
    {
        private Dictionary<int, SavePoint> allSavePoints;
        public Dictionary<int, SavePoint> AllSavePoints
        {
            get
            {
                if (allSavePoints == null) allSavePoints = new Dictionary<int, SavePoint>();
                return allSavePoints;
            }
            set => allSavePoints = value;
        }

        public void UpdateStateSpawnPointsAfterLoad(DataPlayer dataPlayer, bool reset = false)
        {

            for (int i = 0; i < dataPlayer.playerData.stateSpawnPoints.Count; i++)
            {
                bool thisLast = i == dataPlayer.playerData.spawnPoint;
                if (allSavePoints.Count> i) 
                    if(allSavePoints[i]!=null)
                allSavePoints[i].SetAlreadyEnabled(dataPlayer.playerData.stateSpawnPoints[i], thisLast);
            }
        }

        public void ResetDict()
        {
            allSavePoints = new Dictionary<int, SavePoint>();
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
            IGame.Instance.SavePointsManager.AllSavePoints[spawnPoint] = this;
            NotActiveSprite.SetActive(true);
            health = FindObjectOfType<Health>();
        }

        // Обработчик события входа в область портала
        private void OnTriggerEnter(Collider other)
        {
            // Проверяем, что в область портала входит игрок и что переход между сценами не происходит в данный момент
            if (other.gameObject == MainPlayer.Instance.gameObject)
            {
                playerPosition = other.transform.position;
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
                    StartCoroutine(IGame.Instance.gameAPI.SaveGameData(dataPlayer.playerData));

                    IGame.Instance.SavePointsManager.UpdateStateSpawnPointsAfterLoad(dataPlayer); //Обновляем все метки

                    IGame.Instance.playerController.GetHealth().Restore();
                }
                else
                {
                    // Если объект не найден, выводим ошибку
                    Debug.LogError("DataPlayer object not found!");
                }
                // Дополнительно можно сохранить другие параметры игрока, такие как например его здоровье, экипировку и т.д.
            }
        }

        // Метод для получения сохранённой позиции игрока

    }
}
