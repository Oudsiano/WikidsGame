using FarrokhGames.Inventory;
using UI.Inventory.Enums;
using UnityEngine;

namespace UI.Inventory.Data
{
    [CreateAssetMenu(fileName = "Item", menuName = "Inventory/Item", order = 1)]
    public class ItemDefinition : ScriptableObject, IInventoryItem // SO its DATA
    {
        [SerializeField] public int PriceCoins; // TODO rename
        
        [SerializeField] private Sprite _sprite = null;
        [SerializeField] private InventoryShape _shape = null;
        [SerializeField] private ItemType _type = ItemType.Consume;
        [SerializeField] private bool _canDrop = true;
        [SerializeField, HideInInspector] private Vector2Int _position = Vector2Int.zero;
        [SerializeField] private bool _onlyOne = false;
        [SerializeField, HideInInspector] private int _countItems = 1;

        [Header("Item Description")] [SerializeField] [TextArea]
        private string itemDescription; // Переименовано для уникальности

        public int CountItems
        {
            get => _countItems;
            set { _countItems = value; }
        }

        public bool onlyOne => _onlyOne;

        public int price => PriceCoins;

        public string Name => name;

        public ItemType Type => _type;

        public Sprite sprite => _sprite;

        public int width => _shape.width;

        public int height => _shape.height;

        public Vector2Int position
        {
            get => _position;
            set => _position = value;
        }

        public bool IsPartOfShape(Vector2Int localPosition)
        {
            return _shape.IsPartOfShape(localPosition);
        }

        public bool canDrop => _canDrop;

        public string Description => itemDescription;

        public ItemDefinition CreateInstance() // ITS DATA
        {
            var clone = ScriptableObject.Instantiate(this);
            clone.name =
                clone.name.Substring(0, clone.name.Length - 7); // Remove (Clone) from name // TODO magic numbers

            return clone;
        }
    }
}