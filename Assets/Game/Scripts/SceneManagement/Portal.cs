using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using RPG.Core;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static LevelChangeObserver;

// Пространство имен для управления сценами игры
namespace RPG.SceneManagement
{
    // Класс для портала, переносящего игрока между сценами
    public class Portal : MonoBehaviour
    {
        [SerializeField] LevelChangeObserver.allScenes sceneToLoad = LevelChangeObserver.allScenes.emptyScene; // Индекс сцены для загрузки
        [SerializeField] private Transform spawnPoint; // Точка спавна в новой сцене
        [SerializeField] private DestinationIdentifier destination; // Идентификатор места назначения портала
        //[SerializeField] private float fadeOutTime = 2f; // Время затухания перед загрузкой новой сцены
        //[SerializeField] private float fadeInTime = 2f; // Время появления после загрузки новой сцены
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
                IGame.Instance.UIManager.HelpInFirstScene.EndStudy5();

                if (sceneToLoad != sceneComponent.IdScene)
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
            SceneLoader.Instance.TryChangeLevel(sceneToLoad);

            //yield return SceneManager.LoadSceneAsync(IGame.Instance.LevelChangeObserver.DAllScenes[sceneToLoad].name); // Загружаем новую сцену


            if (dataPlayer != null)
            {
                List<allScenes> posTempList = new List<allScenes>(IGame.Instance.LevelChangeObserver.DAllScenes.Keys);

                int posNew = 0;
                foreach (var item in posTempList)
                {
                    if (item == (allScenes)sceneComponent.IdScene)
                    {
                        posNew = (int)sceneComponent.IdScene;
                    }
                }


                if (!dataPlayer.playerData.FinishedRegionsIDs.Contains(posNew))
                    dataPlayer.playerData.FinishedRegionsIDs.Add(posNew);

                //if (dataPlayer.playerData.IDmaxRegionAvaliable < posNew)
                //    dataPlayer.playerData.IDmaxRegionAvaliable = posNew;
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
            if (IGame.Instance.dataPLayer.playerData.FinishedRegionsIDs.Contains((int)sceneComponent.IdScene))
                return;

            if (bonusWeapon != null)
            {
                if (IGame.Instance.UIManager.uIBug.AddEquipInBugIfNotExist(IGame.Instance.WeaponArmorManager.TryGetItemByName(bonusWeapon.name)))
                {
                    IGame.Instance.UIManager.ShowNewWeapon();
                    IGame.Instance.gameAPI.SaveUpdater();

                    if (!IGame.Instance.dataPLayer.playerData.alreadyExistWeapons.Contains(bonusWeapon.name))
                    {
                        IGame.Instance.dataPLayer.playerData.alreadyExistWeapons.Add(bonusWeapon.name);
                    }
                }
                else
                {
                    if (bonusWeapon.price > 0)
                        SellItemDisplay(bonusWeapon.price / 4);
                }
            }

            if (bonusArmor != null)
            {
                if (IGame.Instance.UIManager.uIBug.AddEquipInBugIfNotExist(IGame.Instance.WeaponArmorManager.TryGetItemByName(bonusArmor.name)))
                {
                    IGame.Instance.UIManager.ShowNewArmor();
                    IGame.Instance.gameAPI.SaveUpdater();

                    if (!IGame.Instance.dataPLayer.playerData.alreadyExistWeapons.Contains(bonusArmor.name))
                    {
                        IGame.Instance.dataPLayer.playerData.alreadyExistWeapons.Add(bonusArmor.name);
                    }
                }
                else
                {
                    if (bonusArmor.price > 0)
                        SellItemDisplay(bonusArmor.price / 4);
                }
            }
        }

        private void SellItemDisplay(int coins)
        {
            IGame.Instance.CoinManager.Coins.ChangeCount(coins);

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
            messageText.text = "Вы получили бонусные деньги за прохождение " + coins;
            messageText.alignment = TextAlignmentOptions.Center;
            messageText.color = Color.black;
            RectTransform textRectTransform = textObj.GetComponent<RectTransform>();
            textRectTransform.sizeDelta = new Vector2(500, textRectTransform.sizeDelta.y);
            textRectTransform.anchoredPosition = Vector2.zero;

            // Запуск анимации для исчезновения и удаления панели
            DOVirtual.DelayedCall(3, () =>
            {
                // Плавное исчезновение за 2 секунды
                Image panelImage = panel.GetComponent<Image>();
                TextMeshProUGUI textMesh = messageText;

                // Анимация исчезновения панели
                panelImage.DOFade(0, 2);

                // Анимация исчезновения текста
                textMesh.DOFade(0, 2).OnComplete(() =>
                {
                    // Удаление панели после завершения анимации
                    Destroy(panel);
                });
            });
        }
    }
}