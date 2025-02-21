using System.Collections.Generic;
using FarrokhGames.Inventory;
using UI.Inventory.Data;
using UI.Inventory.Enums;

namespace UI.Inventory
{
    public class InventoryProvider : IInventoryProvider
    {
        private List<IInventoryItem> _items = new();
        private int _maximumAlowedItemCount;
        private readonly ItemType _allowedItem;

        public InventoryProvider(InventoryRenderMode renderMode, int maximumAlowedItemCount = -1,
            ItemType allowedItem = ItemType.Any)
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
                if (_maximumAlowedItemCount < 0) return false;
                return inventoryItemCount >= _maximumAlowedItemCount;
            }
        }

        public bool AddInventoryItem(IInventoryItem item)
        {
            if (_items.Contains(item) == false) // TODO TryAdd
            {
                _items.Add(item);

                return true;
            }

            return false;
        }

        public bool DropInventoryItem(IInventoryItem item)
        {
            return RemoveInventoryItem(item);
        }

        public IInventoryItem GetInventoryItem(int index)
        {
            return _items[index];
        }

        public bool CanAddInventoryItem(IInventoryItem item)
        {
            if (_allowedItem == ItemType.Any)
            {
                return true;
            }

            return (item as ItemDefinition).Type == _allowedItem;
        }

        public bool CanRemoveInventoryItem(IInventoryItem item)
        {
            return true;
        }

        public bool CanDropInventoryItem(IInventoryItem item)
        {
            return true;
        }

        public bool RemoveInventoryItem(IInventoryItem item)
        {
            return _items.Remove(item);
        }
    }
}