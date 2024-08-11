using FarrokhGames.Inventory;
using FarrokhGames.Inventory.Examples;
using RPG.Combat;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
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

    public bool TryTakeQuestItem(string itemName, bool needDelete=false)
    {

        foreach (var item in InventoryBag.inventory.allItems)
        {
            if (item.name == itemName)
            {
                if (needDelete) OnRemoved(item);
                //Debug.Log("������� ������. �������� ����� ��� ��������� ��������");
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
        if (InventoryBag.inventory.Contains(item)) return false;

        if (InventoryBag.inventory.CanAdd(item))
        {
            if (InventoryBag.inventory.TryAdd(item.CreateInstance()))
            {
                IGame.Instance.saveGame.SaveItemToBug(item);
                return true;
            }
            return false;
        }
        return false;
    }


    public void TryAddEquipToBug(ItemDefinition item)
    {
        notAvaliableEvents = true;

        if (item.onlyOne && InventoryBag.inventory.Contains(item)) return;

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


        if (item.Type == ItemType.Weapons)
        {
            Weapon _tempW = IGame.Instance.WeaponArmorManager.TryGetWeaponByName(item.Name);
            GameObject newObj = Instantiate(_tempW.WeaponPrefab,
                new Vector3(posX, posY + 1, IGame.Instance.playerController.transform.localPosition.z),
                Quaternion.Euler(0, 0, 0));

            PickableEquip _tempPE = newObj.GetComponent<PickableEquip>();

            if (_tempPE == null)
                _tempPE = newObj.AddComponent<PickableEquip>();

            SphereCollider sphereCollider = newObj.AddComponent<SphereCollider>();
            sphereCollider.isTrigger = true;
            sphereCollider.center = new Vector3(0, 0, 0);
            sphereCollider.radius = 0.6684607f;

            RotationObjects rotationObjects = newObj.AddComponent<RotationObjects>();
            rotationObjects.rotationAxis = new Vector3(0, 1, 0);
            rotationObjects.rotationDuration = 5;
            rotationObjects.easeType = DG.Tweening.Ease.Linear;

            _tempPE.SetItem(item);
        }
        else
        {
            Instantiate(IGame.Instance.WeaponArmorManager.dafaultPrefab,
                new Vector3(posX, posY + 1, IGame.Instance.playerController.transform.localPosition.z),
                Quaternion.Euler(0, 0, 0))
                .GetComponent<PickableEquip>().SetItem(item);
        }
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

        DropPlace.inventory.onItemAdded += OnAddedDropPlace;

        regen();
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
        if (notAvaliableEvents) return;
        IGame.Instance.WeaponArmorManager.TryGetArmorByName(obj.name).UnEquip();
        IGame.Instance.saveGame.MakeSave();
    }

    private void OnAddedArmor(IInventoryItem obj)
    {
        if (notAvaliableEvents) return;
        IGame.Instance.WeaponArmorManager.TryGetArmorByName(obj.name).EquipIt();
        IGame.Instance.saveGame.MakeSave();
    }

    private void OnRemovedWeapon(IInventoryItem obj)
    {
        if (notAvaliableEvents) return;
        IGame.Instance.playerController.GetFighter().UnequipWeapon();
        IGame.Instance.saveGame.MakeSave();
    }

    private void OnAddedWeapon(IInventoryItem obj)
    {
        if (notAvaliableEvents) return;
        IGame.Instance.playerController.GetFighter().EquipWeapon(IGame.Instance.WeaponArmorManager.TryGetWeaponByName(obj.name));
        IGame.Instance.saveGame.MakeSave();
    }

    private void OnRemoved(IInventoryItem obj)
    {
        if (notAvaliableEvents) return;
        IGame.Instance.saveGame.BugItems.Remove((ItemDefinition)obj);
        IGame.Instance.saveGame.MakeSave();
    }

    private void OnAdded(IInventoryItem obj)
    {
        if (notAvaliableEvents) return;
        IGame.Instance.saveGame.BugItems.Add((ItemDefinition)obj);
        IGame.Instance.saveGame.MakeSave();
    }

    // Update is called once per frame
    void Update()
    {
        DropPlace.inventory.DropAll(); //����� ����, ����� ����� ���������� � ��������� ������. ���� ����� ������ �� ����� �������� ��������, �� �����
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
