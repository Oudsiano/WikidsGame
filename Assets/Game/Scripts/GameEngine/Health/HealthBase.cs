using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameEngine
{
    [System.Serializable]
    public abstract class HealthBase
    {
        protected float maxHealth; // Максимальное здоровье
        protected float currentHealth; // Текущее здоровье
        protected bool isDead = false; // Флаг смерти
        protected bool isRemoved = false; // Флаг удаления
        protected int isAttackedInLast5Sec = 0; // Счетчик времени с последнего удара

        public event Action OnDeath; // Событие смерти
        
        public bool IsRemoved => isRemoved;

        public HealthBase(float maxHealth)
        {
            this.maxHealth = maxHealth;
            this.currentHealth = maxHealth;
        }
        
        public virtual void Heal(float value)
        {
            currentHealth = Mathf.Min(currentHealth + value, maxHealth);
        }

        public abstract void TakeDamage(float value);

        public virtual void TakeProjectileHit(float damage, Vector3 hitDirection)
        {
            TakeDamage(damage); // По умолчанию — обычный урон
        }

        public void Restore()
        {
            currentHealth = maxHealth;
            isDead = false;
        }

        public bool IsDead() => isDead;

        public float GetCurrentHealth() => currentHealth;

        protected virtual void Die()
        {
            isDead = true;
            OnDeath?.Invoke();
        }
        
        // Методы для сохранения и восстановления состояния
        public object CaptureState()
        {
            return new HealthState
            {
                CurrentHealth = currentHealth,
                IsDead = isDead,
                IsRemoved = isRemoved,
                IsAttackedInLast5Sec = isAttackedInLast5Sec
            };
        }

        public void RestoreState(object state)
        {
            var healthState = (HealthState)state;
            currentHealth = healthState.CurrentHealth;
            isDead = healthState.IsDead;
            isRemoved = healthState.IsRemoved;
            isAttackedInLast5Sec = healthState.IsAttackedInLast5Sec;

            if (currentHealth <= 0)
            {
                Die();
            }
        }

        public void SetRemoved()
        {
            isRemoved = true;
        }
    }
    
    [System.Serializable]
    public class HealthState
    {
        public float CurrentHealth;
        public bool IsDead;
        public bool IsRemoved;
        public int IsAttackedInLast5Sec;
    }
    
}
    

