using System.Collections;
using System.Collections;
using System.Collections.Generic;
using System;


namespace GameEngine
{
    public class EnemyHealthBase: HealthBase
    {
        public EnemyHealthBase(float maxHealth) : base(maxHealth)
        {
        }
        
        public override void TakeDamage(float value)
        {
            currentHealth = Math.Max(currentHealth - value, 0);
            if (currentHealth == 0)
            {
                Die();
            }
        }
        
    }
}

