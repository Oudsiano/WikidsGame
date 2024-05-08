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

    private ItemType marketState;
    public List<ItemDefinition> marketItems;

    private Action<Vector2Int> _accept;
    private Action _decline;
    private Vector2Int _grid;

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

        marketInventoryController.onItemPickedUp += HandleItemPickedUp;
        //marketInventoryController.onItemAdded += HandleItemAdded;
        InventoryAll.inventory.onItemAdded += HandleItemAdded;
    }

    private void HandleItemAdded(IInventoryItem obj)
    {
        Debug.Log("addEvent");

        foreach (var item in marketItems)
        {
            if (item.sprite == obj.sprite) return;
        }

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
        foreach (ItemDefinition item in InventoryAll.inventory.allItems)
        {
            if (obj.position == item.position)
            {
                marketItems.Remove(item);
            }
        }
        
    }

    public void ShowMarketItems(ItemType state)
    {
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


    }

    public void InitMarketUI(Action<Vector2Int> accept, Action decline, Vector2Int grid)
    {
        _accept = accept;
        _decline = decline;
        _grid = grid;
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
    }

    private void OnDestroy()
    {
        if (marketInventoryController != null)
        {
            marketInventoryController.onItemHovered -= HandleItemPickedUp;
            InventoryAll.inventory.onItemAdded -= HandleItemAdded;
        }
    }
}
