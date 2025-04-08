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
using GameEngine;

namespace Healths
{
    public abstract class Health : MonoBehaviour
    {
        [FormerlySerializedAs("redHalfCircle")]
        public GameObject RedHalfCircle;

        public Slider healthBar; // Ссылка на полосу здоровья в пользовательском интерфейсе // TODO rename
        [SerializeField] protected float maxHealth; // Максимальное здоровье существа // TODO rename
        protected HealthBase healthBase;
        protected bool _isPlayer = false; // TODO rename
        protected BossNPC _bossNPC; // TODO no need here
        protected QuestManager _questManager;
        protected FastTestsManager _fastTestsManager;
        protected PlayerController _playerController;
        
        public float MaxHealth => maxHealth;
        
        public event Action OnDeath // TODO rename
        {
            add => healthBase.OnDeath += value;
            remove => healthBase.OnDeath -= value;
        }

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
            
            healthBase = CreateHealthBase(maxHealth);
            if (healthBar != null)
                healthBar.value = healthBase.GetCurrentHealth();

            _isPlayer = gameObject.GetComponent<MainPlayer>() != null;
        }
        
        protected abstract HealthBase CreateHealthBase(float maxHealth);
        

        public void Heal(float value)
        {
            healthBase.Heal(value);
            UpdateHealthBar();
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
                TakeDamage(healthBase.GetCurrentHealth());

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
            healthBase.Restore();
            UpdateHealthBar();
        }

        public bool IsDead() => healthBase.IsDead();

        // Метод для захвата состояния существа для сохранения
        public object CaptureState() // TODO not used code
        {
            return healthBase.CaptureState();
        }

        // Примечание: в настоящее время значение здоровья при загрузке новой сцены перезаписывается этим методом
        // из-за порядка выполнения сценариев. Измените start на awake, чтобы исправить проблему
        public void RestoreState(object state) // TODO not used code
        {
            healthBase.RestoreState(state);
            UpdateHealthBar();
            
            if (healthBase.IsDead())
            {
                Die();
            }
        }

        public float GetCurrentHealth() => healthBase.GetCurrentHealth();
        
        private void UpdateHealthBar()
        {
            if (healthBar != null)
                healthBar.value = healthBase.GetCurrentHealth();
        }

        private void Dodge()
        {
            GetComponent<Animator>().SetTrigger("dodge"); // TODO can be cached
        }

        protected virtual void Die() // TODO overload method
        {
            Debug.Log("[Health] Dead");
            if (RedHalfCircle != null)
                RedHalfCircle.SetActive(false);

            GetComponent<Animator>().SetTrigger("dead");
            GetComponent<ActionScheduler>().Cancel();
            RemoveProjectiles();
            
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