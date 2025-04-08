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
    public class EnemyHealth : Health
    {
        private BottleManager _bottleManager;
        private CoinManager _coinManager;
        
        
        public void Construct(PlayerController playerController,
            FastTestsManager fastTestsManager,
            QuestManager questManager,
            CoinManager coinManager,
            BottleManager bottleManager)
        {
            base.Construct(playerController, fastTestsManager, questManager);

            _coinManager = coinManager;
            _bottleManager = bottleManager;
        }
        
        protected override HealthBase CreateHealthBase(float maxHealth)
        {
            return new EnemyHealthBase(maxHealth);
        }
        
        public override void TakeProjectileHit(float damage, Vector3 hitDirection)
        {
            Vector3 forward = transform.forward;
            float angle = Vector3.Angle(forward, -hitDirection);

            Debug.Log("angle =" + angle);
            
            if (angle < 100f) // Спина
            {
                TakeDamage(healthBase.GetCurrentHealth()); // ваншот
                Debug.Log("OneShoot");
                
            }
            else
            {
                TakeDamage(maxHealth/2); // обычный урон
            }
        }

        public override  void TakeDamage(float value)
        {
            healthBase.TakeDamage(value);
            Debug.Log("[EnemyHealth] CurrentHealth= " + healthBase.GetCurrentHealth());
            healthBar.value = healthBase.GetCurrentHealth(); // хил бар только у других. У пользователя свой отдельный скрипт

            if (healthBase.GetCurrentHealth() <= 0)
            {
                Die();
            }
            
        }
        
        protected override void HandlePostDeath()
        {
            if (healthBase.IsRemoved == false)
            {
                GetComponent<NavMeshAgent>().enabled = false;
                healthBase.SetRemoved();
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