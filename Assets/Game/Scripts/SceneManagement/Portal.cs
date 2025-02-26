using System.Collections;
using Combat.Data;
using Core.Player;
using Data;
using DG.Tweening;
using SceneManagement.Enums;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

// Пространство имен для управления сценами игры
namespace SceneManagement
{
    // Класс для портала, переносящего игрока между сценами
    public class Portal : MonoBehaviour // TODO Restruct
    {
        [SerializeField]
        private allScenes sceneToLoad = allScenes.emptyScene; // Индекс сцены для загрузки // TODO rename

        [SerializeField] private Transform spawnPoint; // Точка спавна в новой сцене // TODO rename

        [SerializeField]
        private DestinationIdentifier destination; // Идентификатор места назначения портала // TODO rename

        //[SerializeField] private float fadeOutTime = 2f; // Время затухания перед загрузкой новой сцены
        //[SerializeField] private float fadeInTime = 2f; // Время появления после загрузки новой сцены
        [SerializeField]
        private float betweenFadeTime = 2f; // Время ожидания между затуханием и появлением // TODO rename

        [SerializeField] public DataPlayer dataPlayer; // TODO rename

        [SerializeField] public bool ItFinishPortal = true; // TODO rename

        // Статическая переменная, определяющая, идет ли в данный момент переход между сценами
        private SceneComponent sceneComponent; // TODO rename

        [Header("Bonus")] [SerializeField] private Weapon bonusWeapon; // TODO rename
        [SerializeField] private Armor bonusArmor; // TODO rename

        private MainPlayer _player;
        
        public void Construct(MainPlayer player, DataPlayer dataPlayer, SceneComponent sceneComponent) // TODO construct
        {
            _player = player;
            this.dataPlayer = dataPlayer;
            this.sceneComponent = sceneComponent;

            if (sceneComponent == null)
            {
                Debug.LogError("Errrorr! na scene net SceneComponent.");
            }
        }

        private void OnMouseEnter()
        {
            IGame.Instance.CursorManager.SetCursorExit();
        }

        private void OnMouseExit()
        {
            IGame.Instance.CursorManager.SetCursorDefault();
        }

        private void OnTriggerEnter(Collider other)
        {
            // Проверяем, что в область портала входит игрок и что переход между сценами не происходит в данный момент
            if (other.gameObject == _player.gameObject) // TODO change
            {
                IGame.Instance._uiManager.HelpInFirstScene.EndStudy5();

                if (IGame.Instance.NPCManagment.checkAllTestsComplite() == false)
                {
                    Debug.Log("Рано портироваться, ты еще не сделал все тесты" + // TODO can be cached
                              string.Join(", ", IGame.Instance.NPCManagment.NotComplete));
                    TextDisplay(0, "Рано портироваться, ты еще не сделал все тесты"); // TODO can be cached

                    return;
                }

                if (sceneToLoad != sceneComponent.IdScene)
                {
                    StartCoroutine(Transition()); // Запускаем переход между сценами
                }
                else
                {
                    TransitionInScene();
                }
            }
        }

        private IEnumerator Transition()
        {
            if (sceneToLoad == allScenes.emptyScene)
            {
                Debug.LogError("Empty scene on portal. It's mistake");
            }

            dataPlayer.SetSceneToLoad(sceneToLoad);
            SceneLoader.Instance.TryChangeLevel(sceneToLoad, 0);

            IGame.Instance.CursorManager.SetCursorDefault();
            IGame.Instance.saveGame.MakePortalSave(bonusWeapon, bonusArmor, sceneComponent);

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
            {
                if (portal != this && portal.destination == destination)
                {
                    return portal;
                }
            }

            return null; // Если портал не найден, возвращаем null
        }
        
        private void UpdatePlayerLocation(Portal otherPortal)
        {
            _player.gameObject.GetComponent<NavMeshAgent>().enabled = false;
            
            _player.transform.position = otherPortal.spawnPoint.position;
            _player.transform.rotation = otherPortal.spawnPoint.rotation;
            
            _player.gameObject.GetComponent<NavMeshAgent>().enabled = true;
        }

        private void TextDisplay(int coins, string text) // TODO move Factory canvas
        {
            IGame.Instance._coinManager.Coins.ChangeCount(coins);

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
            if (coins != 0)
                messageText.text = text + " " + coins;
            else
                messageText.text = text;
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