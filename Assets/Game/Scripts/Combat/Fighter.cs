using RPG.Core;
using RPG.Movement;
using UnityEngine;

// Пространство имен, содержащее класс, отвечающий за бой
namespace RPG.Combat
{
    // Класс, представляющий бойца
    public class Fighter : MonoBehaviour, IAction
    {
        // Поля, связанные с оружием
        [Header("Fighter Stats")]
        [Header("Weapon")]
        [SerializeField] private Transform rightHandPosition = null; // Позиция правой руки для прикрепления оружия
        [SerializeField] private Transform leftHandPosition = null; // Позиция левой руки для прикрепления оружия
        [SerializeField] private Weapon defaultWeapon = null; // Базовое оружие
        [SerializeField] private Weapon equippedWeapon = null; // Текущее экипированное оружие
        [SerializeField] private Weapon FireballWeapon = null;
        private bool isPlayer;
        private bool isFirebalNow = false;

        [Header("")]
        public float timer = 20; // Таймер для определения времени между атаками

        // Ссылка на здоровье цели
        public Health target; // Сериализовано для отладки

        // Кэшированные компоненты
        private Mover mover; // Компонент движения
        private ActionScheduler actionScheduler; // Планировщик действий
        private Animator anim; // Компонент анимации

        void Awake()
        {
            mover = GetComponent<Mover>(); // Получаем компонент Mover
            actionScheduler = GetComponent<ActionScheduler>(); // Получаем компонент ActionScheduler
            anim = GetComponent<Animator>(); // Получаем компонент Animator
            isPlayer = gameObject.GetComponent<MainPlayer>() ? true : false;

            if (!equippedWeapon)
                EquipWeapon(defaultWeapon); // Экипируем базовое оружие при старте, если нет текущего оружия
        }

        public void SetFirball()
        {
            isFirebalNow = true;
            FireballWeapon.SpawnToPlayer(rightHandPosition, leftHandPosition, anim);
        }
        public void SetCommonWeapon()
        {
            if (anim == null)
                Awake();

            isFirebalNow = false;
            equippedWeapon.SpawnToPlayer(rightHandPosition, leftHandPosition, anim);
        }

        public void EquipArmor(Armor armor)
        {
            armor.UseToPlayer();
        }

        // Экипировка оружия
        public void EquipWeapon(Weapon weapon)
        {
            if (weapon.IsFireball())
            {
                FireballWeapon = weapon;
                return;
            }
            else
            {
                equippedWeapon = weapon; // Устанавливаем текущее оружие
                SetCommonWeapon();
            }
        }

        // Снятие оружия
        public void UnequipWeapon()
        {
            if (equippedWeapon == defaultWeapon) return; // Нельзя снять базовое оружие
            equippedWeapon.DestroyWeaponOnPlayer(rightHandPosition, leftHandPosition, anim); // Уничтожаем модель оружия на персонаже
            EquipWeapon(defaultWeapon); // Экипируем базовое оружие
        }

        void Update()
        {
            timer += Time.deltaTime; // Обновляем таймер

            if (!target) // Если нет цели, выходим
                return;

            if (!InRange()) // Если цель вне дальности атаки, перемещаемся к цели
            {
                mover.MoveTo(target.transform.position);
            }
            else // Если цель в дальности атаки, атакуем
            {
                transform.LookAt(target.transform); // Поворачиваемся к цели
                mover.Cancel(); // Отменяем движение
                AttackBehavior(); // Выполняем атаку
            }
        }

        // Проверка на нахождение в дальности атаки
        private bool InRange()
        {
            var distance = Mathf.Abs(Vector3.Distance(transform.position, target.transform.position)); // Расстояние до цели
            return distance < equippedWeapon.GetWeaponRange(); // Проверяем, находится ли цель в дальности атаки оружия
        }

        // Поведение при атаке
        private void AttackBehavior()
        {
            if (target.IsDead()) // Если цель мертва, отменяем атаку
            {
                Cancel();
                actionScheduler.CancelAction();
            }
            else if (timer > equippedWeapon.GetTimeBetweenAttacks()) // Проверяем, прошло ли время между атаками
            {
                if (isPlayer && isFirebalNow)
                {

                    if (IGame.Instance.dataPLayer.playerData.chargeEnergy > 0)
                    {
                        MainPlayer.Instance.ChangeCountEnegry(-1);
                        shotFireball();
                        timer = 0; // Сбрасываем таймер атаки
                        return;
                    }
                    else
                    {
                        IGame.Instance.playerController.WeaponPanelUI.ResetWeaponToDefault();
                    }
                }

                anim.ResetTrigger("stopAttack"); // Сбрасываем триггер остановки атаки
                anim.SetTrigger("attack"); // Запускаем анимацию атаки

                if (equippedWeapon.IsRanged()) // Если оружие дальнего боя
                    equippedWeapon.SpawnProjectile(target.transform, rightHandPosition, leftHandPosition); // Создаем снаряд

                timer = 0; // Сбрасываем таймер атаки
            }
        }

        private void shotFireball()
        {
            anim.ResetTrigger("stopAttack"); // Сбрасываем триггер остановки атаки
            anim.SetTrigger("attack"); // Запускаем анимацию атаки
            FireballWeapon.SpawnProjectile(target.transform, rightHandPosition, leftHandPosition); // Создаем снаряд
        }

        // Отмена действия
        public void Cancel()
        {
            target = null; // Сбрасываем цель
            anim.SetTrigger("stopAttack"); // Запускаем триггер остановки атаки
        }

        // Начало атаки по цели
        public void Attack(GameObject combatTarget)
        {
            actionScheduler.StartAction(this); // Начинаем действие
            target = combatTarget.GetComponent<Health>(); // Получаем здоровье цели
        }

        // Можно ли атаковать цель
        public bool CanAttack(GameObject target)
        {
            return target && !target.GetComponent<Health>().IsDead(); // Проверяем, не мертва ли цель
        }

        // Событие попадания (вызывается из анимации)
        void Hit()
        {
            if (!target) return; // Если нет цели, выходим
            AudioManager.instance.Play("Attack");

            target.TakeDamage(equippedWeapon.GetWeaponDamage()); // Наносим урон цели
        }

        // Отрисовка гизмоны для обозначения дальности атаки
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            if (equippedWeapon)
                Gizmos.DrawWireSphere(transform.position, equippedWeapon.GetWeaponRange());
        }

        // Установка цели
        public void SetTarget(Health other)
        {
            target = other;
        }

        // Сохранение состояния
        public object CaptureState()
        {
            return equippedWeapon.name; // Сохраняем имя текущего оружия
        }

        // Восстановление состояния
        public void RestoreState(object state)
        {
            string weaponName = (string)state; // Получаем имя оружия из сохраненного состояния
            Weapon weapon = Resources.Load<Weapon>(weaponName);// Загружаем оружие по его имени из ресурсов
            EquipWeapon(weapon);// Экипируем это оружие
        }
    }
}