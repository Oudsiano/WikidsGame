using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


[CreateAssetMenu(fileName = "Armor", menuName = "Armors", order = 0)]
internal class Armor : ScriptableObject
{

    [Header("Core")]
    [SerializeField] private GameObject PlayerPosition; 
    [SerializeField] private GameObject ArmorPrefab; 



}
