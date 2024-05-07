using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Core;
using RPG.Movement;
using RPG.Combat;

public class WeaponArmorManager : MonoBehaviour
{
    [Header("Weapon")]
    [SerializeField]
    public List<Weapon> allWeaponsInGame;

    [Header("Armors")]
    [SerializeField]
    public List<Armor> allArmorsInGame;

}
