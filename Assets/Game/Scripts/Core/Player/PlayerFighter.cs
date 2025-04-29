using Combat.Data;
using Combat.EnumsCombat;
using Core;
using Core.Interfaces;
using Core.Player;
using Healths;
using Movement;
using UI.Inventory;
using UI.Inventory.Data;
using UnityEngine;
using UnityEngine.Serialization;

namespace Combat
{
[RequireComponent(typeof(Mover))]
public class PlayerFighter : Fighter
{
    [FormerlySerializedAs("fireballWeapon")] [SerializeField]
    private Weapon _fireballWeapon = null;

        private IGame _igame;
        private MainPlayer _player;
        
        private WeaponNow _weapon;
        //private bool isFireballNow = false; // TODO not used code
        

        public void Construct(IGame igame, MainPlayer player)
        {
            _igame = igame;
            _player = player;
            
            base.Construct();
        }

        public void SetHandPositions(Transform RightHand, Transform LeftHand)
        {
            _rightHandPosition = RightHand;
            _leftHandPosition = LeftHand;
            
            EquipWeapon(_defaultWeapon);
        }

        public void EquipWeapon()
        {
            if (_igame.saveGame.EquipedWeapon != null)
            {
                EquipWeapon(_igame.saveGame.EquipedWeapon);
            }
            
            if (_equippedWeapon == false)
            {
                EquipWeapon(_defaultWeapon);
            }
        }
        

        private void Update()
        {
            if (PauseClass.GetPauseState())
            {
                return;
            }

            if (ConversationStarter.IsDialogActive)
            {
                return;
            }

            _timer += Time.deltaTime;

            if (Target == false)
            {
                return;
            }

            if (InRange() == false)
            {
                _mover.MoveTo(Target.transform.position);
            }
            else
            {
                Vector3 lookAt = Target.transform.position;
                lookAt.y = transform.position.y;
                transform.LookAt(lookAt, Vector3.up);
                _mover.Cancel();
                AttackBehavior();
            }
        }
        
        
        public void SetFireball()
        {
            _weapon = WeaponNow.fire;
            _fireballWeapon.SetFireball();

            if (_fireballWeapon.IsFireball() == false)
            {
                Debug.LogError("нет галочки");
            }

            _fireballWeapon.SpawnToPlayer(_rightHandPosition, _leftHandPosition, _animator);
        }

        public  void SetCommonWeapon()
        {

             _weapon = WeaponNow.bow;
            

            if (_equippedWeapon != null)
            {
                _equippedWeapon.SpawnToPlayer(_rightHandPosition, _leftHandPosition, _animator);
            }
        }

        public void SetBow()
        {

            _weapon = WeaponNow.bow;

            if (_bowWeapon != null)
            {
                _bowWeapon.SpawnToPlayer(_rightHandPosition, _leftHandPosition, _animator);
            }
        }

        public void EquipItem(ItemDefinition item)
        {
            if (item is Armor)
            {
                ((Armor)item).EquipIt(); // TODO Expensive unboxing
            }
            else if (item is Weapon)
            {
                EquipWeapon((Weapon)item); // TODO Expensive unboxing
            }
        }

        public void EquipWeapon(Weapon weapon)
        {
            if (weapon.IsFireball())
            {
                _fireballWeapon = weapon;

                return;
            }
            else
            {
                _equippedWeapon = weapon;
                SetCommonWeapon();
            }
            
            if (_igame != null)
            {
                _igame.saveGame.EquipedWeapon = weapon;
            }
        }

        public void Cancel()
        {
            Target = null;
            _animator.SetTrigger("stopAttack"); // TODO can be cached
        }

        public void Attack(GameObject combatTarget)
        {
            if (ConversationStarter.IsDialogActive)
            {
                return;
            }

            _actionScheduler.Setup(this);
            Target = combatTarget.GetComponent<Health>(); // TODO replace getComp
            Target.GetComponent<Fighter>(); // TODO replace getComp
            
            Debug.Log("Attack" + combatTarget.name);
        }

        public bool CanAttack(GameObject target)
        {
            return target && target.GetComponent<Health>().IsDead() == false;
        }

