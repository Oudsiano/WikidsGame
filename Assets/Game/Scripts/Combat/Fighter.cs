using FarrokhGames.Inventory.Examples;
using Movement;
using RPG.Core;
using UnityEngine;

namespace RPG.Combat
{

    public enum WeaponNow
    {
        common,
        fire,
        bow
    }

    public class Fighter : MonoBehaviour, IAction
    {
        [Header("Fighter Stats")]
        [Header("Weapon")]
        [SerializeField] private Transform rightHandPosition = null;
        [SerializeField] private Transform leftHandPosition = null;
        [SerializeField] private Weapon defaultWeapon = null;
        [SerializeField] private Weapon equippedWeapon = null;
        [SerializeField] private Weapon fireballWeapon = null;
        [SerializeField] private Weapon bowWeapon = null;
        private bool isPlayer;
        private WeaponNow weaponNow;
        //private bool isFireballNow = false;

        [Header("")]
        public float timer = 20;

        public Health target;
        private Fighter targetF;

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

        void OnMouseEnter()
        {
            if (!isPlayer && !GetComponent<Health>().IsDead())
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
            weaponNow = WeaponNow.fire;
            fireballWeapon.SetFireball();
            if (!fireballWeapon.IsFireball()) Debug.LogError("нет галочки");
            fireballWeapon.SpawnToPlayer(rightHandPosition, leftHandPosition, anim);
        }

        public void SetCommonWeapon()
        {
            if (anim == null)
                Awake();

            weaponNow = WeaponNow.common;
            if (equippedWeapon != null)
                equippedWeapon.SpawnToPlayer(rightHandPosition, leftHandPosition, anim);
        }

        public void SetBow()
        {
            if (anim == null)
                Awake();

            weaponNow = WeaponNow.bow;
            if (bowWeapon != null)
                bowWeapon.SpawnToPlayer(rightHandPosition, leftHandPosition, anim);
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
                fireballWeapon = weapon;
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
            if (pauseClass.GetPauseState()) return;

            if (ConversationStarter.IsDialogActive) return;

            timer += Time.deltaTime;

            if (!target) return;
            //Archer
            //if (targetF?.defaultWeapon == bowWeapon && (bowWeapon.currentCharges == 0 || weaponNow != WeaponNow.bow))
            //    return;

            if (!InRange())
            {
                mover.MoveTo(target.transform.position);
            }
            else
            {
                Vector3 lookAt = target.transform.position;
                lookAt.y = transform.position.y;
                transform.LookAt(lookAt, Vector3.up);
                mover.Cancel();
                AttackBehavior();
            }
        }

        private float getRangeCurrentWeapon()
        {
            switch (weaponNow)
            {
                case WeaponNow.common:
                    return equippedWeapon.GetWeaponRange();
                case WeaponNow.fire:
                    return fireballWeapon.GetWeaponRange();
                case WeaponNow.bow:
                    return bowWeapon.GetWeaponRange();
                default:
                    return equippedWeapon.GetWeaponRange();
            }
        }


        private bool InRange()
        {
            var distance = Mathf.Abs(Vector3.Distance(transform.position, target.transform.position));


            return distance < getRangeCurrentWeapon();
        }

        private void AttackBehavior()
        {
            if (isPlayer)
                isPlayer = true;

            if (target.IsDead())
            {
                Cancel();
                actionScheduler.CancelAction();
            }
            else if (timer > equippedWeapon.GetTimeBetweenAttacks())
            {
                if (isPlayer && weaponNow == WeaponNow.fire)
                {
                    if (IGame.Instance.dataPlayer.playerData.chargeEnergy > 0)
                    {
                        MainPlayer.Instance.ChangeCountEnegry(-1);
                        ShootFireball();
                        timer = 0;
                        return;
                    }
                    else
                    {
                        IGame.Instance.playerController.WeaponPanelUI.ResetWeaponToDefault();
                    }
                }

                if (isPlayer && weaponNow == WeaponNow.bow)
                {
                    if (bowWeapon.currentCharges > 0)
                    {
                        ShootBow();
                        timer = 0;
                        return;
                    }
                    else
                    {
                        IGame.Instance.playerController.WeaponPanelUI.ResetWeaponToDefault();
                    }
                }
                if (isPlayer && weaponNow != WeaponNow.bow)
                {
                    if (targetF.defaultWeapon == bowWeapon)
                        return;
                }

                if (isPlayer)
                    isPlayer = true;

                anim.ResetTrigger("stopAttack");
                anim.SetTrigger("attack");

                if (equippedWeapon.IsRanged())
                    equippedWeapon.SpawnProjectile(target.transform, rightHandPosition, leftHandPosition, isPlayer);

                timer = 0;
            }
        }

        private void ShootFireball()
        {
            anim.ResetTrigger("stopAttack");
            anim.SetTrigger("attack");
            fireballWeapon.SpawnProjectile(target.transform, rightHandPosition, leftHandPosition, isPlayer);
        }
        private void ShootBow()
        {
            anim.ResetTrigger("stopAttack");
            anim.SetTrigger("attack");
            bowWeapon.SpawnProjectile(target.transform, rightHandPosition, leftHandPosition, isPlayer);
        }

        public void Cancel()
        {
            target = null;
            anim.SetTrigger("stopAttack");
        }

        public void Attack(GameObject combatTarget)
        {
            if (ConversationStarter.IsDialogActive) return;

            actionScheduler.StartAction(this);
            target = combatTarget.GetComponent<Health>();
            targetF = target.GetComponent<Fighter>();
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

            return angleBetween > 120f; // Угол, определяющий, что атака со спины (например, > 135 градусов)
        }

        public void Hit()
        {
            if (!target) return; // Если цели нет, выйти

            if (isPlayer && weaponNow != WeaponNow.bow)
            {
                if (targetF.defaultWeapon == bowWeapon)
                    return;
            }

            AudioManager.instance.PlaySound("Attack");

            if (IsBehindTarget() && !target.GetComponent<MainPlayer>() && !target.GetComponent<Boss>()) // Проверка, если атака сзади и цель не игрок и не босс
            {
                target.AttackFromBehind(false);
            }
            else
            {
                target.TakeDamage(equippedWeapon.GetWeaponDamage()); // Нанести нормальный урон цели
            }

            // Проиграть эффект при попадании
            Vector3 hitPosition = new Vector3(target.transform.position.x, target.transform.position.y + 1.5f, target.transform.position.z - 1); // Использовать позицию цели для VFX
            equippedWeapon.PlayHitVFX(hitPosition);

            // Проверить, жива ли цель
            if (target.IsDead())
            {
                Animator targetAnim = target.GetComponent<Animator>();
                if (targetAnim != null)
                {
                    targetAnim.SetTrigger("dead"); // Запуск анимации смерти
                }
            }
            else
            {
                // Запустить анимацию получения урона у цели
                Animator targetAnim = target.GetComponent<Animator>();
                if (targetAnim != null)
                {
                    targetAnim.SetTrigger("takeDamage");
                }
            }
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
