using FarrokhGames.Inventory.Examples;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "Armor", menuName = "Armors", order = 0)]
public class Armor : ItemDefinition
{
    [Header("Core")]
    [SerializeField] private armorType armorType;
    [SerializeField] private armorID armorName;

    [Header("Stats")]
    [SerializeField] private float armor;

    [Header("Description")]
    [SerializeField] [TextArea] private string description; // Описание брони

    public float ArmorValue { get => armor; set => armor = value; }
    public armorID ArmorName { get => armorName; set => armorName = value; }

    //public string Description { get => description; set => description = value; } // Свойство для описания

    public void EquipIt()
    {
        UnEquipOtherArmorFromPlayer();

        IGame.Instance.saveGame.EquipedArmor = this;
        foreach (var armor in IGame.Instance.playerController.playerArmorManager.AllArmors)
        {
            if (ArmorName == armor.armorID)
            {
                foreach (var item in armor.armorGO)
                {
                    item.SetActive(true);
                }
            }
        }
    }

    public void UnEquip()
    {
        foreach (var armor in IGame.Instance.playerController.playerArmorManager.AllArmors)
        {
            if (ArmorName == armor.armorID)
            {
                foreach (var item in armor.armorGO)
                {
                    item.SetActive(false);
                }
            }
        }

        IGame.Instance.WeaponArmorManager.GerArmorById(armorID.none).EquipIt();
    }

    public void UnEquipOtherArmorFromPlayer()
    {
        foreach (var armor in IGame.Instance.playerController.playerArmorManager.AllArmors)
        {
            foreach (var item in armor.armorGO)
            {
                item.SetActive(false);
            }
        }
    }
}
