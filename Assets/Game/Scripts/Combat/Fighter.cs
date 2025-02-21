using Combat.Data;
using Combat.EnumsCombat;
using Core;
using Core.Health;
using Core.Interfaces;
using Core.Player;
using FarrokhGames.Inventory.Examples;
using Movement;
using RPG.Core;
using UnityEngine;
using UnityEngine.Serialization;

namespace Combat
{
    public class Fighter : MonoBehaviour, IAction
    {
        [FormerlySerializedAs("rightHandPosition")] [Header("Fighter Stats")] [Header("Weapon")] [SerializeField]
        private Transform _rightHandPosition = null;

        [FormerlySerializedAs("leftHandPosition")] [SerializeField]
        private Transform _leftHandPosition = null;

        [FormerlySerializedAs("defaultWeapon")] [SerializeField]
        private Weapon _defaultWeapon = null;

        [FormerlySerializedAs("equippedWeapon")] [SerializeField]
        private Weapon _equippedWeapon = null;

        [FormerlySerializedAs("fireballWeapon")] [SerializeField]
        private Weapon _fireballWeapon = null;

        [FormerlySerializedAs("bowWeapon")] [SerializeField]
        private Weapon _bowWeapon = null;

        [FormerlySerializedAs("target")] public Health Target;

        private float _timer = 20;
        private bool _isPlayer;
        private WeaponNow _weapon;
        //private bool isFireballNow = false; // TODO not used code

        private Fighter _fighter;

        private Mover _mover;
        private ActionScheduler _actionScheduler;
        private Animator _animator;

        private void Awake() // TODO construct
        {
            _mover = GetComponent<Mover>(); // TODO RequireComponents
            _actionScheduler = GetComponent<ActionScheduler>();
            _animator = GetComponent<Animator>();
            _isPlayer = gameObject.GetComponent<MainPlayer>() ? true : false;

            if (_isPlayer)
            {
                if (IGame.Instance.saveGame.EquipedWeapon != null)
                {
                    EquipWeapon(IGame.Instance.saveGame.EquipedWeapon);
                }
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

            //Archer // TODO not used code
            //if (targetF?.defaultWeapon == bowWeapon && (bowWeapon.currentCharges == 0 || weaponNow != WeaponNow.bow))
            //    return;

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

        private void OnMouseEnter()
        {
            if (_isPlayer == false && GetComponent<Health>().IsDead() == false)
            {
                IGame.Instance.CursorManager.SetCursorSword();
            }
        }

        private void OnMouseExit()
        {
            IGame.Instance.CursorManager.SetCursorDefault();
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

        public void SetCommonWeapon()
        {
            if (_animator == null)
            {
                Awake(); // TODO CRITICAL CALL BACK AWAKE??
            }

            _weapon = WeaponNow.common;

            if (_equippedWeapon != null)
            {
                _equippedWeapon.SpawnToPlayer(_rightHandPosition, _leftHandPosition, _animator);
            }
        }

        public void SetBow()
        {
            if (_animator == null)
            {
                Awake(); // TODO CRITICAL CALL BACK AWAKE??
            }

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

            if (_isPlayer)
            {
                if (IGame.Instance != null)
                {
                    IGame.Instance.saveGame.EquipedWeapon = weapon;
                }
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
            _fighter = Target.GetComponent<Fighter>(); // TODO replace getComp
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
        
        public void Hit()
        {
            if (Target == false)
            {
                return; // Если цели нет, выйти
            }

            if (_isPlayer && _weapon != WeaponNow.bow)
            {
                if (_fighter._defaultWeapon == _bowWeapon)
                {
                    return;
                }
            }

            AudioManager.instance.PlaySound("Attack"); // TODO can be cached

            if (IsBehindTarget() && Target.GetComponent<MainPlayer>() == false &&
                Target.GetComponent<Boss>() == false) // Проверка, если атака сзади и цель не игрок и не босс
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
        
        // public void SetTarget(Health other) // TODO not used code
        // {
        //     Target = other;
        // }
        //
        // public object CaptureState()
        // {
        //     return _equippedWeapon.name;
        // }
        //
        // public void RestoreState(object state)
        // {
        //     string weaponName = (string)state;
        //     Weapon weapon = Resources.Load<Weapon>(weaponName);
        //     EquipWeapon(weapon);
        // }
        
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

        private void AttackBehavior()
        {
            if (_isPlayer)
            {
                _isPlayer = true;
            }

            if (Target.IsDead())
            {
                Cancel();
                _actionScheduler.Cancel();
            }
            else if (_timer > _equippedWeapon.GetTimeBetweenAttacks())
            {
                if (_isPlayer && _weapon == WeaponNow.fire)
                {
                    if (IGame.Instance.dataPlayer.playerData.chargeEnergy > 0)
                    {
                        MainPlayer.Instance.ChangeCountEnergy(-1); // TODO magic number
                        ShootFireball();
                        _timer = 0;

                        return;
                    }
                    else
                    {
                        IGame.Instance.playerController.WeaponPanelUI.ResetWeaponToDefault();
                    }
                }

                if (_isPlayer && _weapon == WeaponNow.bow)
                {
                    if (_bowWeapon._currentCharges > 0)
                    {
                        ShootBow();
                        _timer = 0;

                        return;
                    }
                    else
                    {
                        IGame.Instance.playerController.WeaponPanelUI.ResetWeaponToDefault();
                    }
                }

                if (_isPlayer && _weapon != WeaponNow.bow)
                {
                    if (_fighter._defaultWeapon == _bowWeapon)
                    {
                        return;
                    }
                }

                if (_isPlayer)
                {
                    _isPlayer = true;
                }

                _animator.ResetTrigger("stopAttack"); // TODO can be cached
                _animator.SetTrigger("attack"); // TODO can be cached

                if (_equippedWeapon.IsRanged())
                {
                    _equippedWeapon.SpawnProjectile(Target.transform, _rightHandPosition, _leftHandPosition, _isPlayer);
                }

                _timer = 0;
            }
        }

        private void ShootFireball()
        {
            _animator.ResetTrigger("stopAttack"); // TODO can be cached
            _animator.SetTrigger("attack"); // TODO can be cached 
            _fireballWeapon.SpawnProjectile(Target.transform, _rightHandPosition, _leftHandPosition, _isPlayer);
        }

        private void ShootBow()
        {
            _animator.ResetTrigger("stopAttack"); // TODO can be cached
            _animator.SetTrigger("attack"); // TODO can be cached
            _bowWeapon.SpawnProjectile(Target.transform, _rightHandPosition, _leftHandPosition, _isPlayer);
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
        
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;

            if (_equippedWeapon)
            {
                Gizmos.DrawWireSphere(transform.position, _equippedWeapon.GetWeaponRange());
            }
        }
    }
}