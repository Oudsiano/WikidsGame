using FarrokhGames.Inventory;
using FarrokhGames.Inventory.Examples;
using RPG.Combat;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;




public class UiMarketPanel : MonoBehaviour
{
    [SerializeField] private GameObject _confirmPanel;
    [SerializeField] private Button _buttonAccept;
    [SerializeField] private Button _buttonDecline;

    [SerializeField] public SizeInventoryExample InventoryAll;
    private InventoryController marketInventoryController;
    [SerializeField] public SizeInventoryExample InventoryBag;
    //[SerializeField] public SizeInventoryExample InventoryWeapon;
    //[SerializeField] public SizeInventoryExample InventoryArmor;

    [SerializeField] private Button _btnAll;
    [SerializeField] private Button _btnWeapons;
    [SerializeField] private Button _btnArmors;
    [SerializeField] private Button _btnConsume;
    [SerializeField] private Button _btnClose;

    [SerializeField] public TMPro.TMP_Text coinCountText;

    private ItemType marketState;
    public List<ItemDefinition> marketItems;

    private Action<Vector2Int> _accept;
    private Action _decline;
    private Vector2Int _grid;
    private IInventoryItem _item;

    private float angleTryOnEquip;
    private Weapon oldWeaponWhenTryOn;
    private Armor oldArmorWhenTryOn;


    private bool notAvaliableEvents = false;

    private void Awake()
    {
        _buttonAccept.onClick.AddListener(OnClickAccept);
        _buttonDecline.onClick.AddListener(OnClickDecline);

        _btnAll.onClick.AddListener(OnClickBtnAll);
        _btnWeapons.onClick.AddListener(OnClickBtnWeapons);
        _btnArmors.onClick.AddListener(OnClickBtnArmors);
        _btnConsume.onClick.AddListener(OnClickBtnConsume);
        _confirmPanel.SetActive(false);

        _btnClose.onClick.AddListener(OnClickClose);
    }

    private void OnClickClose()
    {
        gameObject.SetActive(false);
        IGame.Instance.SetPause(false);
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

    public void Init()
    {
        marketInventoryController = InventoryAll.GetComponent<InventoryController>();
        InventoryAll.Init();
        InventoryBag.Init();
        GenerateMarketItems();

        InventoryBag.inventory.onItemAdded += HandleItemBugAdded;
        InventoryBag.inventory.onItemRemoved += HandleItemBugRemoved;
        InventoryAll.inventory.onItemAdded += HandleItemAdded;
        InventoryAll.inventory.onItemRemoved += HandleItemRemoved;

        IGame.Instance.CoinManager.Coins.OnChangeCount += OnChangeMoney;
    }

    public void Regen()
    {
        notAvaliableEvents = true;
        InventoryBag.inventory.Clear();
        foreach (ItemDefinition item in IGame.Instance.saveGame.BugItems)
        {
            InventoryBag.inventory.TryAdd(item);
        }


        ShowMarketItems(ItemType.Any);

        notAvaliableEvents = false;
    }

    public void SellItem(Action<Vector2Int> accept, Action decline, Vector2Int grid, IInventoryItem item)
    {
        IGame.Instance.saveGame.Coins += (int)(item.price * InventoryBag.inventory.PriceMultiple);
        accept?.Invoke(grid);
    }

    public void GenerateMarketItems()
    {
        marketItems = new List<ItemDefinition>();
        foreach (var item in IGame.Instance.WeaponArmorManager.AllWeaponsInGame)
        {
            if (item.sprite != null)

                marketItems.Add(item);
        }
        foreach (var item in IGame.Instance.WeaponArmorManager.AllArmorsInGame)
        {
            if (item.sprite != null)
                marketItems.Add(item);
        }
    }

    

    private void OnChangeMoney(double newValue)
    {
        coinCountText.text = newValue.ToString();
    }
    private void HandleItemRemoved(IInventoryItem obj)
    {
        if (notAvaliableEvents) return;
        foreach (ItemDefinition item in InventoryAll.inventory.allItems)
        {
            if (obj.position == item.position)
            {
                marketItems.Remove(item);
            }
        }
    }

    private void HandleItemBugRemoved(IInventoryItem obj)
    {
        if (notAvaliableEvents) return;
        foreach (ItemDefinition item in IGame.Instance.saveGame.BugItems)
        {
            if (item.sprite == obj.sprite)
            {
                IGame.Instance.saveGame.BugItems.Remove(item);
                return;
            }
        }
    }

    private void HandleItemBugAdded(IInventoryItem obj)
    {
        if (notAvaliableEvents) return;
        foreach (var item in IGame.Instance.WeaponArmorManager.AllWeaponsInGame)
        {
            if (item.sprite == obj.sprite)
                IGame.Instance.saveGame.BugItems.Add(item);
        }
        foreach (var item in IGame.Instance.WeaponArmorManager.AllArmorsInGame)
        {
            if (item.sprite == obj.sprite)
                IGame.Instance.saveGame.BugItems.Add(item);
        }
    }


    private void HandleItemAdded(IInventoryItem obj)
    {
        if (notAvaliableEvents) return;

        foreach (var item in IGame.Instance.WeaponArmorManager.AllWeaponsInGame)
        {
            if (item.sprite == obj.sprite)
                marketItems.Add(item);
        }
        foreach (var item in IGame.Instance.WeaponArmorManager.AllArmorsInGame)
        {
            if (item.sprite == obj.sprite)
                marketItems.Add(item);
        }
    }

    private void HandleItemPickedUp(IInventoryItem obj)
    {


    }

    public void ShowMarketItems(ItemType state)
    {
        notAvaliableEvents = true;
        marketState = state;
        InventoryAll.inventory.Clear();

        foreach (ItemDefinition item in marketItems)
        {
            if (item.sprite != null)
            {
                if ((state == ItemType.Any) || (item.Type == state))
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

                    if (IGame.Instance.saveGame.EquipedArmor.name == item.name)
                        find = true;
                    if (IGame.Instance.saveGame.EquipedWeapon.name == item.name)
                        find = true;

                    if (!find)
                    InventoryAll.inventory.TryAdd(item);
                }
            }
        }
        notAvaliableEvents = false;
    }

