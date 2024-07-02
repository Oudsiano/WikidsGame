using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Core;
using RPG.Movement;
using RPG.Combat;
using FarrokhGames.Inventory;

public class WeaponArmorManager : MonoBehaviour
{
    [Header("Weapon")]
    [SerializeField]
    private List<Weapon> allWeaponsInGame;

    [Header("Armors")]
    [SerializeField]
    private List<Armor> allArmorsInGame;

    public List<Weapon> AllWeaponsInGame { get => allWeaponsInGame; set => allWeaponsInGame = value; }
    public List<Armor> AllArmorsInGame { get => allArmorsInGame; set => allArmorsInGame = value; }

    public PickableEquip dafaultPrefab;

    public Armor GerArmorById(armorID armrId)
    {
        return allArmorsInGame.Find((item) => item.ArmorName == armrId);
    }

    public Weapon TryGetWeaponByName(string _name)
    {
        if (_name == "" || _name==null) _name = "Unarmed";
        foreach (var item in allWeaponsInGame)
        {
            if (item.name == _name)
                return item;
        }

        Debug.LogError("Can't return weapon. It can be mistake");
        return null;
    }
    public Armor TryGetArmorByName(string _name)
    {
        foreach (var item in allArmorsInGame)
        {
            if (item.name == _name)
                return item;
        }

        Debug.LogError("Can't return armor. It can be mistake");
        return null;
    }

    public ItemDefinition TryGetItemByName(string _name)
    {
        foreach (var item in allWeaponsInGame)
        {
            if (item.name == _name)
                return item;
        }
        foreach (var item in allArmorsInGame)
        {
            if (item.name == _name)
                return item;
        }
        foreach (var item in IGame.Instance.QuestManager.allQuestsItems)
        {
            if (item.name == _name)
                return item;
        }

        Debug.LogError("Can't return item. It can be mistake");
        return null;
    }
}
