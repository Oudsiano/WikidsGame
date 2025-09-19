using System.Collections;
using System.Collections.Generic;
using Combat;
using UnityEngine;

public class OtherPlayerFighter : Fighter
{
    private IGame _igame;

    public new void Construct(IGame igame)
    {
        _igame = igame;
        base.Construct();
        EquipWeapon(_defaultWeapon);
    }

    protected override void AttackBehavior()
    {
        Debug.LogWarning("AttackBehavior is not implemented in OtherPlayerFighter!");
    }
    
    public void SetHandPositions(Transform RightHand, Transform LeftHand)
    {
        _rightHandPosition = RightHand;
        _leftHandPosition = LeftHand;
            
        EquipWeapon(_defaultWeapon);
    }

    public override void Hit()
    {
        if (Target == false)
        {
            return; // Если цели нет, выйти
        }

        AudioManager.Instance.PlaySound("Attack"); // TODO can be cached

        Target.TakeDamage(_equippedWeapon.GetWeaponDamage()); // Нанести нормальный урон цели

        // Проиграть эффект при попадании
        Vector3 hitPosition = new Vector3(Target.transform.position.x, Target.transform.position.y + 1.5f,
            Target.transform.position.z - 1); // Использовать позицию цели для VFX // // TODO magic numbers
        _equippedWeapon.PlayHitVFX(hitPosition);

        if (Target.IsDead())
        {
            Animator targetAnim = Target.GetComponent<Animator>(); // TODO bad practice with O/C principle

            if (targetAnim != null)
            {
                targetAnim.SetTrigger("Die");
            }
        }
    }
}
