using System.Collections;
using System.Collections.Generic;
using Healths;
using UI;
using UnityEngine.UI;
using UnityEngine;
using GameEngine;

namespace Healths
{
    public class OtherPlayerHealth : Health
    {
        [SerializeField] private Slider _healthSlider;
        private float _currentHealth;
        
        
        public void Construct(float health)
        {
            _currentHealth = health;
            UpdateHealth(health);
        }
        
        public void UpdateHealth(float health)
        {
            _currentHealth = health;

            if (_healthSlider != null)
            {
                _healthSlider.value = _currentHealth;
            }
            else
            {
                Debug.LogWarning("[OtherPlayer] Health slider is not assigned!");
            }
        
            // Например, можно сделать визуальный фидбек
            Debug.Log($"[OtherPlayerController] Current health updated: {_currentHealth}");

            // Здесь можно обновить UI, полоску HP и т.п.
        }
        
        
        protected override HealthBase CreateHealthBase(float maxHealth)
        {
            return new PlayerHealthBase(maxHealth);
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
        
        protected override void HandlePostDeath()
        {
            Debug.Log("OtherPlayer Death");
            Debug.Log("Показан экран смерти");

            // можно здесь отключать управление, если надо:
            // GetComponent<NavMeshAgent>().enabled = false;
            // GetComponent<Collider>().enabled = false;
            // this.enabled = false;
        }
        
    }
}


