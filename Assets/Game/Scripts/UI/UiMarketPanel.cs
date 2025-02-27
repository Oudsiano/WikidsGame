using System;
using System.Collections.Generic;
using AINavigation;
using Combat.Data;
using Core;
using FarrokhGames.Inventory;
using Saving;
using SceneManagement;
using UI.Inventory;
using UI.Inventory.Data;
using UI.Inventory.Enums;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace UI
{
    public class UiMarketPanel : MonoBehaviour
    {
        [SerializeField] public TMPro.TMP_Text coinCountText;
        [SerializeField] public SizeInventoryExample InventoryAll;

        [SerializeField] public SizeInventoryExample InventoryBag;
        //[SerializeField] public SizeInventoryExample InventoryWeapon;
        //[SerializeField] public SizeInventoryExample InventoryArmor;

        [SerializeField] private GameObject _confirmPanel;
        [SerializeField] private Button _buttonAccept;
        [SerializeField] private Button _buttonDecline;

        [SerializeField] private Button _btnAll;
        [SerializeField] private Button _btnWeapons;
        [SerializeField] private Button _btnArmors;
        [SerializeField] private Button _btnConsume;
        [SerializeField] private Button _btnClose;

        private InventoryController _marketInventoryController;
        private ItemType _marketState;

        [FormerlySerializedAs("marketItems")] [SerializeField]
        private List<ItemDefinition> _marketItems;

        private Action<Vector2Int> _accept;
        private Action _decline;
        private Vector2Int _grid;
        private IInventoryItem _item;

        private float _angleTryOnEquip;
        private Weapon _oldWeaponWhenTryOn;
        private Armor _oldArmorWhenTryOn;

        private long _oldTime;
        private int _minPrice;
        private int _maxPrice;

        private bool _notAvaliableEvents = false;
        private PlayerController _playerController;
        private CoinManager _coinManager;
        private SaveGame _saveGame;
        private WeaponArmorManager _weaponArmorManager;
        private IGame _iGame;

        public void Construct(PlayerController playerController, CoinManager coinManager, SaveGame saveGame,
            WeaponArmorManager weaponArmorManager, IGame iGame) 
        {
            _playerController = playerController;
            _coinManager = coinManager;
            _saveGame = saveGame;
            _weaponArmorManager = weaponArmorManager;
            _iGame = iGame;
            
            _buttonAccept.onClick.AddListener(OnClickAccept);
            _buttonDecline.onClick.AddListener(OnClickDecline);

            _btnAll.onClick.AddListener(OnClickBtnAll);
            _btnWeapons.onClick.AddListener(OnClickBtnWeapons);
            _btnArmors.onClick.AddListener(OnClickBtnArmors);
            _btnConsume.onClick.AddListener(OnClickBtnConsume);
            _confirmPanel.SetActive(false);

            _btnClose.onClick.AddListener(OnClickClose);
        }

        private void Update()
        {
            if (_confirmPanel.gameObject.activeInHierarchy == false)
            {
                return;
            }

            if (_oldTime == 0)
            {
                _oldTime = NowTime();
            }

            _angleTryOnEquip += (NowTime() - _oldTime) * 0.05f;
            _oldTime = NowTime();
            
            if (_angleTryOnEquip > 360)
            {
                _angleTryOnEquip -= 360;
            }

            _playerController.ModularCharacter.transform.localRotation =
                Quaternion.Euler(0, _angleTryOnEquip, 0);
        }
        
        public void Init() // TODO constrct
        {
            _marketInventoryController = InventoryAll.GetComponent<InventoryController>();
            InventoryAll.Init();
            InventoryBag.Init();
            GenerateMarketItems();

            InventoryBag.inventory.onItemAdded += HandleItemBugAdded;
            InventoryBag.inventory.onItemRemoved += HandleItemBugRemoved;
            InventoryAll.inventory.onItemAdded += HandleItemAdded;
            InventoryAll.inventory.onItemRemoved += HandleItemRemoved;

            _coinManager.Coins.OnChangeCount += OnChangeMoney;
        }

        public void Regen(int _minPrice, int _maxPrice)
        {
            this._minPrice = _minPrice;
            this._maxPrice = _maxPrice;

            _notAvaliableEvents = true;
            InventoryBag.inventory.Clear();

            foreach (ItemDefinition item in _saveGame.BugItems)
            {
                InventoryBag.inventory.TryAdd(item);
            }


            ShowMarketItems(ItemType.Any);

            _notAvaliableEvents = false;
        }

        public void InitConfirmMarketUI(Action<Vector2Int> accept, Action decline, Vector2Int grid, IInventoryItem item)
        {
            _accept = accept;
            _decline = decline;
            _grid = grid;
            _item = item;

            if (_saveGame.Coins < _item.price)
            {
                _decline?.Invoke();

                return;
            }

            _oldWeaponWhenTryOn = _saveGame.EquipedWeapon;
            _oldArmorWhenTryOn = _saveGame.EquipedArmor;

            ItemDefinition newItem = _weaponArmorManager.TryGetItemByName(item.name);

            if (newItem.Type == ItemType.Armor)
            {
                ((Armor)newItem).EquipIt();
            }

            if (newItem.Type == ItemType.Weapons)
            {
                _playerController.GetFighter()
                    .EquipWeapon(_weaponArmorManager.TryGetWeaponByName(item.name));
            }

            _playerController.ModularCharacter.transform.localPosition =
                new Vector3(1000, 1000, 1000); // TODO magic numbers
            _angleTryOnEquip = 0;
            _playerController.ModularCharacter.transform.localRotation = Quaternion.Euler(0, 0, 0);

            _confirmPanel.SetActive(true);
            _saveGame.MakeSave();
        }

        public void SellItem(Action<Vector2Int> accept, Action decline, Vector2Int grid, IInventoryItem item)
        {
            _saveGame.Coins += (int)(item.price * InventoryBag.inventory.PriceMultiple);
            accept?.Invoke(grid);
            _saveGame.MakeSave();
        }

        private void GenerateMarketItems()
        {
            _marketItems = new List<ItemDefinition>();

            foreach (var item in _weaponArmorManager.AllWeaponsInGame)
            {
                if (item.sprite != null)
                {
                    _marketItems.Add(item);
                }
            }

            foreach (var item in _weaponArmorManager.AllArmorsInGame)
            {
                if (item.sprite != null)
                {
                    _marketItems.Add(item);
                }
            }
        }
        
        private long NowTime()
        {
            return (long)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalMilliseconds;
        }
        
        private void OnClickClose()
        {
            gameObject.SetActive(false);
            _iGame.SavePlayerPosLikeaPause(false);
            PauseClass.IsOpenUI = false;
        }

        private void OnClickBtnConsume()
        {
            ShowMarketItems(ItemType.Consume);
        }

        private void OnClickBtnArmors()
        {
            ShowMarketItems(ItemType.Armor);
        }

        private void OnClickBtnWeapons()
        {
            ShowMarketItems(ItemType.Weapons);
        }

        private void OnClickBtnAll()
        {
            ShowMarketItems(ItemType.Any);
        }

        private void OnChangeMoney(double newValue)
        {
            coinCountText.text = newValue.ToString();
        }

        private void HandleItemRemoved(IInventoryItem obj)
        {
            if (_notAvaliableEvents)
            {
                return;
            }

            foreach (ItemDefinition item in InventoryAll.inventory.allItems)
            {
                if (obj.position == item.position)
                {
                    _marketItems.Remove(item);
                }
            }
        }

        private void HandleItemBugRemoved(IInventoryItem obj)
        {
            if (_notAvaliableEvents)
            {
                return;
            }

            foreach (ItemDefinition item in _saveGame.BugItems)
            {
                if (item.sprite == obj.sprite)
                {
                    _saveGame.BugItems.Remove(item);

                    return;
                }
            }
        }

        private void HandleItemBugAdded(IInventoryItem obj)
        {
            if (_notAvaliableEvents)
            {
                return;
            }

            foreach (var item in _weaponArmorManager.AllWeaponsInGame)
            {
                if (item.sprite == obj.sprite)
                {
                    _saveGame.BugItems.Add(item);
                }
            }

            foreach (var item in _weaponArmorManager.AllArmorsInGame)
            {
                if (item.sprite == obj.sprite)
                {
                    _saveGame.BugItems.Add(item);
                }
            }
        }

        private void HandleItemAdded(IInventoryItem obj)
        {
            if (_notAvaliableEvents)
            {
                return;
            }

            foreach (var item in _weaponArmorManager.AllWeaponsInGame)
            {
                if (item.sprite == obj.sprite)
                {
                    _marketItems.Add(item);
                }
            }

            foreach (var item in _weaponArmorManager.AllArmorsInGame)
            {
                if (item.sprite == obj.sprite)
                {
                    _marketItems.Add(item);
                }
            }
        }

        private void HandleItemPickedUp(IInventoryItem obj) // TODO not used code
        {
        }

        private void ShowMarketItems(ItemType state)
        {
            _notAvaliableEvents = true;
            _marketState = state;
            InventoryAll.inventory.Clear();

            foreach (ItemDefinition item in _marketItems)
            {
                if (item.sprite != null)
                {
                    if (state == ItemType.Any || item.Type == state)
                    {
                        bool find = false;

                        foreach (var itemBug in InventoryBag.inventory.allItems)
                        {
                            if (itemBug.name == item.name)
                            {
                                find = true;

                                break;
                            }
                        }

                        if (_saveGame.EquipedArmor.name == item.name)
                        {
                            find = true;
                        }

                        if (_saveGame.EquipedWeapon.name == item.name)
                        {
                            find = true;
                        }

                        if (find == false)
                        {
                            if (item.price >= _minPrice && item.price <= _maxPrice)
                            {
                                InventoryAll.inventory.TryAdd(item);
                            }
                        }
                    }
                }
            }

            _notAvaliableEvents = false;
        }

        private void OnClickDecline()
        {
            _decline?.Invoke();
            _confirmPanel.SetActive(false);

            _oldArmorWhenTryOn.EquipIt();
            _playerController.GetFighter()
                .EquipWeapon(_weaponArmorManager.TryGetWeaponByName(_oldWeaponWhenTryOn.name));
        }

        private void OnClickAccept()
        {
            _accept?.Invoke(_grid);
            _confirmPanel.SetActive(false);

            _saveGame.Coins -= _item.price;

            _oldArmorWhenTryOn.EquipIt();
            _playerController.GetFighter()
                .EquipWeapon(_weaponArmorManager.TryGetWeaponByName(_oldWeaponWhenTryOn.name));
        }

        private void OnDestroy()
        {
            if (_marketInventoryController != null)
            {
                //marketInventoryController.onItemHovered -= HandleItemPickedUp;
                InventoryAll.inventory.onItemAdded -= HandleItemAdded;
                InventoryAll.inventory.onItemRemoved -= HandleItemRemoved;
                InventoryBag.inventory.onItemAdded -= HandleItemBugAdded;
                InventoryBag.inventory.onItemRemoved -= HandleItemBugRemoved;
            }

            _coinManager.Coins.OnChangeCount -= OnChangeMoney;
            _btnClose.onClick.RemoveAllListeners();

            _buttonAccept.onClick.RemoveAllListeners();
            _buttonDecline.onClick.RemoveAllListeners();

            _btnAll.onClick.RemoveAllListeners();
            _btnWeapons.onClick.RemoveAllListeners();
            _btnArmors.onClick.RemoveAllListeners();
            _btnConsume.onClick.RemoveAllListeners();
        }
    }
}