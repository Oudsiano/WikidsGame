using FarrokhGames.Inventory.Examples;
using RPG.Core;
using RPG.Movement;
using UnityEngine;

namespace RPG.Combat
{
    public class Fighter : MonoBehaviour, IAction
    {
        [Header("Fighter Stats")]
        [Header("Weapon")]
        [SerializeField] private Transform rightHandPosition = null;
        [SerializeField] private Transform leftHandPosition = null;
        [SerializeField] private Weapon defaultWeapon = null;
        [SerializeField] private Weapon equippedWeapon = null;
        [SerializeField] private Weapon FireballWeapon = null;
        private bool isPlayer;
        private bool isFirebalNow = false;

        [Header("")]
        public float timer = 20;

        public Health target;

        private Mover mover;
        private ActionScheduler actionScheduler;
        private Animator anim;

        void Awake()
        {
            mover = GetComponent<Mover>();
            actionScheduler = GetComponent<ActionScheduler>();
            anim = GetComponent<Animator>();
            isPlayer = gameObject.GetComponent<MainPlayer>() ? true : false;

            if (isPlayer)
            {
                if (IGame.Instance.saveGame.EquipedWeapon != null)
                    EquipWeapon(IGame.Instance.saveGame.EquipedWeapon);
            }

            if (!equippedWeapon)
                EquipWeapon(defaultWeapon);
        }

        public void SetFirball()
        {
            isFirebalNow = true;
            FireballWeapon.SpawnToPlayer(rightHandPosition, leftHandPosition, anim);
        }

        public void SetCommonWeapon()
        {
            if (anim == null)
                Awake();

            isFirebalNow = false;
            if (equippedWeapon != null)
                equippedWeapon.SpawnToPlayer(rightHandPosition, leftHandPosition, anim);
        }

        public void EquipItem(ItemDefinition item)
        {
            if (item is Armor)
                ((Armor)item).EquipIt();
            else if (item is Weapon)
                EquipWeapon((Weapon)item);
        }

        public void EquipWeapon(Weapon weapon)
        {
            if (weapon.IsFireball())
            {
                FireballWeapon = weapon;
                return;
            }
            else
            {
                equippedWeapon = weapon;
                SetCommonWeapon();
            }

            if (isPlayer)
            {
                if (IGame.Instance != null)
                    IGame.Instance.saveGame.EquipedWeapon = weapon;
            }
        }

        public void UnequipWeapon()
        {
            if (equippedWeapon == defaultWeapon) return;
            equippedWeapon.DestroyWeaponOnPlayer(rightHandPosition, leftHandPosition, anim);
            EquipWeapon(defaultWeapon);
        }

        void Update()
        {
            if (IGame.Instance.IsPause) return;

            timer += Time.deltaTime;

            if (!target) return;

            if (!InRange())
            {
                mover.MoveTo(target.transform.position);
            }
            else
            {
                transform.LookAt(target.transform);
                mover.Cancel();
                AttackBehavior();
            }
        }

        private bool InRange()
        {
            var distance = Mathf.Abs(Vector3.Distance(transform.position, target.transform.position));
            return distance < equippedWeapon.GetWeaponRange();
        }

        private void AttackBehavior()
        {
            if (target.IsDead())
            {
                Cancel();
                actionScheduler.CancelAction();
            }
            else if (timer > equippedWeapon.GetTimeBetweenAttacks())
            {
                if (isPlayer && isFirebalNow)
                {
                    if (IGame.Instance.dataPLayer.playerData.chargeEnergy > 0)
                    {
                        MainPlayer.Instance.ChangeCountEnegry(-1);
                        shotFireball();
                        timer = 0;
                        return;
                    }
                    else
                    {
                        IGame.Instance.playerController.WeaponPanelUI.ResetWeaponToDefault();
                    }
                }

                anim.ResetTrigger("stopAttack");
                anim.SetTrigger("attack");

                if (equippedWeapon.IsRanged())
                    equippedWeapon.SpawnProjectile(target.transform, rightHandPosition, leftHandPosition);

                timer = 0;
            }
        }

        private void shotFireball()
        {
            anim.ResetTrigger("stopAttack");
            anim.SetTrigger("attack");
            FireballWeapon.SpawnProjectile(target.transform, rightHandPosition, leftHandPosition);
        }

        public void Cancel()
        {
            target = null;
            anim.SetTrigger("stopAttack");
        }

        public void Attack(GameObject combatTarget)
        {
            actionScheduler.StartAction(this);
            target = combatTarget.GetComponent<Health>();
        }

        public bool CanAttack(GameObject target)
        {
            return target && !target.GetComponent<Health>().IsDead();
        }

        private bool IsBehindTarget()
        {
            if (!target) return false;

            Vector3 directionToPlayer = (transform.position - target.transform.position).normalized;
            float angleBetween = Vector3.Angle(target.transform.forward, directionToPlayer);

            return angleBetween > 135f; // Угол, определяющий, что атака со спины (например, > 135 градусов)
        }

        public void Hit()
        {
            if (!target) return; // If there's no target, exit

            AudioManager.instance.PlaySound("Attack");

            if (IsBehindTarget() && !target.GetComponent<MainPlayer>()) // Check if the attack is from behind and the target is not the player
            {
                target.TakeDamage(target.GetCurrentHealth()); // Kill the target instantly
            }
            else
            {
                target.TakeDamage(equippedWeapon.GetWeaponDamage()); // Deal normal damage to the target
            }

            // Play VFX effect on hit
            Vector3 hitPosition = new Vector3(target.transform.position.x, target.transform.position.y + 1.5f, target.transform.position.z - 1); // Use target's position for VFX
            equippedWeapon.PlayHitVFX(hitPosition);
        }


        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            if (equippedWeapon)
                Gizmos.DrawWireSphere(transform.position, equippedWeapon.GetWeaponRange());
        }

        public void SetTarget(Health other)
        {
            target = other;
        }

        public object CaptureState()
        {
            return equippedWeapon.name;
        }

        public void RestoreState(object state)
        {
            string weaponName = (string)state;
            Weapon weapon = Resources.Load<Weapon>(weaponName);
            EquipWeapon(weapon);
        }
    }
}
