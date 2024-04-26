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
        [SerializeField] LevelChangeObserver.allScenes sceneToLoad = LevelChangeObserver.allScenes.emptyScene; // Индекс сцены для загрузки
        [SerializeField] private Transform spawnPoint; // Точка спавна в новой сцене
        [SerializeField] private DestinationIdentifier destination; // Идентификатор места назначения портала
        [SerializeField] private float fadeOutTime = 2f; // Время затухания перед загрузкой новой сцены
        [SerializeField] private float fadeInTime = 2f; // Время появления после загрузки новой сцены
        [SerializeField] private float betweenFadeTime = 2f; // Время ожидания между затуханием и появлением
        [SerializeField] public DataPlayer dataPlayer;
        [SerializeField] public bool ItFinishPortal = true;
        // Статическая переменная, определяющая, идет ли в данный момент переход между сценами
        private SceneComponent sceneComponent;

        [Header("Bonus")]
        [SerializeField] private RPG.Combat.Weapon bonusWeapon;
        [SerializeField] private Armor bonusArmor;


        // Перечисление для идентификации места назначения портала
        enum DestinationIdentifier
        {
            A, B, C, D, E
        }

        private void Awake()
        {
            dataPlayer = FindObjectOfType<DataPlayer>(); // Находит объект DataPlayer в сцене
            sceneComponent = FindObjectOfType<SceneComponent>();

            if (sceneComponent == null)
            {
                Debug.LogError("Errrorr! na scene net SceneComponent.");
            }
            else
            {

            }
        }

        // Обработчик события входа в область портала
        private void OnTriggerEnter(Collider other)
        {
            // Проверяем, что в область портала входит игрок и что переход между сценами не происходит в данный момент
            if (other.gameObject == MainPlayer.Instance.gameObject)
            {
                if ((int)sceneToLoad != IGame.Instance.dataPLayer.playerData.sceneToLoad)
                    StartCoroutine(Transition()); // Запускаем переход между сценами
                else
                {
                    TransitionInScene();
                }
            }
        }

        // Метод для перехода между сценами
        private IEnumerator Transition()
        {
            if (sceneToLoad == LevelChangeObserver.allScenes.emptyScene)
                Debug.LogError("Empty scene on portal. It's mistake");

            dataPlayer.SetSceneToLoad(sceneToLoad);
            SceneLoader.Instance.LoadScene(sceneToLoad);

            //yield return SceneManager.LoadSceneAsync(IGame.Instance.LevelChangeObserver.DAllScenes[sceneToLoad].name); // Загружаем новую сцену


            if (dataPlayer != null)
            {
                if (dataPlayer.playerData.IDmaxRegionAvaliable < (int)sceneComponent.IdScene)
                    dataPlayer.playerData.IDmaxRegionAvaliable = (int)sceneComponent.IdScene;
            }
            else
                Debug.LogError("DataPlayer object not found!"); // Выводит ошибку, если объект DataPlayer не найден в сцене

            SetBonusWeaponAndArmor();
            yield return new WaitForSeconds(betweenFadeTime); // Ждем некоторое время после загрузки сцены
        }

        private void TransitionInScene()
        {
            Portal otherPortal = GetOtherPortal(); // Получаем портал, соответствующий месту назначения текущего портала
            UpdatePlayerLocation(otherPortal); // Обновляем местоположение игрока
        }

        // Метод для получения портала, соответствующего месту назначения текущего портала
        private Portal GetOtherPortal()
        {
            Portal[] portals = FindObjectsOfType<Portal>(); // Находим все порталы в сцене

            // Проходим по всем порталам
            foreach (var portal in portals)
                if (portal != this && portal.destination == this.destination)
                    return portal;

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

        private void SetBonusWeaponAndArmor()
        {
            if (bonusWeapon != null)
                IGame.Instance.playerController.GetFighter().EquipWeapon(bonusWeapon);

            if (bonusArmor != null)
                IGame.Instance.playerController.GetFighter().EquipArmor(bonusArmor);
        }


    }
}