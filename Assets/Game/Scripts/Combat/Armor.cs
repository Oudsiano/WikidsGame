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

    //[SerializeField] private GameObject PlayerPosition;
    //[SerializeField] private GameObject ArmorPrefab;
    [Header("Stats")]
    [SerializeField] private float armor;

    public float ArmorValue { get => armor; set => armor = value; }
    public armorID ArmorName { get => armorName; set => armorName = value; }

    public void EquipIt()
    {
        //if (ArmorID == armorID.none)
        //    Debug.LogError("Not correct armor");
        /*if (armorType == armorType.none)
            Debug.LogError("Not correct armor");*/

            UnEquipOtherArmorFromPlayer();
        //IGame.Instance.dataPLayer.playerData.armorIdToload = (int)ArmorName;

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

        IGame.Instance.saveGame.MakeSave();
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
