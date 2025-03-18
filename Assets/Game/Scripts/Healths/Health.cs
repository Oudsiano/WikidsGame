using System;
using System.Collections;
using AINavigation;
using Combat;
using Core;
using Core.NPC;
using Core.Player;
using Core.Quests;
using SceneManagement;
using UI;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Healths
{
    public abstract class Health : MonoBehaviour
    {
        [FormerlySerializedAs("redHalfCircle")]
        public GameObject RedHalfCircle;

        public Slider healthBar; // Ссылка на полосу здоровья в пользовательском интерфейсе // TODO rename
        public float maxHealth; // Максимальное здоровье существа // TODO rename
        public float currentHealth; // Текущее здоровье существа // TODO rename

        protected bool _isDead = false; // Флаг, указывающий, что существо мертво
        protected bool _isRemoved = false; // Флаг, указывающий, что существо было удалено
        protected int isAtackedInlast5sec = 0; // TODO why int

        protected bool _isPlayer = false; // TODO rename
        protected BossNPC _bossNPC; // TODO no need here
        protected QuestManager _questManager;
        protected FastTestsManager _fastTestsManager;
        protected PlayerController _playerController;
        
        public event Action OnDeath; // TODO rename

        public BossNPC BossNPC
        {
            get => _bossNPC;
            set => _bossNPC = value;
        } // TODO no need here

        public virtual void Construct(PlayerController playerController,
            FastTestsManager fastTestsManager,
            QuestManager questManager)
        {
            _playerController = playerController;
            _fastTestsManager = fastTestsManager;
            _questManager = questManager;

            currentHealth = maxHealth;
            if (healthBar != null)
                healthBar.value = currentHealth;

            _isPlayer = gameObject.GetComponent<MainPlayer>() != null;
        }

        private IEnumerator HeallUpPLayer()
        {
            while (true) // TODO can be allocated memory
            {
                if (_isDead == false)
                {
                    if (isAtackedInlast5sec > 0)
                    {
                        isAtackedInlast5sec--;
                    }

                    if (isAtackedInlast5sec == 0)
                    {
                        Heal(3);
                    }
                    else
                    {
                        Heal(1);
                    }
                }

                yield return new WaitForSeconds(1); // TODO magic numbers
            }
        }

        public void Heal(float value)
        {
            currentHealth = Mathf.Min(currentHealth + value, maxHealth);
        }


        public void MissFastTest() // TODO rename
        {
            Fighter fighter = GetComponent<Fighter>(); // TODO tryGetComp

            if (fighter != null)
            {
                fighter.Target = _playerController.GetHealth();
            }
        }


        public void AttackFromBehind(bool alreadyNeedKill)
        {
            if (alreadyNeedKill)
            {
                TakeDamage(GetCurrentHealth());

                return;
            }

            if (_isPlayer == false)
            {
                _fastTestsManager.WasAttaked(this); // TODO rename
            }
        }
        
        
        public abstract void TakeDamage(float value);
        
        public virtual void TakeProjectileHit(float damage, Vector3 hitDirection)
        {
            TakeDamage(damage); // по умолчанию — обычный урон
        }

        public void Restore()
        {
            currentHealth = maxHealth;
            _isDead = false;
        }

        public bool IsDead() => _isDead;

        // Метод для захвата состояния существа для сохранения
        public object CaptureState() // TODO not used code
        {
            return currentHealth;
        }

        // Примечание: в настоящее время значение здоровья при загрузке новой сцены перезаписывается этим методом
        // из-за порядка выполнения сценариев. Измените start на awake, чтобы исправить проблему
        public void RestoreState(object state) // TODO not used code
        {
            currentHealth =
                (float)state; // Восстанавливаем текущее здоровье из сохраненного состояния // TODO expensive unboxing
            if (currentHealth <= 0) // Если здоровье меньше или равно нулю, вызываем метод смерти
            {
                Die();
            }
        }

        public float GetCurrentHealth() => currentHealth;

        private void Dodge()
        {
            GetComponent<Animator>().SetTrigger("dodge"); // TODO can be cached
        }

        protected virtual void Die() // TODO overload method
        {
            if (RedHalfCircle != null)
                RedHalfCircle.SetActive(false);

            GetComponent<Animator>().SetTrigger("dead");
            _isDead = true;
            GetComponent<ActionScheduler>().Cancel();
            RemoveProjectiles();

            OnDeath?.Invoke();

            HandlePostDeath();
        }
        
        protected abstract void HandlePostDeath();

        private void RemoveProjectiles()
        {
            Projectile[]
                projectiles =
                    GetComponentsInChildren<Projectile>(); // Получаем все снаряды, находящиеся в дочерних объектах // TODO can be cached

            foreach (Projectile projectile in projectiles) // Перебираем все снаряды
            {
                Destroy(projectile.gameObject); // Уничтожаем снаряд
                Debug.Log("hello"); // Выводим сообщение в консоль
            }
        }
    }
}