using FarrokhGames.Inventory.Examples;
using System.Collections.Generic;
using UnityEngine;

public class SaveGame
{
    //Заготовка для сейв гейма

    //Сумка
    public List<ItemDefinition> bugItems;

    public SaveGame()
    {
        bugItems = new List<ItemDefinition>();
    }

    public void AddItemToBug(ItemDefinition item)
    {
        bugItems.Add(item);
    }

    public void RemoveItemFromBug(ItemDefinition item)
    {
        bugItems.Remove(item);
    }



    public void MakeSave()
    {

    }
}
