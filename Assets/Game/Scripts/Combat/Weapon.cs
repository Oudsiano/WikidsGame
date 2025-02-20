using System;
using FarrokhGames.Inventory.Examples;
using UnityEngine;
using UnityEngine.Serialization;

namespace Combat
{
    [CreateAssetMenu(fileName = "Weapon", menuName = "Weapons", order = 0)]
    public class Weapon : ItemDefinition // TODO data -> its a data(SO)
    {
        private const string WeaponNameForHand = "weapon"; 
        
        [FormerlySerializedAs("weaponOverride")] [Header("Core")] [SerializeField]
        private AnimatorOverrideController _weaponOverride;

        [FormerlySerializedAs("weaponPrefab")] [SerializeField]
        private GameObject _weaponPrefab;

        [FormerlySerializedAs("isRightHanded")] [SerializeField]
        private bool _isRightHanded = true;

        [FormerlySerializedAs("projectile")] [SerializeField]
        private Projectile _projectile;

        [FormerlySerializedAs("isFireballs")] [SerializeField]
        private bool _isFireballs = false;

        [FormerlySerializedAs("hitVFX")] [SerializeField]
        private GameObject _hitVFX;

        [FormerlySerializedAs("weaponDamage")] [Header("Stats")] [SerializeField]
        private float _weaponDamage;

        [FormerlySerializedAs("weaponRange")] [SerializeField]
        private float _weaponRange = 2f;

        [FormerlySerializedAs("timeBetweenAttacks")] [SerializeField]
        private float _timeBetweenAttacks;

        [FormerlySerializedAs("description")] [Header("Description")] [SerializeField] [TextArea]
        private string _description;

        [Header("Charges")] [SerializeField] private int maxCharges = 10;
        [FormerlySerializedAs("currentCharges")] public int _currentCharges;

        public event Action Fired; // Событие для уведомления о выстреле

        public GameObject WeaponPrefab // TODO GO
        {
            get => _weaponPrefab;
            set => _weaponPrefab = value;
        }

        // public void InitializeWeapon() // TODO not used code
        // {
        //     _currentCharges = maxCharges;
        // }

        public void SpawnToPlayer(Transform rightHandPos, Transform lefthandPos, Animator animator) 
            // TODO cannot be dynamically used 
        {
            DestroyWeaponOnPlayer(rightHandPos, lefthandPos, animator);

            if (WeaponPrefab)
            {
                Transform handPos = FindTransformOfHand(rightHandPos, lefthandPos);
                GameObject wepInScene = Instantiate(WeaponPrefab, handPos);
                wepInScene.transform.localScale = Vector3.one * 1 / wepInScene.transform.lossyScale.x;
                wepInScene.name = WeaponNameForHand;
            }

            var overrideController = animator.runtimeAnimatorController as AnimatorOverrideController;

            if (_weaponOverride)
            {
                animator.runtimeAnimatorController = _weaponOverride;
            }
            else if (overrideController)
            {
                animator.runtimeAnimatorController = overrideController.runtimeAnimatorController;
            }
        }

        private Transform FindTransformOfHand(Transform rightHandPos, Transform lefthandPos)
            // TODO cannot be dynamically used 
        {
            return _isRightHanded ? rightHandPos : lefthandPos;
        }

        public void DestroyWeaponOnPlayer(Transform rightHandPos, Transform leftHandPos,
            Animator anim) // TODO cannot be dynamically used 
        {
            DestroyWeaponOnHand(rightHandPos);
            DestroyWeaponOnHand(leftHandPos);
        }

        private void DestroyWeaponOnHand(Transform handPos) // TODO cannot be dynamically used 
        {
            Transform handWep = handPos.Find(WeaponNameForHand);
            if (handWep)
            {
                handWep.name = "DESTROYING";
                Destroy(handWep.gameObject);
            }
        }

        public float GetWeaponDamage() // TODO cannot be dynamically used 
        {
            return _weaponDamage;
        }

        public float GetWeaponRange() // TODO cannot be dynamically used 
        {
            return _weaponRange;
        }

        public float GetTimeBetweenAttacks() // TODO cannot be dynamically used 
        {
            return _timeBetweenAttacks;
        }

        public string GetDescription() // TODO cannot be dynamically used 
        {
            return _description;
        }

        public void SpawnProjectile(Transform target, Transform rightHand, Transform leftHand,
            bool isPlayer) // TODO cannot be dynamically used 
        {
            if (_isFireballs)
                _currentCharges = IGame.Instance.dataPlayer.playerData.chargeEnergy + 1; // TODO magic number

            if (isPlayer && ConsumeCharge() == false)
            {
                Debug.Log("Out of charges!");
                return;
            }

            var proj = Instantiate(_projectile, FindTransformOfHand(rightHand, leftHand).position, Quaternion.identity);
            proj.SetTarget(target, _weaponDamage);

            AudioManager.instance.PlaySound("Shot");
            Fired?.Invoke(); // Вызов события
        }

        private bool ConsumeCharge() // TODO cannot be dynamically used 
        {
            if (_currentCharges > 0)
            {
                _currentCharges--;
                Debug.Log("Charges left: " + _currentCharges);
                /*if (currentCharges == 0)
                {
                    OnOutOfCharges();
                }*/

                return true;
            }

            Debug.Log("No charges left!");
            OnOutOfCharges();

            return false;
        }

        private void OnOutOfCharges() // TODO cannot be dynamically used 
        {
            // Получаем ссылку на WeaponPanelUI и вызываем ResetWeaponToDefault
            WeaponPanelUI weaponPanelUI = FindObjectOfType<WeaponPanelUI>();
            if (weaponPanelUI != null)
            {
                weaponPanelUI.ResetWeaponToDefault();
            }
        }

        public void ReloadCharges(int charges) // TODO cannot be dynamically used 
        {
            _currentCharges = Mathf.Min(_currentCharges + charges, maxCharges);
            Debug.Log("Charges reloaded. Current charges: " + _currentCharges);
        }

        public void SetFireball() // TODO cannot be dynamically used 
        {
            _isFireballs = true;
        }

        public bool IsFireball() // TODO cannot be dynamically used 
        {
            return _isFireballs;
        }

        public bool IsRanged() // TODO cannot be dynamically used 
        {
            return _projectile != null;
        }

        public void PlayHitVFX(Vector3 position) // TODO cannot be dynamically used 
        {
            if (_hitVFX != null)
            {
                GameObject vfx = Instantiate(_hitVFX, position, Quaternion.identity);
                Destroy(vfx,
                    vfx.GetComponent<ParticleSystem>().main
                        .duration); // Уничтожаем VFX после завершения // TODO GETCOMP
            }
        }

        public int GetCurrentCharges() // TODO cannot be dynamically used 
        {
            return _currentCharges;
        }
    }
}