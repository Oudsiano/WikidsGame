using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PickableArmor;

public enum armorType
{
    chest
}

[Serializable]
public enum armorID
{
    none,
    armorM28,
    armorM2
};

[Serializable]
public class ArmorOne
{
    public armorType armorType;
    public armorID armorID;
    public GameObject armorGO;

}


public class PlayerArmorManager : MonoBehaviour
{

    [Header("Chests")]
    [SerializeField] public ArmorOne[] AllChests;


    private void Awake()
    {

    }

    public void EquipNewArmor(armorType type, armorID id)
    {
        foreach (var armor in AllChests)
        {
            if (type == armor.armorType)
            {
                if (id == armor.armorID)
                    armor.armorGO.SetActive(true);
                else
                    armor.armorGO.SetActive(false);
            }

        }

    }
}
