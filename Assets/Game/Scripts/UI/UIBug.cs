using AINavigation;
using Combat;
using Combat.Data;
using Core;
using FarrokhGames.Inventory;
using Saving;
using UI.Inventory;
using UI.Inventory.Data;
using UI.Inventory.Enums;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace UI
{
    public class UIBug : MonoBehaviour // TODO rename => bAg // TODO overload class
    {
        [SerializeField] public SizeInventoryExample InventoryBag; // TODO OC error

        [FormerlySerializedAs("InventoryWeapon")] [SerializeField]
        private SizeInventoryExample _inventoryWeapon;

        [FormerlySerializedAs("InventoryArmor")] [SerializeField]
        private SizeInventoryExample _inventoryArmor;

        [FormerlySerializedAs("DropPlace")] [SerializeField]
        private SizeInventoryExample _dropPlace;

        [FormerlySerializedAs("btnClose")] [SerializeField]
        private Button _buttonClose;

        private InventoryController _marketInventoryController;

        private bool _notAvaliableEvents = false;
        private bool _inited = false;

        private bool _noAddToSave;

        private IGame _iGame;
        private SaveGame _saveGame;
        private PlayerController _playerController;
        private WeaponArmorManager _weaponArmorManager;
        
        public void Construct(IGame iGame, SaveGame saveGame, PlayerController playerController, WeaponArmorManager weaponArmorManager) // TODO construct
        {
            _iGame = iGame;
            _saveGame = saveGame;
            _playerController = playerController;
            _weaponArmorManager = weaponArmorManager;
            
            _marketInventoryController = InventoryBag.GetComponent<InventoryController>();
            InventoryBag.Init();
            _inventoryArmor.Init();
            _inventoryWeapon.Init();
            _dropPlace.Init();
            _inited = true;
            _buttonClose.onClick.AddListener(OnClickClose);
        }

        private void Start() // TODO construct
        {
            InventoryBag.inventory.onItemAdded += OnAdded;
            InventoryBag.inventory.onItemRemoved += OnRemoved;

            InventoryBag.inventory.onItemDropped += OnDrop;


            _inventoryWeapon.inventory.onItemAdded += OnAddedWeapon;
            _inventoryWeapon.inventory.onItemRemoved += OnRemovedWeapon;

            _inventoryArmor.inventory.onItemAdded += OnAddedArmor;
            _inventoryArmor.inventory.onItemRemoved += OnRemovedArmor;

            _dropPlace.inventory.onItemAdded += OnAddedDropPlace;

            Regen();
        }

        private void Update()
        {
            _dropPlace.inventory
                .DropAll();
        }

        private void OnDestroy()
        {
            if (!_inited) return;
            InventoryBag.inventory.onItemAdded -= OnAdded;
            InventoryBag.inventory.onItemRemoved -= OnRemoved;
            _inventoryWeapon.inventory.onItemAdded -= OnAddedWeapon;
            _inventoryWeapon.inventory.onItemRemoved -= OnRemovedWeapon;
            _inventoryArmor.inventory.onItemAdded -= OnAddedArmor;
            _inventoryArmor.inventory.onItemRemoved -= OnRemovedArmor;

            _buttonClose.onClick.RemoveAllListeners();
        }

        public bool TryTakeQuestItem(string itemName, bool needDelete = false)
        {
            foreach (var item in InventoryBag.inventory.allItems)
            {
                if (item.name == itemName)
                {
                    if (needDelete)
                    {
                        OnRemoved(item);
                    }

                    return true;
                }
            }

            return false;
        }

        public void NeedDeleteItem(string itemName)
        {
            foreach (var item in InventoryBag.inventory.allItems)
            {
                if (item.name == itemName)
                {
                    OnRemoved(item);
                }
            }
        }

        public bool AddEquipInBugIfNotExist(ItemDefinition item)
        {
            if (InventoryBag.inventory.Contains(item))
            {
                return false;
            }

            if (InventoryBag.inventory.CanAdd(item))
            {
                _noAddToSave = true;

                if (InventoryBag.inventory.TryAdd(item.CreateInstance()))
                {
                    _saveGame.SaveItemToBug(item);
                    _noAddToSave = false;

                    return true;
                }

                _noAddToSave = false;

                return false;
            }

            return false;
        }

        public void TryAddEquipToBug(ItemDefinition item)
        {
            _notAvaliableEvents = true;

            if (item.onlyOne && InventoryBag.inventory.Contains(item))
            {
                return;
            }

            if (InventoryBag.inventory.CanAdd(item))
            {
                _saveGame.SaveItemToBug(item);
                InventoryBag.inventory.TryAdd(item.CreateInstance());
            }
            else
            {
                DropItemNearPlayer(item);
            }

            _notAvaliableEvents = false;
        }

        public void Regen() // TODO check idk what is it
        {
            _notAvaliableEvents = true;
            InventoryBag.inventory.Clear();

            foreach (ItemDefinition item in _saveGame.BugItems)
            {
                InventoryBag.inventory.TryAdd(item);
            }

            _inventoryWeapon.inventory.Clear();
            if (_saveGame.EquipedWeapon.sprite != null)
            {
                _inventoryWeapon.inventory.TryAdd(_saveGame.EquipedWeapon.CreateInstance());
            }

            _inventoryArmor.inventory.Clear();

            if (_saveGame.EquipedArmor.sprite != null)
            {
                _inventoryArmor.inventory.TryAdd(_saveGame.EquipedArmor.CreateInstance());
            }

            _notAvaliableEvents = false;
        }

        private void OnClickClose()
        {
            gameObject.SetActive(false);
            _iGame.SavePlayerPosLikeaPause(false);
            PauseClass.IsOpenUI = false;
        }

        private void DropItemNearPlayer(ItemDefinition item)
        {
            _notAvaliableEvents = true;

            float posX = _playerController.transform.localPosition.x + // TODO magic numbers
                         UnityEngine.Random.Range(-10, 10) * 0.1f;
            float posY = _playerController.transform.localPosition.y + // TODO magic numbers
                         UnityEngine.Random.Range(-10, 10) * 0.1f;


            if (item.Type == ItemType.Weapons)
            {
                Weapon _tempW = _weaponArmorManager.TryGetWeaponByName(item.Name);
                GameObject newObj = Instantiate(_tempW.WeaponPrefab,
                    new Vector3(posX, posY + 1,
                        _playerController.transform.localPosition.z), // TODO magic numbers
                    Quaternion.Euler(0, 0, 0));

                PickableEquip _tempPE = newObj.GetComponent<PickableEquip>();

                if (_tempPE == null)
                {
                    _tempPE = newObj.AddComponent<PickableEquip>();
                }

                SphereCollider sphereCollider = newObj.AddComponent<SphereCollider>();
                sphereCollider.isTrigger = true;
                sphereCollider.center = new Vector3(0, 0, 0); // TODO magic numbers
                sphereCollider.radius = 0.6684607f; // TODO magic numbers

                RotationObjects rotationObjects = newObj.AddComponent<RotationObjects>();
                rotationObjects.rotationAxis = new Vector3(0, 1, 0); // TODO magic numbers
                rotationObjects.rotationDuration = 5; // TODO magic numbers
                rotationObjects.easeType = DG.Tweening.Ease.Linear;

                _tempPE.SetItem(item);
            }
            else
            {
                Instantiate(_weaponArmorManager.DefaultPrefab,
                        new Vector3(posX, posY + 1, _playerController.transform.localPosition.z),
                        Quaternion.Euler(0, 0, 0))
                    .GetComponent<PickableEquip>().SetItem(item);
            }

            _notAvaliableEvents = false;
        }

        private void OnAddedDropPlace(IInventoryItem obj)
        {
            DropItemNearPlayer((ItemDefinition)obj);
        }

        private void OnDrop(IInventoryItem obj)
        {
            DropItemNearPlayer((ItemDefinition)obj);
        }

        private void OnRemovedArmor(IInventoryItem obj)
        {
            if (_notAvaliableEvents) return;
            _weaponArmorManager.TryGetArmorByName(obj.name).UnEquip();
            _saveGame.MakeSave();
        }

        private void OnAddedArmor(IInventoryItem obj)
        {
            if (_notAvaliableEvents) return;
            _weaponArmorManager.TryGetArmorByName(obj.name).EquipIt();
            _saveGame.MakeSave();
        }

        private void OnRemovedWeapon(IInventoryItem obj)
        {
            if (_notAvaliableEvents) return;
            _playerController.GetFighter().UnequipWeapon();
            _saveGame.MakeSave();
        }

        private void OnAddedWeapon(IInventoryItem obj)
        {
            if (_notAvaliableEvents)
            {
                return;
            }

            _playerController.GetFighter()
                .EquipWeapon(_weaponArmorManager.TryGetWeaponByName(obj.name));
            _saveGame.MakeSave();
        }

        private void OnRemoved(IInventoryItem obj)
        {
            if (_notAvaliableEvents)
            {
                return;
            }

            _saveGame.BugItems.Remove((ItemDefinition)obj);
            _saveGame.MakeSave();
        }

        private void OnAdded(IInventoryItem obj)
        {
            if (_notAvaliableEvents)
            {
                return;
            }

            if (_noAddToSave == false)
            {
                _saveGame.BugItems.Add((ItemDefinition)obj);
            }

            _saveGame.MakeSave();
        }
    }
}