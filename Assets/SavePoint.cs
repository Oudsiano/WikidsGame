using System.Collections;
using RPG.Core;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

namespace RPG.SceneManagement
{
    public class SavePoint : MonoBehaviour
    {
        [SerializeField] public DataPlayer dataPlayer;
        [SerializeField] public int spawnPoint;
        [SerializeField] public GameAPI api;
        // Переменная для хранения позиции игрока
        private Vector3 playerPosition;

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
