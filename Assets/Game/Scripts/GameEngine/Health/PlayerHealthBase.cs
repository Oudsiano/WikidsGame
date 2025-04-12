using System.Collections;
using System.Collections.Generic;
using Healths;
using System;

namespace GameEngine
{
    public class PlayerHealthBase: HealthBase
    {
        public event Action OnDodge;
        
        public int IsAttackedInLast5Sec
        {
            get => isAttackedInLast5Sec;
            set => isAttackedInLast5Sec = value;
        }

        public PlayerHealthBase(float maxHealth) : base(maxHealth)
        {
        }

        public override void TakeDamage(float value)
        {
            Random random = new Random();
            var tempRandom = random.Next(0, 10);
            
            if (tempRandom > 6) // 30% шанс уклонения
            {
                OnDodge?.Invoke();
                return; // Уклонение
            }

            currentHealth = Math.Max(currentHealth - value, 0);
            if (currentHealth == 0)
            {
                Die();
            }

            isAttackedInLast5Sec = 5;
        }

        // Логика лечения без корутин
        public void UpdateHealth()
        {
            if (!isDead)
            {
                if (isAttackedInLast5Sec > 0)
                {
                    isAttackedInLast5Sec--;
                }

                if (isAttackedInLast5Sec == 0)
                {
                    Heal(3);
                }
                else
                {
                    Heal(1);
                }
            }
        }
        
    } 
}

