using FarrokhGames.Inventory.Examples;
using RPG.Combat;
using System;
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

    private string playerName;
    public event Action<string> OnChangePlayerName;
    public event Action OnLoadItems;

    public Weapon EquipedWeapon { get => equipedWeapon; set { equipedWeapon = value;  } }
    public Armor EquipedArmor { get => equipedArmor; set { equipedArmor = value; } }
    public double Coins { get => coins; set { coins = value; IGame.Instance.CoinManager.Coins.SetCount(value); } }

    public List<ItemDefinition> BugItems
    {
        get => bugItems;
        set => bugItems = value;
    }
    public string PlayerName { get => playerName; set { playerName = value; OnChangePlayerName?.Invoke(playerName); } }

    public SaveGame()
    {
        BugItems = new List<ItemDefinition>();


        EquipedArmor = new Armor();
        EquipedWeapon = new Weapon();
    }

    public void SaveItemToBug(ItemDefinition item)
    {
        BugItems.Add(item);
    }

    public void RemoveItemFromBug(ItemDefinition item)
    {
        if (BugItems.Contains(item))
        {
            BugItems.Remove(item);
        }
        else
        {
            Debug.LogError("Потенциально ошибка");
        }
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

        IGame.Instance.dataPLayer.playerData.playerName = PlayerName;

        IGame.Instance.dataPLayer.playerData.soundOn = AudioManager.instance.SoundON;
        IGame.Instance.dataPLayer.playerData.soundVol = AudioManager.instance.SoundVol;
        IGame.Instance.dataPLayer.playerData.musicOn = !SoundManager.GetMusicMuted();
        IGame.Instance.dataPLayer.playerData.musicVol = SoundManager.GetMusicVolume();

        IGame.Instance.gameAPI.SaveUpdater();
    }

    public void MakeLoad()
    {
        IGame.Instance.UIManager.UpdateParamsUI();


        BugItems.Clear();
        EquipedArmor = IGame.Instance.WeaponArmorManager.GerArmorById((armorID)IGame.Instance.dataPLayer.playerData.armorIdToload);
        EquipedWeapon = IGame.Instance.WeaponArmorManager.TryGetWeaponByName(IGame.Instance.dataPLayer.playerData.weaponToLoad);

        if (IGame.Instance.dataPLayer.playerData.playerName != "")
            PlayerName = IGame.Instance.dataPLayer.playerData.playerName;

        if (IGame.Instance.dataPLayer.playerData.containsBug==null)
            IGame.Instance.dataPLayer.playerData.containsBug = new string[0];

        if (IGame.Instance.dataPLayer.playerData.containsBug.Length > 99)
        {
            IGame.Instance.dataPLayer.playerData.containsBug = new string[0];
            MakeSave();
        }
        else
            foreach (var item in IGame.Instance.dataPLayer.playerData.containsBug)
            {
                BugItems.Add((ItemDefinition)IGame.Instance.WeaponArmorManager.TryGetItemByName(item)
                    .CreateInstance()
                    );
            }
        OnLoadItems?.Invoke();
        Coins = IGame.Instance.dataPLayer.playerData.coins;
    }
}
