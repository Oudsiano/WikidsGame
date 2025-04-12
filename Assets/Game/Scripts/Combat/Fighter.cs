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
    public abstract class Fighter : MonoBehaviour, IAction
    {
        [FormerlySerializedAs("rightHandPosition")] [Header("Fighter Stats")] [Header("Weapon")] [SerializeField]
        protected Transform _rightHandPosition = null;

        [FormerlySerializedAs("leftHandPosition")] [SerializeField]
        protected Transform _leftHandPosition = null;

        [FormerlySerializedAs("defaultWeapon")] [SerializeField]
        protected Weapon _defaultWeapon = null;

        [FormerlySerializedAs("equippedWeapon")] [SerializeField]
        protected Weapon _equippedWeapon = null;
        
        [FormerlySerializedAs("bowWeapon")] [SerializeField]
        protected Weapon _bowWeapon = null;

        [FormerlySerializedAs("target")] public Health Target;

        protected float _timer = 20;
        protected Mover _mover;
        protected ActionScheduler _actionScheduler;
        protected Animator _animator;

        public void Construct()
        {
            _mover = GetComponent<Mover>(); 
            _actionScheduler = GetComponent<ActionScheduler>();
            _animator = GetComponent<Animator>();
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
        
        public void EquipWeapon(Weapon weapon)
        {
            _equippedWeapon = weapon;
            weapon.SpawnToPlayer(_rightHandPosition, _leftHandPosition, _animator);
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



        public abstract void Hit();
        

        
        protected float GetRangeCurrentWeapon()
        {
            return _equippedWeapon.GetWeaponRange();
        }

        private bool InRange()
        {
            var distance =
                Mathf.Abs(Vector3.Distance(transform.position, Target.transform.position)); // TODO Vector3 Extensions

            return distance < GetRangeCurrentWeapon();
        }

        protected abstract void AttackBehavior();
        

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