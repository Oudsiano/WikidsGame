using DialogueEditor;
using FarrokhGames.Inventory.Examples;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GiveItem : MonoBehaviour
{
    [SerializeField] public ItemDefinition Item;

    public void TryAddItemItem()
    {
        if (Item!=null)
        IGame.Instance.UIManager.uIBug.TryAddEquipToBug(Instantiate(Item));
    }

}
