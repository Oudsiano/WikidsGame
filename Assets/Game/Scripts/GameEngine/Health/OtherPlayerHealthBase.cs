using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameEngine
{
    public class OtherPlayerHealthBase : HealthBase
    {
        
        public OtherPlayerHealthBase(float maxHealth) : base(maxHealth){}
        
        public override void TakeDamage(float value)
        {
            Debug.Log("TakeDamage");
        }
    }
}
