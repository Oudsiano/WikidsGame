using System.Collections;
using System.Collections.Generic;
using Combat.Data;
using Core.Player;
using Data;
using DG.Tweening;
using NaughtyAttributes;
using Saving;
using SceneManagement.Enums;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using Utils;

namespace SceneManagement
{
    public class Portal : MonoBehaviour // TODO Restruct
    {
        [Dropdown("GetSceneNames")][SerializeField] private string sceneToLoad; // имя сцены для загрузки

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
        private CursorManager _cursorManager;
        private UIManager _uiManager;
        private NPCManagment _npcManagment;
        private SaveGame _saveGame;
        private CoinManager _coinManager;
        private LevelChangeObserver _levelChangeObserver;

        public void Construct(MainPlayer player, DataPlayer dataPlayer, SceneComponent sceneComponent,
            CursorManager cursorManager, UIManager uiManager, NPCManagment npcManagment, SaveGame saveGame, CoinManager coinManager, LevelChangeObserver levelChangeObserver)
        {
            _player = player;
            _cursorManager = cursorManager;
            _uiManager = uiManager;
            _npcManagment = npcManagment;
            _saveGame = saveGame;
            _coinManager = coinManager;

            this.dataPlayer = dataPlayer;
            this.sceneComponent = sceneComponent;
            _levelChangeObserver = levelChangeObserver;
        }
        
        private void OnMouseEnter()
        {
            _cursorManager.SetCursorExit();
        }

        private void OnMouseExit()
        {
            _cursorManager.SetCursorDefault();
        }

        private void OnTriggerEnter(Collider other)
        {
            // Проверяем, что в область портала входит игрок и что переход между сценами не происходит в данный момент
            if (other.gameObject == _player.gameObject)
            {
                _uiManager.HelpInFirstScene.EndStudy5();

                // if (_npcManagment.checkAllTestsComplite() == false)
                // {
                //     Debug.Log("Рано портироваться, ты еще не сделал все тесты" + // TODO can be cached
                //               string.Join(", ", _npcManagment.NotComplete));
                //     TextDisplay(0, "Рано портироваться, ты еще не сделал все тесты"); // TODO can be cached
                //
                //     return;
                // }

                if (sceneToLoad != Constants.Scenes.OpenScene)
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
            dataPlayer.SetSceneToLoad(sceneToLoad);
            _levelChangeObserver.TryChangeLevel(sceneToLoad, 0);

            _cursorManager.SetCursorDefault();
            _saveGame.MakePortalSave(bonusWeapon, bonusArmor, sceneComponent);

            yield return new WaitForSeconds(betweenFadeTime);
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
            _coinManager.Coins.ChangeCount(coins);

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
        
        private List<string> GetSceneNames()
        {
            return new List<string>
            {
                Constants.Scenes.BootstrapScene,
                Constants.Scenes.OpenScene,
                Constants.Scenes.MapScene,
                Constants.Scenes.FirstTownScene,
                Constants.Scenes.FirstBattleScene,
                Constants.Scenes.SecondBattleScene,
                Constants.Scenes.ThirdBattleScene,
                Constants.Scenes.FourthBattleSceneDark,
                Constants.Scenes.FifthBattleSceneKingdom,
                Constants.Scenes.SixthBattleSceneKingdom,
                Constants.Scenes.SeventhBattleSceneViking,
                Constants.Scenes.BossFightDarkScene,
                Constants.Scenes.BossFightKingdom1Scene,
                Constants.Scenes.BossFightKingdom2Scene,
                Constants.Scenes.BossFightViking1Scene,
                Constants.Scenes.LibraryScene,
                Constants.Scenes.HollScene,
                Constants.Scenes.EndScene
            };
        }
    }
}