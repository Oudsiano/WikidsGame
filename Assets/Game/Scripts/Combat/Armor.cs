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

    [SerializeField] private GameObject PlayerPosition; 
    [SerializeField] private GameObject ArmorPrefab;
    [Header("Stats")]
    [SerializeField] private float armor;

    public void UseToPlayer()
    {
        if (armorName == armorID.none)
            Debug.LogError("Not correct armor");
        if (armorType == armorType.none)
            Debug.LogError("Not correct armor");

        foreach (var armor in IGame.Instance.playerController.playerArmorManager.AllChests)
        {
            if (armorType == armor.armorType)
            {
                if (armorName == armor.armorID)
                    armor.armorGO.SetActive(true);
                else
                    armor.armorGO.SetActive(false);
            }

        }
    }


}
