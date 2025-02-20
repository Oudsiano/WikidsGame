using RPG.Core;
using UnityEngine;
using UnityEngine.Serialization;

namespace Combat
{
    public class Projectile : MonoBehaviour
    {
        [FormerlySerializedAs("target")] [Header("Projectile Settings")] [SerializeField]
        private Transform _target; // Цель, к которой летит снаряд

        [FormerlySerializedAs("speed")] [SerializeField]
        private float _speed = 5f; // Скорость полета снаряда

        [FormerlySerializedAs("thisProjectileDamage")] [SerializeField]
        private float _thisProjectileDamage = 0f; // Урон этого конкретного снаряда

        [FormerlySerializedAs("projectileDestroySpeed")] [SerializeField]
        private float _projectileDestroySpeed = 5f; // Время до уничтожения снаряда после попадания

        [FormerlySerializedAs("projectileDecaySpeed")] [SerializeField]
        private float _projectileDecaySpeed = 5f; // Время до уничтожения снаряда после завершения полета

        [FormerlySerializedAs("homing")] [SerializeField]
        private bool _homing = false; // Флаг, определяющий, должен ли снаряд самонаводиться на цель

        [FormerlySerializedAs("hitEffect")] [Header("FX")] [SerializeField]
        private GameObject _hitEffect = null; // Эффект при попадании c // TODO GO

        [FormerlySerializedAs("minHitVariance")] [Header("Arrow Effect Settings")] [SerializeField]
        private float _minHitVariance = 0f; // Минимальное отклонение при попадании

        [FormerlySerializedAs("maxHitVariance")] [SerializeField]
        private float _maxHitVariance = 1f; // Максимальное отклонение при попадании

        [FormerlySerializedAs("destroyOnHit")] [SerializeField]
        private GameObject[] _destroyOnHit; // Объекты, уничтожаемые при попадании // TODO GO

        private float _damage; // Общий урон снаряда
        private bool _reachedCollider = false; // Флаг, указывающий, достиг ли снаряд коллайдера

        private Rigidbody _rigidbody;

        private void Start()
        {
            transform.LookAt(GetTargetPosition());
            _rigidbody = GetComponent<Rigidbody>(); // TODO RequireComp
            _rigidbody.collisionDetectionMode = CollisionDetectionMode.Continuous;
        }

        private void Update()
        {
            if (_target == null)
            {
                return;
            }

            bool isDead = _target.GetComponent<Health>().IsDead(); // TODO getcomp

            if (_reachedCollider == false || isDead)
            {
                // Если снаряд должен самонаводиться, корректируем его направление на цель
                if (_homing)
                {
                    transform.LookAt(GetTargetPosition());
                }

                // Перемещаем снаряд вперед по направлению
                _rigidbody.MovePosition(transform.position +
                                        transform.forward * _speed * Time.deltaTime); // TODO  MOVE TO FIXEDUPDATE 
                //transform.Translate(Vector3.forward * speed * Time.deltaTime);

                // Если цель мертва, уничтожаем снаряд через некоторое время
                if (isDead)
                {
                    Destroy(gameObject, _projectileDecaySpeed);
                    _homing = false; // Отключаем самонаведение
                }
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject == _target.gameObject && _target.GetComponent<Health>().IsDead() == false)
            {
                other.GetComponent<Health>().TakeDamage(_damage); // TODO can be changed with Actions

                // Рассчитываем позицию снаряда после попадания
                Vector3 targetPos = GetTargetPosition();
                transform.position = targetPos;
                transform.parent =
                    other.transform; // Делаем снаряд дочерним объектом цели // TODO move to projectile factory

                if (_hitEffect)
                {
                    GameObject effect =
                        Instantiate(_hitEffect, GetTargetPosition(), Quaternion.identity); // TODO not used code
                }

                foreach (GameObject gameObjectToDestroy in _destroyOnHit) // TODO GO
                {
                    Destroy(gameObjectToDestroy);
                }

                Destroy(gameObject, _projectileDestroySpeed);
                TrailRenderer trail = GetComponentInChildren<TrailRenderer>();

                if (trail)
                {
                    trail.enabled = false; // Отключаем эффект следа за снарядом
                }

                _reachedCollider = true; // Устанавливаем флаг, указывающий, что снаряд достиг коллайдера
            }
        }

        // Устанавливаем цель и урон для снаряда
        public void SetTarget(Transform target, float damage)
        {
            _target = target;
            _damage = damage + _thisProjectileDamage; // Учитываем дополнительный урон от этого конкретного снаряда
        }

        private Vector3 GetTargetPosition()
        {
            CapsuleCollider targetCollider = _target.GetComponent<CapsuleCollider>(); // TODO TRYGETCOMP

            // Добавляем случайное отклонение по высоте, чтобы снаряд выглядел естественно при попадании
            return (_target.position +
                    Vector3.up * (targetCollider.height + Random.Range(_minHitVariance, _maxHitVariance)) / 2);
        }
    }
}