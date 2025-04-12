using System.Collections;
using System.Collections.Generic;
using Combat;
using UnityEngine;
using Healths;
using Movement;

namespace Combat
{
    [RequireComponent(typeof(Mover))]
    public class EnemyFighter : Fighter
    {
        private IGame _igame;
        
        public new void Construct(IGame igame)
        {
            _igame = igame;
            base.Construct();
            EquipWeapon(_defaultWeapon);
        }
        
        private void OnMouseEnter()
        {
            if (GetComponent<Health>().IsDead() == false)
            {
                _igame.CursorManager.SetCursorSword(); // TODO replace
            }
        }
        
        public  override void Hit()
        {
            if (Target == false)
            {
                return; // Если цели нет, выйти
            }

            // if (_isPlayer && _weapon != WeaponNow.bow)
            // {
            //     if (_defaultWeapon == _bowWeapon)
            //     {
            //         return;
            //     }
            // }

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
                    targetAnim.SetTrigger("dead"); // Запуск анимации смерти // TODO can be cached
                }
            }
            else
            {
                // Запустить анимацию получения урона у цели
                Animator targetAnim = Target.GetComponent<Animator>(); // TODO bad practice with O/C principle

                if (targetAnim != null)
                {
                    targetAnim.SetTrigger("takeDamage"); // TODO can be cached
                }
            }
        }

        private void OnMouseExit()
        {
            _igame.CursorManager.SetCursorDefault();
        }
        
        protected override void AttackBehavior()
        {
            if (Target.IsDead())
            {
                Cancel();
                _actionScheduler.Cancel();
                return;
            }

            if (_timer > _equippedWeapon.GetTimeBetweenAttacks())
            {
                _animator.ResetTrigger("stopAttack");
                _animator.SetTrigger("attack");

                if (_equippedWeapon.IsRanged())
                {
                    _equippedWeapon.SpawnProjectile(Target.transform, _rightHandPosition, _leftHandPosition, false);
                }

                _timer = 0;
            }
        }
    }
}

