using JetBrains.Annotations;
using RPG.Core;
using UnityEngine;

namespace RPG.Combat
{
    public class Projectile : MonoBehaviour
    {
        [Header("Projectile Settings")]
        [SerializeField] private Transform target; // Цель, к которой летит снаряд
        [SerializeField] private float speed = 5f; // Скорость полета снаряда
        [SerializeField] private float thisProjectileDamage = 0f; // Урон этого конкретного снаряда
        [SerializeField] private float projectileDestroySpeed = 5f; // Время до уничтожения снаряда после попадания
        [SerializeField] private float projectileDecaySpeed = 5f; // Время до уничтожения снаряда после завершения полета
        [SerializeField] private bool homing = false; // Флаг, определяющий, должен ли снаряд самонаводиться на цель

        [Header("FX")]
        [SerializeField] private GameObject hitEffect = null; // Эффект при попадании

        [Header("Arrow Effect Settings")]
        [SerializeField] private float minHitVariance = 0f; // Минимальное отклонение при попадании
        [SerializeField] private float maxHitVariance = 1f; // Максимальное отклонение при попадании
        [SerializeField] private GameObject[] destroyOnHit; // Объекты, уничтожаемые при попадании

        private float damage; // Общий урон снаряда
        private bool reachedCollider = false; // Флаг, указывающий, достиг ли снаряд коллайдера

        // Устанавливаем направление полета снаряда на момент запуска
        void Start()
        {
            transform.LookAt(GetTargetPosition());
        }

        void Update()
        {
            // Проверяем, не умерла ли цель или не достиг ли снаряд коллайдера
            bool isDead = target.GetComponent<Health>().IsDead();
            if (!reachedCollider || isDead)
            {
                // Если снаряд должен самонаводиться, корректируем его направление на цель
                if (homing)
                    transform.LookAt(GetTargetPosition());

                // Перемещаем снаряд вперед по направлению
                transform.Translate(Vector3.forward * speed * Time.deltaTime);

                // Если цель мертва, уничтожаем снаряд через некоторое время
                if (isDead)
                {
                    Destroy(gameObject, projectileDecaySpeed);
                    homing = false; // Отключаем самонаведение
                }
            }
        }

        // Получаем позицию, куда должен попасть снаряд
        private Vector3 GetTargetPosition()
        {
            CapsuleCollider targetCollider = target.GetComponent<CapsuleCollider>();
            // Добавляем случайное отклонение по высоте, чтобы снаряд выглядел естественно при попадании
            return (target.position + Vector3.up * (targetCollider.height + Random.Range(minHitVariance, maxHitVariance)) / 2);
        }

        // Устанавливаем цель и урон для снаряда
        public void SetTarget(Transform target, float damage)
        {
            this.target = target;
            this.damage = damage + thisProjectileDamage; // Учитываем дополнительный урон от этого конкретного снаряда
        }

        // Обработчик столкновения с коллайдером
        private void OnTriggerEnter(Collider other)
        {
            // Проверяем, что объект столкнулся с целью и цель жива
            if (other.gameObject == target.gameObject && !target.GetComponent<Health>().IsDead())
            {
                // Наносим урон цели
                other.GetComponent<Health>().TakeDamage(damage);

                // Рассчитываем позицию снаряда после попадания
                Vector3 targetPos = GetTargetPosition();
                transform.position = targetPos;
                transform.parent = other.transform; // Делаем снаряд дочерним объектом цели

                // Воспроизводим эффект при попадании, если он указан
                if (hitEffect)
                {
                    GameObject effect = Instantiate(hitEffect, GetTargetPosition(), Quaternion.identity);
                }

                // Уничтожаем все объекты, указанные для уничтожения при попадании
                foreach (GameObject gameObjectToDestroy in destroyOnHit)
                {
                    Destroy(gameObjectToDestroy);
                }

                // Уничтожаем снаряд через некоторое время
                Destroy(gameObject, projectileDestroySpeed);

                TrailRenderer trail = GetComponentInChildren<TrailRenderer>();
                if (trail)
                    trail.enabled = false; // Отключаем эффект следа за снарядом

                reachedCollider = true; // Устанавливаем флаг, указывающий, что снаряд достиг коллайдера
            }
        }
    }
}
