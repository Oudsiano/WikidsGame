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
    public class Fighter : MonoBehaviour, IAction
    {
        private IGame _igame;

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

        private MainPlayer _player;
        private Mover _mover;
        private ActionScheduler _actionScheduler;
        private Animator _animator;

        public void Construct(IGame igame, MainPlayer player)
        {
            Debug.Log("%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%");
            Debug.Log("Start fighter constructed");
            _igame = igame;
            Debug.Log("get igame");
            _player = player;
            Debug.Log("get player");
            
            _mover = GetComponent<Mover>(); 
            Debug.Log("get mover");
            _actionScheduler = GetComponent<ActionScheduler>();
            Debug.Log("get _actionScheduler");
            _animator = GetComponent<Animator>();
            Debug.Log("get _animator");

            
            Debug.Log("Finish fighter constructed");
            Debug.Log("%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%");
        }

        public void SetHandPositions(Transform RightHand, Transform LeftHand)
        {
            _rightHandPosition = RightHand;
            _leftHandPosition = LeftHand;
        }

        public void EquipWeapon()
        {
            _isPlayer = gameObject.GetComponent<MainPlayer>() ? true : false;
            Debug.Log("isPlayer: " + _isPlayer);

            if (_isPlayer)
            {
                if (_igame.saveGame.EquipedWeapon != null)
                {
                    EquipWeapon(_igame.saveGame.EquipedWeapon);
                    Debug.Log("Equip _igame.saveGame.EquipedWeapon");
                }
            }

            if (_equippedWeapon == false)
            {
                EquipWeapon(_defaultWeapon);
                Debug.Log("Equip _defaultWeapon");
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
                _igame.CursorManager.SetCursorSword(); // TODO replace
            }
        }

        private void OnMouseExit()
        {
            _igame.CursorManager.SetCursorDefault();
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
            // if (_animator == null)
            // {
            //     Awake(); // TODO CRITICAL CALL BACK AWAKE??
            // }

            _weapon = WeaponNow.common;

            if (_equippedWeapon != null)
            {
                _equippedWeapon.SpawnToPlayer(_rightHandPosition, _leftHandPosition, _animator);
            }
        }

        public void SetBow()
        {
            // if (_animator == null)
            // {
            //     Awake(); // TODO CRITICAL CALL BACK AWAKE??
            // }

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
                if (_igame != null)
                {
                    _igame.saveGame.EquipedWeapon = weapon;
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

        public void Hit()
        {
            if (Target == false)
            {
                return; // Если цели нет, выйти
            }

            if (_isPlayer && _weapon != WeaponNow.bow)
            {
                if (_defaultWeapon == _bowWeapon)
                {
                    return;
                }
            }

            AudioManager.Instance.PlaySound("Attack"); // TODO can be cached

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
                        _igame.playerController.WeaponPanelUI.ResetWeaponToDefault();
                    }
                }

                if (_isPlayer && _weapon != WeaponNow.bow)
                {
                    if (_defaultWeapon == _bowWeapon)
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