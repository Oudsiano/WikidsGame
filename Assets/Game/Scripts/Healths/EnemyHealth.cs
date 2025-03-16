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
        
        public override void TakeProjectileHit(float damage, Vector3 hitDirection)
        {
            Vector3 forward = transform.forward;
            float angle = Vector3.Angle(forward, -hitDirection);

            Debug.Log("angle =" + angle);
            
            if (angle < 100f) // Спина
            {
                TakeDamage(currentHealth); // ваншот
                Debug.Log("OneShoot");
                
            }
            else
            {
                TakeDamage(maxHealth/2); // обычный урон
            }
        }

        public override  void TakeDamage(float value)
        {

            currentHealth = Mathf.Max(currentHealth - value, 0); // Уменьшаем текущее здоровье на урон, но не меньше 0 

            if (currentHealth == 0) // Если здоровье достигло нуля, вызываем метод смерти
            {
                Die();
            }
            
            healthBar.value = currentHealth; // хил бар только у других. У пользователя свой отдельный скрипт
            
        }
        
        public bool IsDead() => _isDead;
        

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
        
    }
}