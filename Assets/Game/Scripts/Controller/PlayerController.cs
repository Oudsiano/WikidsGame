using UnityEngine;
using UnityEngine.EventSystems;
using RPG.Movement;
using RPG.Combat;
using RPG.Core;
using DialogueEditor;
using System;

namespace RPG.Controller
{
    // Класс, управляющий игроком
    public class PlayerController : MonoBehaviour
    {
        // Ссылки на компоненты
        private Fighter fighter; // Компонент, отвечающий за атакующее поведение игрока
        private Mover mover; // Компонент, отвечающий за перемещение игрока
        private Health health; // Компонент, отвечающий за здоровье игрока

        public  PlayerArmorManager playerArmorManager;

        public WeaponPanelUI WeaponPanelUI;
        public PlayerUIManager PlayerUIManager;

        private int enemyLayer = 9; // Номер слоя для врагов

        // Метод Start вызывается перед первым обновлением кадра
        public void Init()
        {
            // Получаем ссылки на компоненты
            mover = GetComponent<Mover>();
            fighter = GetComponent<Fighter>();
            health = GetComponent<Health>();
            playerArmorManager = FindObjectOfType<PlayerArmorManager>();


            WeaponPanelUI = FindObjectOfType<WeaponPanelUI>();
            PlayerUIManager = FindObjectOfType<PlayerUIManager>();

            WeaponPanelUI.Init();
            PlayerUIManager.Init();

            RPG.Core.SceneLoader.LevelChanged += SceneLoader_LevelChanged;
        }

        private void SceneLoader_LevelChanged(LevelChangeObserver.allScenes obj)
        {
            EquipWeaponAndArmorAfterLoad();
        }

        public Health GetHealth () => health;
        public Fighter GetFighter () => fighter;

        public void EquipWeaponAndArmorAfterLoad()
        {
            if (IGame.Instance.dataPLayer.playerData.weaponToLoad.Length>1)
            {
                foreach (Weapon weapon in IGame.Instance.WeaponManager.allWeaponsInGame)
                {
                    if (weapon.name== IGame.Instance.dataPLayer.playerData.weaponToLoad)
                    {
                        fighter.EquipWeapon(weapon);
                    }
                }

            }
        }

        // Метод Update вызывается один раз за кадр
        void Update()
        {
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

                if (ConversationManager.Instance!=null)
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
