using FarrokhGames.Inventory.Examples;
using RPG.Combat;
using System.Collections.Generic;
using UnityEngine;

public class SaveGame
{
    //Заготовка для сейв гейма

    //Сумка
    private List<ItemDefinition> bugItems;
    private Armor equipedArmor;
    private Weapon equipedWeapon;

    private double coins;


    public Weapon EquipedWeapon { get => equipedWeapon; set => equipedWeapon = value; }
    public Armor EquipedArmor { get => equipedArmor; set => equipedArmor = value; }
    public double Coins { get => coins; set { coins = value; IGame.Instance.CoinManager.Coins.SetCount(value); }}

    public List<ItemDefinition> BugItems { get => bugItems; set => bugItems = value; }

    public SaveGame()
    {
        BugItems = new List<ItemDefinition>();


        EquipedArmor = new Armor();
        EquipedWeapon = new Weapon();
    }

    public void AddItemToBug(ItemDefinition item)
    {
        BugItems.Add(item);
    }

    public void RemoveItemFromBug(ItemDefinition item)
    {
        BugItems.Remove(item);
    }



    public void MakeSave()
    {
        IGame.Instance.dataPLayer.playerData.armorIdToload = (int)EquipedArmor.ArmorName;
        IGame.Instance.dataPLayer.playerData.weaponToLoad = EquipedWeapon.name;

        List<string> tempBug = new List<string>();
        foreach (var item in BugItems)
        {
            tempBug.Add(item.name);
        }
        IGame.Instance.dataPLayer.playerData.containsBug = tempBug.ToArray();
        IGame.Instance.dataPLayer.playerData.coins = Coins;

        IGame.Instance.gameAPI.SaveUpdater();
    }

    public void MakeLoad()
    {
        EquipedArmor = IGame.Instance.WeaponArmorManager.GerArmorById((armorID)IGame.Instance.dataPLayer.playerData.armorIdToload);
        EquipedWeapon = IGame.Instance.WeaponArmorManager.TryGetWeaponByName(IGame.Instance.dataPLayer.playerData.weaponToLoad);

        foreach (var item in IGame.Instance.dataPLayer.playerData.containsBug)
        {
            BugItems.Add((ItemDefinition)IGame.Instance.WeaponArmorManager.TryGetItemByName(item).CreateInstance());
        }

        Coins = IGame.Instance.dataPLayer.playerData.coins;
    }
}
