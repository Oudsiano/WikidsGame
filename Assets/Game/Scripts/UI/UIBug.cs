using FarrokhGames.Inventory;
using FarrokhGames.Inventory.Examples;
using RPG.Combat;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIBug : MonoBehaviour
{

    [SerializeField] public SizeInventoryExample InventoryBag;
    private InventoryController marketInventoryController;

    [SerializeField] public SizeInventoryExample InventoryWeapon;
    [SerializeField] public SizeInventoryExample InventoryArmor;

    private bool notAvaliableEvents = false;
    private bool inited = false;

    public void Init()
    {
        marketInventoryController = InventoryBag.GetComponent<InventoryController>();
        InventoryBag.Init();
        InventoryArmor.Init();
        InventoryWeapon.Init();
        inited = true;
    }

    public void regen()
    {
        notAvaliableEvents = true;
        InventoryBag.inventory.Clear();
        foreach (ItemDefinition item in IGame.Instance.saveGame.bugItems)
        {
            InventoryBag.inventory.TryAdd(item);
        }

        InventoryWeapon.inventory.Clear();
        foreach (Weapon item in IGame.Instance.WeaponArmorManager.allWeaponsInGame)
        {
            if (IGame.Instance.dataPLayer.playerData.weaponToLoad == item.name)
            {
                if (item.sprite!=null)
                InventoryWeapon.inventory.TryAdd(item);
            }
        }
        notAvaliableEvents = false;


    }
    void Start()
    {
        InventoryBag.inventory.onItemAdded += OnAdded;
        InventoryBag.inventory.onItemRemoved += OnRemoved;

        InventoryWeapon.inventory.onItemAdded += OnAddedWeapon;
        InventoryWeapon.inventory.onItemRemoved += OnRemovedWeapon;

        InventoryArmor.inventory.onItemAdded += OnAddedArmor;
        InventoryArmor.inventory.onItemRemoved += OnRemovedArmor;
    }

    private void OnRemovedArmor(IInventoryItem obj)
    {
        if (notAvaliableEvents) return;
        foreach (Armor item in IGame.Instance.WeaponArmorManager.allArmorsInGame)
        {
            item.UnEquip();
        }
    }

    private void OnAddedArmor(IInventoryItem obj)
    {
        if (notAvaliableEvents) return;
        foreach (Armor item in IGame.Instance.WeaponArmorManager.allArmorsInGame)
        {
            if (item == obj)
                item.EquipIt();
        }
    }

    private void OnRemovedWeapon(IInventoryItem obj)
    {
        if (notAvaliableEvents) return;
        IGame.Instance.playerController.GetFighter().UnequipWeapon();
    }

    private void OnAddedWeapon(IInventoryItem obj)
    {
        if (notAvaliableEvents) return;
        foreach (Weapon item in IGame.Instance.WeaponArmorManager.allWeaponsInGame)
        {
            if (item == obj)
                IGame.Instance.playerController.GetFighter().EquipWeapon(item);
        }
    }

    private void OnRemoved(IInventoryItem obj)
    {
        if (notAvaliableEvents) return;
        IGame.Instance.saveGame.bugItems.Remove((ItemDefinition)obj);
    }

    private void OnAdded(IInventoryItem obj)
    {
        if (notAvaliableEvents) return;
        IGame.Instance.saveGame.bugItems.Add((ItemDefinition)obj);
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnDestroy()
    {
        if (!inited) return;
        InventoryBag.inventory.onItemAdded -= OnAdded;
        InventoryBag.inventory.onItemRemoved -= OnRemoved;
        InventoryWeapon.inventory.onItemAdded -= OnAddedWeapon;
        InventoryWeapon.inventory.onItemRemoved -= OnRemovedWeapon;
        InventoryArmor.inventory.onItemAdded -= OnAddedArmor;
        InventoryArmor.inventory.onItemRemoved -= OnRemovedArmor;
    }
}
