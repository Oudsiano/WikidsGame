using FarrokhGames.Inventory.Examples;
using UnityEngine;

namespace AINavigation
{
    public class GiveItem : MonoBehaviour
    {
        [SerializeField] public ItemDefinition Item;

        public void TryAddItemItem()
        {
            if (Item!=null)
                IGame.Instance.UIManager.uIBug.TryAddEquipToBug(Item.CreateInstance());
        }

    }
}
