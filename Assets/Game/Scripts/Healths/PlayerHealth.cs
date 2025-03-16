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
    public class PlayerHealth : Health
    {
        private UIManager _uiManager;
   

        public void Construct(PlayerController playerController,
            FastTestsManager fastTestsManager,
            QuestManager questManager,
            UIManager uiManager)
        {
            base.Construct(playerController, fastTestsManager, questManager);

            _uiManager = uiManager;
            StartCoroutine(HeallUpPLayer());
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

        public virtual void TakeDamage(float value)
        {

            var tempRandom = UnityEngine.Random.Range(0, 9);

            if (tempRandom > 6) //30% // TODO magic numbers
            {
                Dodge();

                return;
            }

            currentHealth = Mathf.Max(currentHealth - value, 0); // Уменьшаем текущее здоровье на урон, но не меньше 0 

            if (currentHealth == 0) // Если здоровье достигло нуля, вызываем метод смерти
            {
                Die();
            }

            isAtackedInlast5sec = 5;
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


        
        protected override void HandlePostDeath()
        {
            _uiManager.DeathUI.ShowDeathScreen();

            // можно здесь отключать управление, если надо:
            // GetComponent<NavMeshAgent>().enabled = false;
            // GetComponent<Collider>().enabled = false;
            // this.enabled = false;
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
