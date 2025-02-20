using FarrokhGames.Inventory.Examples;
using UnityEngine;
using UnityEngine.Serialization;

namespace AINavigation
{
    public class GiveItem : MonoBehaviour // TODO rename
    {
        [FormerlySerializedAs("Item")] [SerializeField] private ItemDefinition _item; // TODO rename

        public void TryAddItemItem()
        {
            if (_item != null)
                IGame.Instance.UIManager.uIBug.TryAddEquipToBug(_item.CreateInstance());
        }
    }
}