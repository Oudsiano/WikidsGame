using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum armorType
{
    none,
    chest
}

[Serializable]
public enum armorID
{
    none,
    armorM28,
    armorM2,
    leatherGreenArmor,
    armorBlueWolf,
    armorDragon
    
};

[Serializable]
public class ArmorOne
{
    public armorType armorType;
    public armorID armorID;
    public GameObject[] armorGO;

}


public class PlayerArmorManager : MonoBehaviour
{

    [Header("Chests")]
    [SerializeField] public ArmorOne[] AllArmors;


}
