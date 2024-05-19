using FarrokhGames.Inventory;
using FarrokhGames.Inventory.Examples;
using RPG.Combat;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIBug : MonoBehaviour
{

    [SerializeField] public SizeInventoryExample InventoryBag;
    private InventoryController marketInventoryController;

    [SerializeField] public SizeInventoryExample InventoryWeapon;
    [SerializeField] public SizeInventoryExample InventoryArmor;

    [SerializeField] public Button btnClose;

    private bool notAvaliableEvents = false;
    private bool inited = false;

    public void Init()
    {
        marketInventoryController = InventoryBag.GetComponent<InventoryController>();
        InventoryBag.Init();
        InventoryArmor.Init();
        InventoryWeapon.Init();
        inited = true;

        btnClose.onClick.AddListener(onClickClose);

        
    }

    private void onClickClose()
    {
        gameObject.SetActive(false);
        IGame.Instance.SetPause(false);
    }



    public void TryAddEquipToBug(ItemDefinition item)
    {
        notAvaliableEvents = true;
        if (InventoryBag.inventory.CanAdd(item))
        {
            InventoryBag.inventory.TryAdd(item.CreateInstance());
            IGame.Instance.saveGame.SaveItemToBug(item);
        }
        else
        {
            DropItemNearPlayer(item);
        }
        notAvaliableEvents = false;
    }

    public void DropItemNearPlayer(ItemDefinition item)
    {
        notAvaliableEvents = true;

        float posX = IGame.Instance.playerController.transform.localPosition.x + UnityEngine.Random.Range(-10, 10) * 0.1f;
        float posY = IGame.Instance.playerController.transform.localPosition.y + UnityEngine.Random.Range(-10, 10) * 0.1f;

        Instantiate(IGame.Instance.WeaponArmorManager.dafaultPrefab,
            new Vector3(posX, posY+1, IGame.Instance.playerController.transform.localPosition.z ),
            Quaternion.Euler(0, 0, 0))
            .GetComponent<PickableEquip>().SetItem(item);

        notAvaliableEvents = false;
    }

    public void regen()
    {
        notAvaliableEvents = true;
        InventoryBag.inventory.Clear();
        foreach (ItemDefinition item in IGame.Instance.saveGame.BugItems)
        {
            InventoryBag.inventory.TryAdd(item);
        }

        InventoryWeapon.inventory.Clear();
        if (IGame.Instance.saveGame.EquipedWeapon.sprite != null)
            InventoryWeapon.inventory.TryAdd(IGame.Instance.saveGame.EquipedWeapon.CreateInstance());
        InventoryArmor.inventory.Clear();
        if (IGame.Instance.saveGame.EquipedArmor.sprite != null)
            InventoryArmor.inventory.TryAdd(IGame.Instance.saveGame.EquipedArmor.CreateInstance());

        notAvaliableEvents = false;
    }

    void Start()
    {
        InventoryBag.inventory.onItemAdded += OnAdded;
        InventoryBag.inventory.onItemRemoved += OnRemoved;

        InventoryBag.inventory.onItemDropped += OnDrop;


        InventoryWeapon.inventory.onItemAdded += OnAddedWeapon;
        InventoryWeapon.inventory.onItemRemoved += OnRemovedWeapon;

        InventoryArmor.inventory.onItemAdded += OnAddedArmor;
        InventoryArmor.inventory.onItemRemoved += OnRemovedArmor;

        regen();
    }

    private void OnDrop(IInventoryItem obj)
    {
        DropItemNearPlayer((ItemDefinition)obj);
    }

    private void OnRemovedArmor(IInventoryItem obj)
    {
        if (notAvaliableEvents) return;
        IGame.Instance.WeaponArmorManager.TryGetArmorByName(obj.name).UnEquip();
    }

    private void OnAddedArmor(IInventoryItem obj)
    {
        if (notAvaliableEvents) return;
        IGame.Instance.WeaponArmorManager.TryGetArmorByName(obj.name).EquipIt();
    }

    private void OnRemovedWeapon(IInventoryItem obj)
    {
        if (notAvaliableEvents) return;
        IGame.Instance.playerController.GetFighter().UnequipWeapon();
    }

    private void OnAddedWeapon(IInventoryItem obj)
    {
        if (notAvaliableEvents) return;
        IGame.Instance.playerController.GetFighter().EquipWeapon(IGame.Instance.WeaponArmorManager.TryGetWeaponByName(obj.name));
    }

    private void OnRemoved(IInventoryItem obj)
    {
        if (notAvaliableEvents) return;
        IGame.Instance.saveGame.BugItems.Remove((ItemDefinition)obj);
    }

    private void OnAdded(IInventoryItem obj)
    {
        if (notAvaliableEvents) return;
        IGame.Instance.saveGame.BugItems.Add((ItemDefinition)obj);
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

        btnClose.onClick.RemoveAllListeners();
    }
}
