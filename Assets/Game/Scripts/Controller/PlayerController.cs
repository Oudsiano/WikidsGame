using UnityEngine;
using UnityEngine.EventSystems;
using RPG.Movement;
using RPG.Combat;
using RPG.Core;
using DialogueEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

namespace RPG.Controller
{
    // Класс, управляющий игроком
    public class PlayerController : MonoBehaviour
    {
        // Ссылки на компоненты
        private Fighter fighter; // Компонент, отвечающий за атакующее поведение игрока
        private Mover mover; // Компонент, отвечающий за перемещение игрока
        private Health health; // Компонент, отвечающий за здоровье игрока

        public PlayerArmorManager playerArmorManager;

        public WeaponPanelUI WeaponPanelUI;

        public GameObject modularCharacter;

        [SerializeField] Image canvasHelmet;
        [SerializeField] float startDistanceForShowIcon = 125;

        private int enemyLayer = 9; // Номер слоя для врагов

        private List<Fighter> allEnemyes;
        private bool _currentInvisState = false; //невидимость(Стелс) выключена


        // Метод Start вызывается перед первым обновлением кадра
        public void Init()
        {
            // Получаем ссылки на компоненты
            mover = GetComponent<Mover>();
            fighter = GetComponent<Fighter>();
            health = GetComponent<Health>();
            playerArmorManager = FindObjectOfType<PlayerArmorManager>();


            WeaponPanelUI = FindObjectOfType<WeaponPanelUI>();

            WeaponPanelUI.Init();

            RPG.Core.SceneLoader.LevelChanged += SceneLoader_LevelChanged;
            IGame.Instance.saveGame.OnLoadItems += SaveGame_OnOnLoadItems;

            FollowCamera.OnCameraDistance += FollowCamera_OnCameraDistance;
        }

        private void FollowCamera_OnCameraDistance(float obj)
        {
            if (obj > startDistanceForShowIcon)
            {
                canvasHelmet.gameObject.SetActive(true);
                Color newColor = canvasHelmet.color;
                newColor.a = Mathf.Min(((obj- startDistanceForShowIcon) /50f),1);
                canvasHelmet.color = newColor;
            }
            else
            {
                canvasHelmet.gameObject.SetActive(false);
            }
        }

        private void SaveGame_OnOnLoadItems()
        {
            fighter.EquipItem(IGame.Instance.saveGame.EquipedArmor);
            fighter.EquipItem(IGame.Instance.saveGame.EquipedWeapon);
        }

        private void SceneLoader_LevelChanged(LevelChangeObserver.allScenes obj)
        {
            IGame.Instance.saveGame.MakeLoad();

            //Начало работы над автоатакой. Типа сначала получаем всех врагов, а потом будем смотреть, есть ли кто рядом. Но сейчас пока задача отложенна ради более важных
            //allEnemyes = new List<Fighter>();
            //allEnemyes = FindObjectsOfType<Fighter>().ToList();

            EquipWeaponAndArmorAfterLoad();
        }

        public Health GetHealth() => health;
        public Fighter GetFighter() => fighter;


        public bool GetPlayerInvis()
        {
            return _currentInvisState;
        }

        public void SetInvisByBtn(bool invis)
        {
            if (_currentInvisState==invis) return;
            _currentInvisState = invis;


        }
        

        public void EquipWeaponAndArmorAfterLoad()
        {
            if (IGame.Instance.dataPLayer.playerData.weaponToLoad.Length > 1)
            {
                fighter.EquipWeapon(IGame.Instance.WeaponArmorManager.TryGetWeaponByName(IGame.Instance.dataPLayer.playerData.weaponToLoad));
            }
            IGame.Instance.WeaponArmorManager.GerArmorById((armorID)IGame.Instance.dataPLayer.playerData.armorIdToload).EquipIt();
        }

        // Метод Update вызывается один раз за кадр
        void Update()
        {
            if (IGame.Instance.IsPause) return;

            // Если игрок мертв, прекращаем выполнение метода
            if (health.IsDead())
                return;

            // Пытаемся взаимодействовать с боем
            if (InteractWithCombat())
                return;

            // Пытаемся взаимодействовать с клавиатурой
            InteractWithKeyboard();

            // Пытаемся взаимодействовать с перемещением
            if (InteractWithMovement())
                return;
        }

        // Метод для взаимодействия с клавиатурой
        private void InteractWithKeyboard()
        {
            // Если нажата клавиша D, снимаем оружие
            if (Input.GetKeyDown(KeyCode.D))
            {
                fighter.UnequipWeapon();
            }
        }

        // Метод для взаимодействия с боем
        private bool InteractWithCombat()
        {
            // Применяем побитовую операцию, чтобы получить только слой врагов
            int layerMask = 1 << enemyLayer;

            // Получаем луч из мыши
            var ray = GetMouseRay();

            // Проводим луч в мире и получаем результаты
            RaycastHit[] hits;
            hits = Physics.RaycastAll(ray, Mathf.Infinity, layerMask);

            // Перебираем все попадания
            foreach (var enemyHit in hits)
            {
                CombatTarget target = enemyHit.transform.gameObject.GetComponent<CombatTarget>();

                // Если цель не существует или игрок не может атаковать выбранного противника, продолжаем цикл
                if (!target || !fighter.CanAttack(target.gameObject))
                    continue;

                // Если игрок кликнул мышью, атакуем цель
                if (Input.GetMouseButtonDown(0))
                {
                    fighter.Attack(target.gameObject);
                }

                return true;
            }

            return false;
        }

        // Метод для взаимодействия с перемещением
        private bool InteractWithMovement()
        {
            // Получаем луч из мыши
            Ray ray = GetMouseRay();
            bool hasHit = Physics.Raycast(ray, out RaycastHit hit);

            // Если луч не попал ни в один объект, возвращаем false
            if (!hasHit)
                return false;

            // Если игрок кликнул мышью, перемещаемся к указанной точке
            if (Input.GetMouseButton(0))
                if (!EventSystem.current.IsPointerOverGameObject())
                {
                    bool readyToGo = true;

                    if (ConversationManager.Instance != null)
                        if (ConversationManager.Instance.IsConversationActive)
                            readyToGo = false;

                    if (readyToGo)

                        mover.StartMoveAction(hit.point);
                }

            return true;
        }

        // Метод для получения луча из позиции мыши
        private static Ray GetMouseRay()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            return ray;
        }
    }
}
