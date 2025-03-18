using FarrokhGames.Inventory;
using UI.Inventory.Data;
using UI.Inventory.Enums;
using UnityEngine;

namespace UI.Inventory
{
    [RequireComponent(typeof(InventoryController), typeof(InventoryRenderer))]
    public class SizeInventoryExample : MonoBehaviour
    {
        [SerializeField] private InventoryRenderMode _renderMode = InventoryRenderMode.Grid;
        [SerializeField] private int _maximumAlowedItemCount = -1;
        [SerializeField] private ItemType _allowedItem = ItemType.Any;
        [SerializeField] private int _width = 8;
        [SerializeField] private int _height = 4;
        [SerializeField] private ItemDefinition[] _definitions = null;
        [SerializeField] private bool _fillRandomly = true; // Should the inventory get filled with random items?
        [SerializeField] private bool _fillEmpty = false; // Should the inventory get completely filled?
        [SerializeField] private bool _isMarket = false;
        [SerializeField] private bool _dropedFromThere = false;
        [SerializeField] private float _priceMultiple = 1;
        [SerializeField] private bool _showPricesThere;

        public InventoryManager inventory;

        public void Init() // TODO construct
        {
            GetComponent<InventoryRenderer>().Init(); 
            var provider = new InventoryProvider(_renderMode, _maximumAlowedItemCount, _allowedItem);

            // Create inventory
            inventory = new InventoryManager(provider, _width, _height);
            inventory.isMarket = _isMarket;
            inventory.DropedFromThere = _dropedFromThere;
            inventory.PriceMultiple = _priceMultiple;
            inventory.ShowPricesThere = _showPricesThere;
            
            // Fill inventory with random items
            if (_fillRandomly)
            {
                var tries = (_width * _height) / 3; // TODO magic numbers
                
                for (var i = 0; i < tries; i++)
                {
                    inventory.TryAdd(_definitions[Random.Range(0, _definitions.Length)].CreateInstance());
                }
            }

            // Fill empty slots with first (1x1) item
            if (_fillEmpty)
            {
                for (var i = 0; i < _width * _height; i++)
                {
                    inventory.TryAdd(_definitions[0].CreateInstance());
                }
            }
            
            GetComponent<InventoryRenderer>().SetInventory(inventory, provider.inventoryRenderMode);
            
            inventory.onItemDropped += (item) =>
            {
                Debug.Log((item as ItemDefinition).Name + " was dropped on the ground");
            };
            
            inventory.onItemDroppedFailed += (item) =>
            {
                Debug.Log($"You're not allowed to drop {(item as ItemDefinition).Name} on the ground");
            };
            
            inventory.onItemAddedFailed += (item) =>
            {
                Debug.Log($"You can't put {(item as ItemDefinition).Name} there!");
            };
        }
    }
}