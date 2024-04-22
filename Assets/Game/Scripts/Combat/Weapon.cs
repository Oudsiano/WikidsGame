using UnityEngine;

namespace RPG.Combat
{
    // Создает объект оружия для использования в бою
    [CreateAssetMenu(fileName = "Weapon", menuName = "Weapons", order = 0)]
    public class Weapon : ScriptableObject
    {
        [Header("Core")]
        [SerializeField] private AnimatorOverrideController weaponOverride; // Переопределенный контроллер анимации оружия
        [SerializeField] private GameObject weaponPrefab; // Префаб объекта оружия
        [SerializeField] private bool isRightHanded = true; // Определяет, используется ли правая рука по умолчанию
        [SerializeField] private Projectile projectile; // Ссылка на снаряд, если оружие дистанционное
        [SerializeField] private bool IsFireballs = false;

        [Header("Stats")]
        [SerializeField] private float weaponDamage; // Урон, наносимый оружием
        [SerializeField] private float weaponRange = 2f; // Расстояние, на котором оружие может наносить урон
        [SerializeField] private float timeBetweenAttacks; // Время между атаками

        private const string weaponName = "weapon"; // Имя объекта оружия в сцене

        // Спаунит объект оружия для игрока
        public void SpawnToPlayer(Transform rightHandPos, Transform lefthandPos, Animator anim)
        {
            // Уничтожаем предыдущее оружие игрока
            DestroyWeaponOnPlayer(rightHandPos, lefthandPos, anim);

            // Создаем новое оружие, если задан префаб
            if (weaponPrefab)
            {
                Transform handPos = FindTransformOfHand(rightHandPos, lefthandPos);
                GameObject wepInScene = Instantiate(weaponPrefab, handPos);
                wepInScene.transform.localScale = Vector3.one*  1/ wepInScene.transform.lossyScale.x;
                wepInScene.name = weaponName;
            }

            // Устанавливаем переопределенный контроллер анимации оружия, если он задан
            var overrideController = anim.runtimeAnimatorController as AnimatorOverrideController;
            if (weaponOverride)
                anim.runtimeAnimatorController = weaponOverride;
            else if (overrideController)
            {
                anim.runtimeAnimatorController = overrideController.runtimeAnimatorController;
            }
        }

        // Находит позицию для оружия в соответствии с выбранной рукой
        private Transform FindTransformOfHand(Transform rightHandPos, Transform lefthandPos)
        {
            return isRightHanded ? rightHandPos : lefthandPos;
        }

        // Уничтожает объект оружия у игрока
        public void DestroyWeaponOnPlayer(Transform rightHandPos, Transform leftHandPos, Animator anim)
        {
            DestroyWeaponOnHand(rightHandPos);
            DestroyWeaponOnHand(leftHandPos);
        }

        // Уничтожает объект оружия в указанной руке
        private void DestroyWeaponOnHand(Transform handPos)
        {
            Transform handWep = handPos.Find(weaponName);
            if (handWep)
            {
                handWep.name = "DESTROYING";
                Destroy(handWep.gameObject);
            }
        }

        // Возвращает урон оружия
        public float GetWeaponDamage()
        {
            return weaponDamage;
        }

        // Возвращает дальность действия оружия
        public float GetWeaponRange()
        {
            return weaponRange;
        }

        // Возвращает время между атаками оружия
        public float GetTimeBetweenAttacks()
        {
            return timeBetweenAttacks;
        }

        // Создает снаряд, если оружие дистанционное, и назначает цель для него
        public void SpawnProjectile(Transform target, Transform rightHand, Transform leftHand)
        {
            var proj = Instantiate(projectile, FindTransformOfHand(rightHand, leftHand).position, Quaternion.identity);
            proj.SetTarget(target, weaponDamage);
        }

        public bool IsFireball()
        {
            return IsFireballs;
        }


        // Определяет, является ли оружие дистанционным
        public bool IsRanged()
        {
            return projectile;
        }
    }
}
