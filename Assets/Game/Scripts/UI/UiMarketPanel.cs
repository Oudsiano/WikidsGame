using FarrokhGames.Inventory;
using FarrokhGames.Inventory.Examples;
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

    [SerializeField] public TMPro.TMP_Text coinCountText;

    private ItemType marketState;
    public List<ItemDefinition> marketItems;

    private Action<Vector2Int> _accept;
    private Action _decline;
    private Vector2Int _grid;
    private IInventoryItem _item;

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

        /*
        foreach (var item in IGame.Instance.WeaponArmorManager.allWeaponsInGame)
        {
            if (item.sprite != null)
                InventoryAll.inventory.TryAdd(item);
        }
        foreach (var item in IGame.Instance.WeaponArmorManager.allArmorsInGame)
        {
            if (item.sprite != null)
                InventoryAll.inventory.TryAdd(item);
        }*/
    }

    public void Regen()
    {
        notAvaliableEvents = true;
        InventoryBag.inventory.Clear();
        foreach (ItemDefinition item in IGame.Instance.saveGame.bugItems)
        {
            InventoryBag.inventory.TryAdd(item);
        }

        ShowMarketItems(ItemType.Any);

        notAvaliableEvents = false;
    }
    public void GenerateMarketItems()
    {
        marketItems = new List<ItemDefinition>();
        foreach (var item in IGame.Instance.WeaponArmorManager.allWeaponsInGame)
        {
            if (item.sprite != null)
                marketItems.Add(item);
        }
        foreach (var item in IGame.Instance.WeaponArmorManager.allArmorsInGame)
        {
            if (item.sprite != null)
                marketItems.Add(item);
        }

        //marketInventoryController.onItemPickedUp += HandleItemPickedUp;
        //marketInventoryController.onItemAdded += HandleItemAdded;
        InventoryAll.inventory.onItemAdded += HandleItemAdded;
        InventoryAll.inventory.onItemRemoved += HandleItemRemoved;
        InventoryBag.inventory.onItemAdded += HandleItemBugAdded;
        InventoryBag.inventory.onItemRemoved += HandleItemBugRemoved;
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
        foreach (ItemDefinition item in IGame.Instance.saveGame.bugItems)
        {
            if (item.sprite == obj.sprite)
            {
                IGame.Instance.saveGame.bugItems.Remove(item);
                return;
            }
        }
    }

    private void HandleItemBugAdded(IInventoryItem obj)
    {
        if (notAvaliableEvents) return;
        foreach (var item in IGame.Instance.WeaponArmorManager.allWeaponsInGame)
        {
            if (item.sprite == obj.sprite)
                IGame.Instance.saveGame.bugItems.Add(item);
        }
        foreach (var item in IGame.Instance.WeaponArmorManager.allArmorsInGame)
        {
            if (item.sprite == obj.sprite)
                IGame.Instance.saveGame.bugItems.Add(item);
        }
    }


    private void HandleItemAdded(IInventoryItem obj)
    {
        if (notAvaliableEvents) return;

        foreach (var item in IGame.Instance.WeaponArmorManager.allWeaponsInGame)
        {
            if (item.sprite == obj.sprite)
                marketItems.Add(item);
        }
        foreach (var item in IGame.Instance.WeaponArmorManager.allArmorsInGame)
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
                    InventoryAll.inventory.TryAdd(item);
                }
            }
        }

        notAvaliableEvents = false;

    }

    public void InitMarketUI(Action<Vector2Int> accept, Action decline, Vector2Int grid, IInventoryItem item)
    {
        _accept = accept;
        _decline = decline;
        _grid = grid;
        _item = item;

        if (IGame.Instance.dataPLayer.playerData.coins< _item.price)
        {
            _decline?.Invoke();
            return;
        }

        _confirmPanel.SetActive(true);
    }

    private void OnClickDecline()
    {
        _decline?.Invoke();
        _confirmPanel.SetActive(false);
    }

    private void OnClickAccept()
    {

        _accept?.Invoke(_grid);
        _confirmPanel.SetActive(false);

        IGame.Instance.dataPLayer.playerData.coins -= _item.price;
        coinCountText.text = IGame.Instance.dataPLayer.playerData.coins.ToString();
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
    }
}