    public void InitConfirmMarketUI(Action<Vector2Int> accept, Action decline, Vector2Int grid, IInventoryItem item)
    {
        _accept = accept;
        _decline = decline;
        _grid = grid;
        _item = item;

        if (IGame.Instance.saveGame.Coins < _item.price)
        {
            _decline?.Invoke();
            return;
        }

        oldWeaponWhenTryOn = IGame.Instance.saveGame.EquipedWeapon;
        oldArmorWhenTryOn = IGame.Instance.saveGame.EquipedArmor;

        ItemDefinition newItem = IGame.Instance.WeaponArmorManager.TryGetItemByName(item.name);

        if (newItem.Type == ItemType.Armor)
        {
            ((Armor)newItem).EquipIt();
        }
        if (newItem.Type == ItemType.Weapons)
        {
            IGame.Instance.playerController.GetFighter().EquipWeapon(IGame.Instance.WeaponArmorManager.TryGetWeaponByName(item.name));
        }

        IGame.Instance.playerController.modularCharacter.transform.localPosition = new Vector3(1000, 1000, 1000);
        angleTryOnEquip = 0;
        IGame.Instance.playerController.modularCharacter.transform.localRotation = Quaternion.Euler(0, 0, 0);

        _confirmPanel.SetActive(true);
    }

    private void OnClickDecline()
    {
        _decline?.Invoke();
        _confirmPanel.SetActive(false);

        oldArmorWhenTryOn.EquipIt();
        IGame.Instance.playerController.GetFighter().EquipWeapon(IGame.Instance.WeaponArmorManager.TryGetWeaponByName(oldWeaponWhenTryOn.name));

        IGame.Instance.playerController.modularCharacter.transform.localPosition = new Vector3(0, 0, 0);
        IGame.Instance.playerController.modularCharacter.transform.localRotation = Quaternion.Euler(0, 0, 0);
    }

    private void OnClickAccept()
    {

        _accept?.Invoke(_grid);
        _confirmPanel.SetActive(false);

        IGame.Instance.saveGame.Coins -= _item.price;

        oldArmorWhenTryOn.EquipIt();
        IGame.Instance.playerController.GetFighter().EquipWeapon(IGame.Instance.WeaponArmorManager.TryGetWeaponByName(oldWeaponWhenTryOn.name));

        IGame.Instance.playerController.modularCharacter.transform.localPosition = new Vector3(0, 0, 0);
        IGame.Instance.playerController.modularCharacter.transform.localRotation = Quaternion.Euler(0, 0, 0);
    }

    private void OnDestroy()
    {
        if (marketInventoryController != null)
        {
            //marketInventoryController.onItemHovered -= HandleItemPickedUp;
            InventoryAll.inventory.onItemAdded -= HandleItemAdded;
            InventoryAll.inventory.onItemRemoved -= HandleItemRemoved;
            InventoryBag.inventory.onItemAdded -= HandleItemBugAdded;
            InventoryBag.inventory.onItemRemoved -= HandleItemBugRemoved;
        }

        IGame.Instance.CoinManager.Coins.OnChangeCount -= OnChangeMoney;
        _btnClose.onClick.RemoveAllListeners();

        _buttonAccept.onClick.RemoveAllListeners();
        _buttonDecline.onClick.RemoveAllListeners();

        _btnAll.onClick.RemoveAllListeners();
        _btnWeapons.onClick.RemoveAllListeners();
        _btnArmors.onClick.RemoveAllListeners();
        _btnConsume.onClick.RemoveAllListeners();
    }

    private long oldTime;

    private void Update()
    {
        if (!_confirmPanel.gameObject.activeInHierarchy) return;

        if (oldTime == 0) oldTime = now_time();

        angleTryOnEquip += (now_time()-oldTime)*0.05f;
        oldTime = now_time();
        if (angleTryOnEquip > 360) angleTryOnEquip -= 360;
        IGame.Instance.playerController.modularCharacter.transform.localRotation = Quaternion.Euler(0, angleTryOnEquip, 0);
    }
    public long now_time()
    {
        return (long)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalMilliseconds;
    }
}
