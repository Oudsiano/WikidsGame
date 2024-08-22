using System;
using FarrokhGames.Inventory;
using FarrokhGames.Inventory.Examples;
using UnityEngine;

namespace RPG.Combat
{
    [CreateAssetMenu(fileName = "Weapon", menuName = "Weapons", order = 0)]
    public class Weapon : ItemDefinition
    {
        [Header("Core")]
        [SerializeField] private AnimatorOverrideController weaponOverride;
        [SerializeField] private GameObject weaponPrefab;
        [SerializeField] private bool isRightHanded = true;
        [SerializeField] private Projectile projectile;
        [SerializeField] private bool isFireballs = false;
        [SerializeField] private GameObject hitVFX;

        [Header("Stats")]
        [SerializeField] private float weaponDamage;
        [SerializeField] private float weaponRange = 2f;
        [SerializeField] private float timeBetweenAttacks;

        [Header("Description")]
        [SerializeField] [TextArea] private string description;

        [Header("Charges")]
        [SerializeField] private int maxCharges = 10;
        public int currentCharges;

        private const string weaponNameForHand = "weapon";
        public event Action OnShotFired; // Событие для уведомления о выстреле

        public GameObject WeaponPrefab { get => weaponPrefab; set => weaponPrefab = value; }

        public void InitializeWeapon()
        {
            currentCharges = maxCharges;
        }

        public void SpawnToPlayer(Transform rightHandPos, Transform lefthandPos, Animator anim)
        {
            DestroyWeaponOnPlayer(rightHandPos, lefthandPos, anim);

            if (WeaponPrefab)
            {
                Transform handPos = FindTransformOfHand(rightHandPos, lefthandPos);
                GameObject wepInScene = Instantiate(WeaponPrefab, handPos);
                wepInScene.transform.localScale = Vector3.one * 1 / wepInScene.transform.lossyScale.x;
                wepInScene.name = weaponNameForHand;
            }

            var overrideController = anim.runtimeAnimatorController as AnimatorOverrideController;
            if (weaponOverride)
                anim.runtimeAnimatorController = weaponOverride;
            else if (overrideController)
            {
                anim.runtimeAnimatorController = overrideController.runtimeAnimatorController;
            }
        }

        private Transform FindTransformOfHand(Transform rightHandPos, Transform lefthandPos)
        {
            return isRightHanded ? rightHandPos : lefthandPos;
        }

        public void DestroyWeaponOnPlayer(Transform rightHandPos, Transform leftHandPos, Animator anim)
        {
            DestroyWeaponOnHand(rightHandPos);
            DestroyWeaponOnHand(leftHandPos);
        }

        private void DestroyWeaponOnHand(Transform handPos)
        {
            Transform handWep = handPos.Find(weaponNameForHand);
            if (handWep)
            {
                handWep.name = "DESTROYING";
                Destroy(handWep.gameObject);
            }
        }

        public float GetWeaponDamage()
        {
            return weaponDamage;
        }

        public float GetWeaponRange()
        {
            return weaponRange;
        }

        public float GetTimeBetweenAttacks()
        {
            return timeBetweenAttacks;
        }

        public string GetDescription()
        {
            return description;
        }

        public void SpawnProjectile(Transform target, Transform rightHand, Transform leftHand, bool isPlayer)
        {
            if(isFireballs)
                currentCharges = IGame.Instance.dataPlayer.playerData.chargeEnergy + 1;

            if (isPlayer && !ConsumeCharge())
            {
                Debug.Log("Out of charges!");
                return;
            }

            var proj = Instantiate(projectile, FindTransformOfHand(rightHand, leftHand).position, Quaternion.identity);
            proj.SetTarget(target, weaponDamage);

            AudioManager.instance.PlaySound("Shot");
            OnShotFired?.Invoke(); // Вызов события
        }

        private bool ConsumeCharge()
        {
            if (currentCharges > 0)
            {
                currentCharges--;
                Debug.Log("Charges left: " + currentCharges);
                if (currentCharges == 0)
                {
                    OnOutOfCharges();
                }
                return true;
            }
            Debug.Log("No charges left!");
            OnOutOfCharges();
            return false;
        }

        private void OnOutOfCharges()
        {
            // Получаем ссылку на WeaponPanelUI и вызываем ResetWeaponToDefault
            WeaponPanelUI weaponPanelUI = FindObjectOfType<WeaponPanelUI>();
            if (weaponPanelUI != null)
            {
                weaponPanelUI.ResetWeaponToDefault();
            }
        }

        public void ReloadCharges(int charges)
        {
            currentCharges = Mathf.Min(currentCharges + charges, maxCharges);
            Debug.Log("Charges reloaded. Current charges: " + currentCharges);
        }

        public void SetFireball()
        {
            isFireballs=true;
        }
        public bool IsFireball()
        {
            return isFireballs;
        }

        public bool IsRanged()
        {
            return projectile != null;
        }

        public void PlayHitVFX(Vector3 position)
        {
            if (hitVFX != null)
            {
                GameObject vfx = Instantiate(hitVFX, position, Quaternion.identity);
                Destroy(vfx, vfx.GetComponent<ParticleSystem>().main.duration); // Уничтожаем VFX после завершения
            }
        }

        public int GetCurrentCharges()
        {
            return currentCharges;
        }
    }
}
