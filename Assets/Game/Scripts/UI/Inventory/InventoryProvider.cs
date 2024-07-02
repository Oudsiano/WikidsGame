using System;
using System.Collections.Generic;

namespace FarrokhGames.Inventory
{
    public class InventoryProvider : IInventoryProvider
    {
        private List<ItemDefinition> _items = new List<ItemDefinition>();
        private int _maximumAlowedItemCount;
        ItemType _allowedItem;

        /// <summary>
        /// CTOR
        /// </summary>
        public InventoryProvider(InventoryRenderMode renderMode, int maximumAlowedItemCount = -1, ItemType allowedItem = ItemType.Any)
        {
            inventoryRenderMode = renderMode;
            _maximumAlowedItemCount = maximumAlowedItemCount;
            _allowedItem = allowedItem;
        }

        public int inventoryItemCount => _items.Count;

        public InventoryRenderMode inventoryRenderMode { get; private set; }

        public bool isInventoryFull
        {
            get
            {
                if (_maximumAlowedItemCount < 0)return false;
                return inventoryItemCount >= _maximumAlowedItemCount;
            }
        }

        public bool AddInventoryItem(ItemDefinition item)
        {
            if (!_items.Contains(item))
            {
                _items.Add(item);
                return true;
            }
            return false;
        }

        public bool DropInventoryItem(ItemDefinition item)
        {
            return RemoveInventoryItem(item);
        }

        public ItemDefinition GetInventoryItem(int index)
        {
            return _items[index];
        }

        public bool CanAddInventoryItem(ItemDefinition item)
        {
            if (_allowedItem == ItemType.Any)return true;
            return (item as ItemDefinition).Type == _allowedItem;
        }

        public bool CanRemoveInventoryItem(ItemDefinition item)
        {
            return true;
        }

        public bool CanDropInventoryItem(ItemDefinition item)
        {
            return true;
        }

        public bool RemoveInventoryItem(ItemDefinition item)
        {
            return _items.Remove(item);
        }
    }
}