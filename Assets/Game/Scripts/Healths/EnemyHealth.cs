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
    public class EnemyHealth : Health
    {
        private BottleManager _bottleManager;
        private CoinManager _coinManager;
        
        public event Action OnDeath;
        
        public void Construct(PlayerController playerController,
            FastTestsManager fastTestsManager,
            QuestManager questManager,
            CoinManager coinManager,
            BottleManager bottleManager,
            UIManager uiManager)
        {
            base.Construct(playerController, fastTestsManager, questManager);

            _coinManager = coinManager;
            Debug.Log("Coin Manager= "+ coinManager);
            _bottleManager = bottleManager;
            Debug.Log("Bottle Manager= " + _bottleManager);
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

        public virtual void TakeDamage(float value)
        {
            currentHealth = Mathf.Max(currentHealth - value, 0); // Уменьшаем текущее здоровье на урон, но не меньше 0 

            if (currentHealth == 0) // Если здоровье достигло нуля, вызываем метод смерти
            {
                Die();
            }
            
            healthBar.value = currentHealth; // хил бар только у других. У пользователя свой отдельный скрипт
        }

        public void Restore()
        {
            currentHealth = maxHealth;
            _isDead = false;
        }

        public bool IsDead() => _isDead;

        // // Метод для захвата состояния существа для сохранения
        // public object CaptureState() // TODO not used code
        // {
        //     return currentHealth;
        // }
        //
        // // Примечание: в настоящее время значение здоровья при загрузке новой сцены перезаписывается этим методом
        // // из-за порядка выполнения сценариев. Измените start на awake, чтобы исправить проблему
        // public void RestoreState(object state) // TODO not used code
        // {
        //     currentHealth =
        //         (float)state; // Восстанавливаем текущее здоровье из сохраненного состояния // TODO expensive unboxing
        //     if (currentHealth <= 0) // Если здоровье меньше или равно нулю, вызываем метод смерти
        //     {
        //         Die();
        //     }
        // }

        public float GetCurrentHealth() => currentHealth;
        

        
        protected override void HandlePostDeath()
        {
            if (_isRemoved == false)
            {
                GetComponent<NavMeshAgent>().enabled = false;
                _isRemoved = true;
            }

            QuestSpecialEnemyName specialEnemy = GetComponent<QuestSpecialEnemyName>();
            if (specialEnemy != null)
                _questManager.KillNew(specialEnemy.SpecialEnemyName);
            else
                _questManager.KillNew();

            if (healthBar != null)
                Destroy(healthBar.gameObject);

            LineRenderer lineRenderer = GetComponentInChildren<LineRenderer>();
            if (lineRenderer != null)
                Destroy(lineRenderer.gameObject);

            Destroy(gameObject, 5f);
            _coinManager.MakeGoldOnSceneWithCount(25, transform.position);

            var tempRandom = UnityEngine.Random.Range(0, 9);
            if (tempRandom > 6)
            {
                _bottleManager.MakeBottleOnSceneWithCount(25, transform.position);
            }
        }

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