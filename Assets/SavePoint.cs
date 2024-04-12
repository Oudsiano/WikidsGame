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

        public static void UpdateStateSpawnPointsAfterLoad(DataPlayer dataPlayer)
        {
            for (int i = 0; i < dataPlayer.playerData.stateSpawnPoints.Count - 1; i++)
            {
                allSavePoints[i].AlreadyEnabled = dataPlayer.playerData.stateSpawnPoints[i];
            }
        }
    }

    public class SavePoint : MonoBehaviour
    {
        [SerializeField] public DataPlayer dataPlayer;
        [SerializeField] public int spawnPoint;
        [SerializeField] public GameAPI api;
        [SerializeField] private bool alreadyEnabled;
        // Переменная для хранения позиции игрока
        private Vector3 playerPosition;

        public bool AlreadyEnabled
        {
            get => alreadyEnabled;
            set
            {


                alreadyEnabled = value;
            }
        }

        private void Awake()
        {
            SavePointsManager.AllSavePoints[spawnPoint] = this;
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

                dataPlayer.SavePlayerPosition(spawnPoint);
                api = FindObjectOfType<GameAPI>(); // Попытка найти объект DataPlayer в сцене

                if (dataPlayer != null)
                {
                    AlreadyEnabled = true;
                    while (dataPlayer.playerData.stateSpawnPoints.Count < spawnPoint) dataPlayer.playerData.stateSpawnPoints.Add(false);
                    dataPlayer.playerData.stateSpawnPoints[spawnPoint] = true;

                    // Если объект найден, продолжаем с сохранением игры
                    StartCoroutine(api.SaveGameData(dataPlayer.playerData));
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
