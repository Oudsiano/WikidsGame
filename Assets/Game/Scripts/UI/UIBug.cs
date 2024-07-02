using FarrokhGames.Inventory;
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
    [SerializeField] public SizeInventoryExample DropPlace;

    [SerializeField] public Button btnClose;

    private bool notAvaliableEvents = false;
    private bool inited = false;

    public void Init()
    {
        marketInventoryController = InventoryBag.GetComponent<InventoryController>();
        InventoryBag.Init();
        InventoryArmor.Init();
        InventoryWeapon.Init();
        DropPlace.Init();
        inited = true;
        btnClose.onClick.AddListener(onClickClose);
    }

    private void onClickClose()
    {
        gameObject.SetActive(false);
        IGame.Instance.SavePlayerPosLikeaPause(false);
        pauseClass.IsOpenUI = false;
    }

    public bool TryTakeQuestItem(string itemName)
    {

        foreach (var item in InventoryBag.inventory.allItems)
        {
            if (item.name == itemName)
            {
                Debug.Log("Предмет найдет. Возможно стоит еще прописать удаление");
                return true;
            }
        }

        return false;

    }

    public bool AddEquipInBugIfNotExist(ItemDefinition item, int count)
    {
        if (InventoryBag.inventory.Contains(item)) return false;

        if (InventoryBag.inventory.CanAdd(item))
        {
            if (InventoryBag.inventory.TryAdd(item.CreateInstance(),count))
            {
                IGame.Instance.saveGame.AddItemToBug(item.name, 1);
                return true;
            }
            return false;
        }
        return false;
    }

    public void TryAddEquipToBug(ItemDefinition item, int count)
    {
        notAvaliableEvents = true;

        if (item.onlyOne && InventoryBag.inventory.Contains(item)) return;

            if (InventoryBag.inventory.CanAdd(item))
        {
            InventoryBag.inventory.TryAdd(item.CreateInstance(), count);
            IGame.Instance.saveGame.AddItemToBug(item.name, 1);
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
        foreach (ItemDefinition item in IGame.Instance.saveGame.getBugList())
        {
            InventoryBag.inventory.TryAdd(item, item.CountItems);
        }

        InventoryWeapon.inventory.Clear();
        if (IGame.Instance.saveGame.EquipedWeapon.sprite != null)
            InventoryWeapon.inventory.TryAdd(IGame.Instance.saveGame.EquipedWeapon.CreateInstance(),1);
        InventoryArmor.inventory.Clear();
        if (IGame.Instance.saveGame.EquipedArmor.sprite != null)
            InventoryArmor.inventory.TryAdd(IGame.Instance.saveGame.EquipedArmor.CreateInstance(),1);

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

        DropPlace.inventory.onItemAdded += OnAddedDropPlace;

        regen();
    }

    private void OnAddedDropPlace(ItemDefinition obj, int count)
    {
        DropItemNearPlayer((ItemDefinition)obj);
    }

    private void OnDrop(ItemDefinition obj)
    {
        DropItemNearPlayer((ItemDefinition)obj);
    }

    private void OnRemovedArmor(ItemDefinition obj, int count)
    {
        if (notAvaliableEvents) return;
        IGame.Instance.WeaponArmorManager.TryGetArmorByName(obj.name).UnEquip();
        IGame.Instance.saveGame.MakeSave();
    }

    private void OnAddedArmor(ItemDefinition obj, int count)
    {
        if (notAvaliableEvents) return;
        IGame.Instance.WeaponArmorManager.TryGetArmorByName(obj.name).EquipIt();
        IGame.Instance.saveGame.MakeSave();
    }

    private void OnRemovedWeapon(ItemDefinition obj, int count)
    {
        if (notAvaliableEvents) return;
        IGame.Instance.playerController.GetFighter().UnequipWeapon();
        IGame.Instance.saveGame.MakeSave();
    }

    private void OnAddedWeapon(ItemDefinition obj, int count)
    {
        if (notAvaliableEvents) return;
        IGame.Instance.playerController.GetFighter().EquipWeapon(IGame.Instance.WeaponArmorManager.TryGetWeaponByName(obj.name));
        IGame.Instance.saveGame.MakeSave();
    }

    private void OnRemoved(ItemDefinition obj, int count)
    {
        if (notAvaliableEvents) return;
        IGame.Instance.saveGame.RemoveItemFromBug(obj.name, count);
        IGame.Instance.saveGame.MakeSave();
    }

    private void OnAdded(ItemDefinition obj, int count)
    {
        if (notAvaliableEvents) return;
        IGame.Instance.saveGame.AddItemToBug(obj.name, count);
        IGame.Instance.saveGame.MakeSave();
    }

    // Update is called once per frame
    void Update()
    {
        DropPlace.inventory.DropAll(); //Вынес сюда, чтобы сброс происходил в отдельном фрейме. Если сброс делать во время переноса предмета, то косяк
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
