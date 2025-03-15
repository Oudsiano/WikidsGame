using System.Collections;
using System.Collections.Generic;
using Combat;
using Combat.EnumsCombat;
using UnityEngine;

public class PlayerFighter : Fighter
{
    public override void SetCommonWeapon()
    {
        _weapon = WeaponNow.bow;
        

        if (_equippedWeapon != null)
        {
            _equippedWeapon.SpawnToPlayer(_rightHandPosition, _leftHandPosition, _animator);
        }
    }
}
