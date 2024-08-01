using FarrokhGames.Inventory.Examples;
using RPG.Combat;
using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class OneItemForSave
{
    public int count;
    public string name;

    public OneItemForSave(int _c, string _n)
    {
        count = _c;
        name = _n;
    }
}

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

    public Weapon EquipedWeapon { get => equipedWeapon; set { equipedWeapon = value; } }
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
        IGame.Instance.dataPlayer.playerData.armorIdToload = (int)EquipedArmor.ArmorName;
        IGame.Instance.dataPlayer.playerData.weaponToLoad = EquipedWeapon.name;

        List<OneItemForSave> tempBug = new List<OneItemForSave>();
        /*for (int i = BugItems.Count-1; i >= 0; i--)
        {
            for (int i2  = i-1; i2 >= 0; i2--)
            {
                if (BugItems[i].name == BugItems[i2].name)
                {
                    BugItems[i].CountItems += BugItems[i2].CountItems;
                    BugItems[i2].CountItems = 0;
                }
            }
        }
        for (int i = BugItems.Count-1; i >= 0; i--)
            if (BugItems[i].CountItems == 0) bugItems.RemoveAt(i);*/

        foreach (var item in BugItems)
        {
            item.CountItems = 1;

            tempBug.Add(new OneItemForSave(item.CountItems, item.name));
        }
        //IGame.Instance.dataPLayer.playerData.containsBug = tempBug.ToArray();
        IGame.Instance.dataPlayer.playerData.containsBug = null;
        IGame.Instance.dataPlayer.playerData.containsBug2 = tempBug;
        IGame.Instance.dataPlayer.playerData.coins = Coins;

        IGame.Instance.dataPlayer.playerData.playerName = PlayerName;

        IGame.Instance.dataPlayer.playerData.soundOn = AudioManager.instance.SoundON;
        IGame.Instance.dataPlayer.playerData.soundVol = AudioManager.instance.SoundVol;
        IGame.Instance.dataPlayer.playerData.musicOn = !SoundManager.GetMusicMuted();
        IGame.Instance.dataPlayer.playerData.musicVol = SoundManager.GetMusicVolume();

        IGame.Instance.gameAPI.SaveUpdater();
    }

    public void MakeLoad()
    {
        IGame.Instance.UIManager.UpdateParamsUI();


        BugItems.Clear();
        EquipedArmor = IGame.Instance.WeaponArmorManager.GerArmorById((armorID)IGame.Instance.dataPlayer.playerData.armorIdToload);
        EquipedWeapon = IGame.Instance.WeaponArmorManager.TryGetWeaponByName(IGame.Instance.dataPlayer.playerData.weaponToLoad);

        if (IGame.Instance.dataPlayer.playerData.playerName != "")
            PlayerName = IGame.Instance.dataPlayer.playerData.playerName;

        if (IGame.Instance.dataPlayer.playerData.containsBug2 == null)
            IGame.Instance.dataPlayer.playerData.containsBug2 = new List<OneItemForSave>();

        if (IGame.Instance.dataPlayer.playerData.containsBug != null)
        {
            if (IGame.Instance.dataPlayer.playerData.containsBug.Length > 99)
            {
                IGame.Instance.dataPlayer.playerData.containsBug = new string[0];
                MakeSave();
            }
            else
            if (IGame.Instance.dataPlayer.playerData.containsBug != null)
                foreach (var item in IGame.Instance.dataPlayer.playerData.containsBug)
                {
                    BugItems.Add((ItemDefinition)IGame.Instance.WeaponArmorManager.TryGetItemByName(item)
                        .CreateInstance()
                        );
                }
        }


        foreach (var item in IGame.Instance.dataPlayer.playerData.containsBug2)
        {
            ItemDefinition newI = (ItemDefinition)IGame.Instance.WeaponArmorManager.TryGetItemByName(item.name.Replace("(Clone)", ""))
                .CreateInstance();
            newI.CountItems = item.count;

            BugItems.Add(newI);
        }

        OnLoadItems?.Invoke();
        Coins = IGame.Instance.dataPlayer.playerData.coins;
    }
}
