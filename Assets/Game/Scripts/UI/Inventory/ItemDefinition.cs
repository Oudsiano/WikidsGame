using UnityEngine;

namespace FarrokhGames.Inventory.Examples
{
    /// <summary>
    /// Scriptable Object representing an Inventory Item
    /// </summary>
    [CreateAssetMenu(fileName = "Item", menuName = "Inventory/Item", order = 1)]
    public class ItemDefinition : ScriptableObject, IInventoryItem
    {
        [SerializeField] public int PriceCoins;
        [SerializeField] private Sprite _sprite = null;
        [SerializeField] private InventoryShape _shape = null;
        [SerializeField] private ItemType _type = ItemType.Consume;
        [SerializeField] private bool _canDrop = true;
        [SerializeField, HideInInspector] private Vector2Int _position = Vector2Int.zero;
        [SerializeField] private bool _onlyOne = false;
        [SerializeField, HideInInspector] private int _countItems = 1;
        [Header("Item Description")]
        [SerializeField] [TextArea] private string itemDescription; // Переименовано для уникальности

        public int CountItems
        {
            get => _countItems;
            set
            {
                _countItems = value;
            }
        }

        public bool onlyOne => _onlyOne;

        public int price => PriceCoins;

        /// <summary>
        /// The name of the item
        /// </summary>
        public string Name => name;

        /// <summary>
        /// The type of the item
        /// </summary>
        public ItemType Type => _type;

        /// <inheritdoc />
        public Sprite sprite => _sprite;

        /// <inheritdoc />
        public int width => _shape.width;

        /// <inheritdoc />
        public int height => _shape.height;

        /// <inheritdoc />
        public Vector2Int position
        {
            get => _position;
            set => _position = value;
        }

        /// <inheritdoc />
        public bool IsPartOfShape(Vector2Int localPosition)
        {
            return _shape.IsPartOfShape(localPosition);
        }

        /// <inheritdoc />
        public bool canDrop => _canDrop;

        /// <summary>
        /// Public property to access the description
        /// </summary>
        public string Description => itemDescription;

        /// <summary>
        /// Creates a copy if this scriptable object
        /// </summary>
        public ItemDefinition CreateInstance()
        {
            var clone = ScriptableObject.Instantiate(this);
            clone.name = clone.name.Substring(0, clone.name.Length - 7); // Remove (Clone) from name
            return clone;
        }
    }
}
