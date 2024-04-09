using System.Collections;
using RPG.Core;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

// Пространство имен для управления сценами игры
namespace RPG.SceneManagement
{
    // Класс для портала, переносящего игрока между сценами
    public class Portal : MonoBehaviour
    {
        [SerializeField] int sceneToLoad = -1; // Индекс сцены для загрузки
        [SerializeField] private Transform spawnPoint; // Точка спавна в новой сцене
        [SerializeField] private DestinationIdentifier destination; // Идентификатор места назначения портала
        [SerializeField] private float fadeOutTime = 2f; // Время затухания перед загрузкой новой сцены
        [SerializeField] private float fadeInTime = 2f; // Время появления после загрузки новой сцены
        [SerializeField] private float betweenFadeTime = 2f; // Время ожидания между затуханием и появлением
        [SerializeField] public DataPlayer dataPlayer;
        // Статическая переменная, определяющая, идет ли в данный момент переход между сценами
        private static bool isTransitioning = false;

        // Перечисление для идентификации места назначения портала
        enum DestinationIdentifier
        {
            A, B, C, D, E
        }

        // Обработчик события входа в область портала
        private void OnTriggerEnter(Collider other)
        {
            // Проверяем, что в область портала входит игрок и что переход между сценами не происходит в данный момент
            if (other.gameObject == MainPlayer.Instance.gameObject && !isTransitioning)
            {
                StartCoroutine(Transition()); // Запускаем переход между сценами
                isTransitioning = true; // Устанавливаем флаг перехода в состояние "идет переход"
            }
        }

        // Метод для перехода между сценами
        private IEnumerator Transition()
        {
            DontDestroyOnLoad(this.gameObject); // Не уничтожаем портал при загрузке новой сцены



            yield return SceneManager.LoadSceneAsync(sceneToLoad); // Загружаем новую сцену


            
            if (dataPlayer != null)
            {
                int newSceneNumber = sceneToLoad; // Новое значение номера локации
                dataPlayer.SetSceneToLoad(newSceneNumber); // Устанавливает новое значение номера локациb

            }
            else
            {
                Debug.LogError("DataPlayer object not found!"); // Выводит ошибку, если объект DataPlayer не найден в сцене
            }
            Portal otherPortal = GetOtherPortal(); // Получаем портал, соответствующий месту назначения текущего портала
            UpdatePlayerLocation(otherPortal); // Обновляем местоположение игрока
            yield return new WaitForSeconds(betweenFadeTime); // Ждем некоторое время после загрузки сцены
            isTransitioning = false; // Устанавливаем флаг перехода в состояние "переход завершен"
            dataPlayer = FindObjectOfType<DataPlayer>(); // Находит объект DataPlayer в сцене
            Destroy(this.gameObject); // Уничтожаем портал
        }

        // Метод для получения портала, соответствующего месту назначения текущего портала
        private Portal GetOtherPortal()
        {
            Portal[] portals = FindObjectsOfType<Portal>(); // Находим все порталы в сцене

            // Проходим по всем порталам
            foreach (var portal in portals)
            {
                // Если находим портал с таким же местом назначения, но не текущий портал, возвращаем его
                if (portal != this && portal.destination == this.destination)
                {
                    return portal;
                }
            }

            return null; // Если портал не найден, возвращаем null
        }

        // Метод для обновления местоположения игрока
        private void UpdatePlayerLocation(Portal otherPortal)
        {
            // Отключаем навигацию для игрока
            MainPlayer.Instance.gameObject.GetComponent<NavMeshAgent>().enabled = false;

            // Устанавливаем позицию и поворот игрока в соответствии с порталом назначения
            MainPlayer.Instance.transform.position = otherPortal.spawnPoint.position;
            MainPlayer.Instance.transform.rotation = otherPortal.spawnPoint.rotation;

            // Включаем навигацию для игрока
            MainPlayer.Instance.gameObject.GetComponent<NavMeshAgent>().enabled = true;
            
        }

       
        
    }
}