        public void UnequipWeapon()
        {
            if (_equippedWeapon == _defaultWeapon)
            {
                return;
            }

            _equippedWeapon.DestroyWeaponOnPlayer(_rightHandPosition, _leftHandPosition, _animator);
            EquipWeapon(_defaultWeapon);
        }
        
        private float GetRangeCurrentWeapon()
        {
            switch (_weapon)
            {
                case WeaponNow.common:
                    return _equippedWeapon.GetWeaponRange();

                case WeaponNow.fire:
                    return _fireballWeapon.GetWeaponRange();

                case WeaponNow.bow:
                    return _bowWeapon.GetWeaponRange();

                default:
                    return _equippedWeapon.GetWeaponRange();
            }
        }

        private bool InRange()
        {
            var distance =
                Mathf.Abs(Vector3.Distance(transform.position, Target.transform.position)); // TODO Vector3 Extensions

            return distance < GetRangeCurrentWeapon();
        }
        
        public override void Hit()
        {
            if (Target == false)
            {
                return; // Если цели нет, выйти
            }

            if (_weapon != WeaponNow.bow)
            {
                if (_defaultWeapon == _bowWeapon)
                {
                    return;
                }
            }

            AudioManager.Instance.PlaySound("Attack"); // TODO can be cached
            // SoundManager.PlaySound("Attack"); // TODO can be cached

            if (IsBehindTarget() && Target.GetComponent<Boss>() == false) // Проверка, если атака сзади и цель  не босс
            {
                Target.AttackFromBehind(false);
            }
            else
            {
                Target.TakeDamage(_equippedWeapon.GetWeaponDamage()); // Нанести нормальный урон цели
            }

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

        protected override void AttackBehavior()
        {
            if (Target.IsDead())
            {
                Cancel();
                _actionScheduler.Cancel();
            }
            else if (_timer > _equippedWeapon.GetTimeBetweenAttacks())
            {
                if (_weapon == WeaponNow.fire)
                {
                    if (_igame.dataPlayer.PlayerData.chargeEnergy > 0)
                    {
                        _player.ChangeCountEnergy(-1); // TODO magic number
                        ShootFireball();
                        _timer = 0;

                        return;
                    }
                    else
                    {
                        _igame.playerController.WeaponPanelUI.ResetWeaponToDefault();
                    }
                }

                if (_weapon == WeaponNow.bow)
                {
                    if (_bowWeapon._currentCharges > 0)
                    {
                        ShootBow();
                        _timer = 0;

                        return;
                    }
                    else
                    {
                        _igame.playerController.WeaponPanelUI.ResetWeaponToDefault();
                    }
                }

                if ( _weapon != WeaponNow.bow)
                {
                    if (_defaultWeapon == _bowWeapon)
                    {
                        return;
                    }
                }
                
                _animator.ResetTrigger("stopAttack"); // TODO can be cached
                _animator.SetTrigger("attack"); // TODO can be cached

                if (_equippedWeapon.IsRanged())
                {
                    _equippedWeapon.SpawnProjectile(Target.transform, _rightHandPosition, _leftHandPosition, true);
                }

                _timer = 0;
            }
        }

        private void ShootFireball()
        {
            _animator.ResetTrigger("stopAttack"); // TODO can be cached
            _animator.SetTrigger("attack"); // TODO can be cached 
            _fireballWeapon.SpawnProjectile(Target.transform, _rightHandPosition, _leftHandPosition, true);
        }

        private void ShootBow()
        {
            _animator.ResetTrigger("stopAttack"); // TODO can be cached
            _animator.SetTrigger("attack"); // TODO can be cached
            _bowWeapon.SpawnProjectile(Target.transform, _rightHandPosition, _leftHandPosition, true);
        }

        private bool IsBehindTarget()
        {
            if (Target == false)
            {
                return false;
            }

            Vector3 directionToPlayer = (transform.position - Target.transform.position).normalized;
            float angleBetween = Vector3.Angle(Target.transform.forward, directionToPlayer);

            return angleBetween > 120f; // Угол, определяющий, что атака со спины (например, > 135 градусов)
            // TODO magic number
        }
    }
}

