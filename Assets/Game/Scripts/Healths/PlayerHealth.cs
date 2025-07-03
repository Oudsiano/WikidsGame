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
    public class PlayerHealth : Health
    {
        private UIManager _uiManager;
        private float healTimer = 0f;
        
        public int Health => (int)healthBase.GetCurrentHealth();
   

        public void Construct(PlayerController playerController,
            FastTestsManager fastTestsManager,
            QuestManager questManager,
            UIManager uiManager)
        {
            base.Construct(playerController, fastTestsManager, questManager);

            _uiManager = uiManager;
            
            if (healthBase is PlayerHealthBase playerHealthBase)
            {
                playerHealthBase.OnDodge += Dodge;
            }
            else
            {
                Debug.LogError("healthBase is not a PlayerHealthBase in PlayerHealth!");
            }
            
            StartCoroutine(HeallUpPLayer());
            
        }
        
        protected override HealthBase CreateHealthBase(float maxHealth)
        {
            return new PlayerHealthBase(maxHealth);
        }

        private IEnumerator HeallUpPLayer()
        {
            while (true) // TODO can be allocated memory
            {
                if (healthBase is PlayerHealthBase playerHealthBase && healthBase.GetCurrentHealth() < maxHealth)
                {
                    playerHealthBase.UpdateHealth();
                    
                    SocketManager.SendUserHealthInGame(SocketManager.MyLocalPlayerId, healthBase.GetCurrentHealth());
                }
                
                yield return new WaitForSeconds(1); // TODO magic numbers
            }
        }

        public void Heal(float value)
        {
            healthBase.Heal(value);
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

        public override  void TakeDamage(float value)
        {
            healthBase.TakeDamage(value);
            
            if (healthBase.GetCurrentHealth() <= 0)
            {
                Die();
                MultiplayerController.Instance.DestroyOtherPlayer();
            }
            
            SocketManager.SendUserHealthInGame(SocketManager.MyLocalPlayerId, healthBase.GetCurrentHealth());
            // SocketManager.SendPlayerHealth(healthBase.GetCurrentHealth());
        }
        
        
        // Метод для захвата состояния существа для сохранения

        

        // public float GetCurrentHealth() => currentHealth;

        private void Dodge()
        {
            GetComponent<Animator>().SetTrigger("dodge"); // TODO can be cached
        }


        
        protected override void HandlePostDeath()
        {
            Debug.Log("Player Death");
            _uiManager.DeathUI.ShowDeathScreen();
            Debug.Log("Показан экран смерти");

            // можно здесь отключать управление, если надо:
            // GetComponent<NavMeshAgent>().enabled = false;
            // GetComponent<Collider>().enabled = false;
            // this.enabled = false;
        }
        
    }
}
