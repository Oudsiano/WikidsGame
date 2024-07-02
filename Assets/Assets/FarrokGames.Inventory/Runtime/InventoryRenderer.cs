using FarrokhGames.Shared;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FarrokhGames.Inventory
{
    public class ItemElementInGrid
    {
        public GameObject itemGO;

        public Image img;

        public GameObject PricetextGo;
        private TMPro.TextMeshProUGUI PricetextUI;


        public Image CountimgBg;
        public GameObject CounttextGo;
        private TMPro.TextMeshProUGUI CounttextUI;



        private Image CoinImg;

        private int price = 0;
        private float multy = 1;

        private int count = 1;

        public ItemElementInGrid()
        {
            itemGO = new GameObject("item");
            img = itemGO.AddComponent<Image>();
            img.transform.localScale = Vector3.one;

            PricetextGo = new GameObject("PricetextItem");
            PricetextGo.transform.SetParent(itemGO.transform, false);
            PriceTextUI = PricetextGo.AddComponent<TMPro.TextMeshProUGUI>();
            var PricetextRt = PricetextGo.GetComponent<RectTransform>();
            PricetextRt.anchorMin = new Vector2(0.5f, 0);
            PricetextRt.anchorMax = new Vector2(0.5f, 0);
            PricetextRt.pivot = new Vector2(0.5f, 0.5f);
            //PricetextRt.localPosition = new Vector2(0, 25);
            PriceTextUI.text = "";
            PriceTextUI.alignment = TMPro.TextAlignmentOptions.Center;
            PricetextRt.anchoredPosition = new Vector2(-25, 25);


            CountimgBg = new GameObject("CountimgBg").AddComponent<Image>();
            CountimgBg.gameObject.transform.SetParent(itemGO.transform, false);
            CounttextGo = new GameObject("CounttextItem");
            CounttextGo.transform.SetParent(CountimgBg.transform, false);
            CounttextUI = CounttextGo.AddComponent<TMPro.TextMeshProUGUI>();
            var _texture2 = Resources.Load("BGForCountItems", typeof(Texture2D)) as Texture2D;
            var _sprite2 = Sprite.Create(_texture2, new Rect(0, 0, _texture2.width, _texture2.height), new Vector2(0f, 0f), 1f);
            CountimgBg.sprite = _sprite2;
            var CounttextRt = CountimgBg.GetComponent<RectTransform>();
            CounttextRt.anchorMin = new Vector2(0, 1);
            CounttextRt.anchorMax = new Vector2(0, 1);
            CounttextRt.pivot = new Vector2(0, 0.5f);
            //CounttextRt.localPosition = new Vector2(20, -50);
            SetCount(1);
            CounttextUI.alignment = TMPro.TextAlignmentOptions.Center;
            CounttextRt.anchoredPosition = new Vector2(20, -50);
            CounttextRt.sizeDelta = new Vector2(100, 50);

            CoinImg = new GameObject("coin").AddComponent<Image>();
            CoinImg.gameObject.transform.SetParent(PricetextGo.transform, false);


            var _texture = Resources.Load("CoinForPrice", typeof(Texture2D)) as Texture2D;
            var _sprite = Sprite.Create(_texture, new Rect(0, 0, _texture.width, _texture.height), new Vector2(0f, 0f), 1f);


            CoinImg.sprite = _sprite;
            CoinImg.transform.localPosition = new Vector2(50, 0);
            CoinImg.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(50, 50);
                        
        }

        public void SetCount(int _count)
        {
            count = _count;
            if (_count > 1)
            {
                CounttextUI.text = count.ToString();
                CounttextUI.gameObject.SetActive(true);
                CountimgBg.gameObject.SetActive(true);
            }
            else
            {
                CounttextUI.gameObject.SetActive(false);
                CountimgBg.gameObject.SetActive(false);
            }
        }

        public TextMeshProUGUI PriceTextUI { get => PricetextUI; set => PricetextUI = value; }

        public void settextFromPrice(int _price)
        {
            price = _price;
            PricetextUI.text = ((int)(price * multy)).ToString();
        }

        public void setVisPriceSector(float multiple)
        {
            multy = multiple;
            if (multiple > 0)
            {

                PricetextUI.text = ((int)(price * multy)).ToString();
                PriceTextUI.gameObject.SetActive(true);
                CoinImg.gameObject.SetActive(true);
            }
            else
            {
                PriceTextUI.gameObject.SetActive(false);
                CoinImg.gameObject.SetActive(false);
            }
        }

    }



    /// <summary>
    /// Renders a given inventory
    /// /// </summary>
    [RequireComponent(typeof(RectTransform))]
    public class InventoryRenderer : MonoBehaviour
    {
        [SerializeField, Tooltip("The size of the cells building up the inventory")]
        private Vector2Int _cellSize = new Vector2Int(32, 32);

        [SerializeField, Tooltip("The sprite to use for empty cells")]
        private Sprite _cellSpriteEmpty = null;

        [SerializeField, Tooltip("The sprite to use for selected cells")]
        private Sprite _cellSpriteSelected = null;

        [SerializeField, Tooltip("The sprite to use for blocked cells")]
        private Sprite _cellSpriteBlocked = null;

        internal IInventoryManager inventory;
        InventoryRenderMode _renderMode;
        private bool _haveListeners;
        private Pool<Image> _imagePool2;
        private Pool<ItemElementInGrid> _itemPool;
        private Image[] _grids;
        private Dictionary<ItemDefinition, ItemElementInGrid> _items = new Dictionary<ItemDefinition, ItemElementInGrid>();

        /*
         * Setup
         */
        public void Init()
        {
            rectTransform = GetComponent<RectTransform>();

            // Create the image container
            var imageContainer = new GameObject("Image Pool").AddComponent<RectTransform>();
            imageContainer.transform.SetParent(transform);
            imageContainer.transform.localPosition = Vector3.zero;
            imageContainer.transform.localScale = Vector3.one;

            // Create pool of items
            _itemPool = new Pool<ItemElementInGrid>(
                delegate
                {
                    var item = new ItemElementInGrid();

                    if (((InventoryManager)inventory).ShowPricesThere)
                        item.setVisPriceSector(((InventoryManager)inventory).PriceMultiple);
                    else
                        item.setVisPriceSector(0);

                    item.itemGO.transform.SetParent(imageContainer);
                    item.itemGO.transform.localScale = Vector3.one;

                    return item;
                });
            // Create pool of images
            _imagePool2 = new Pool<Image>(
                delegate
                {
                    var image = new GameObject("Image").AddComponent<Image>();
                    image.transform.SetParent(imageContainer);
                    image.transform.localScale = Vector3.one;
                    return image;
                });
        }

        /// <summary>
        /// Set what inventory to use when rendering
        /// </summary>
        public void SetInventory(IInventoryManager inventoryManager, InventoryRenderMode renderMode)
        {
            OnDisable();
            inventory = inventoryManager ?? throw new ArgumentNullException(nameof(inventoryManager));
            _renderMode = renderMode;
            OnEnable();
        }

        /// <summary>
        /// Returns the RectTransform for this renderer
        /// </summary>
        public RectTransform rectTransform { get; private set; }

        /// <summary>
        /// Returns the size of this inventory's cells
        /// </summary>
        public Vector2 cellSize => _cellSize;

        /* 
        Invoked when the inventory inventoryRenderer is enabled
        */
        void OnEnable()
        {
            if (inventory != null && !_haveListeners)
            {
                if (_cellSpriteEmpty == null) { throw new NullReferenceException("Sprite for empty cell is null"); }
                if (_cellSpriteSelected == null) { throw new NullReferenceException("Sprite for selected cells is null."); }
                if (_cellSpriteBlocked == null) { throw new NullReferenceException("Sprite for blocked cells is null."); }

                inventory.onRebuilt += ReRenderAllItems;
                inventory.onItemAdded += HandleItemAdded;
                inventory.onItemRemoved += HandleItemRemoved;
                inventory.onItemDropped += HandleItemRemoved;
                inventory.onResized += HandleResized;
                _haveListeners = true;

                // Render inventory
                ReRenderGrid();
                ReRenderAllItems();
            }
        }

        /* 
        Invoked when the inventory inventoryRenderer is disabled
        */
        void OnDisable()
        {
            if (inventory != null && _haveListeners)
            {
                inventory.onRebuilt -= ReRenderAllItems;
                inventory.onItemAdded -= HandleItemAdded;
                inventory.onItemRemoved -= HandleItemRemoved;
                inventory.onItemDropped -= HandleItemRemoved;
                inventory.onResized -= HandleResized;
                _haveListeners = false;
            }
        }

        /*
        Clears and renders the grid. This must be done whenever the size of the inventory changes
        */
        private void ReRenderGrid()
        {
            // Clear the grid
            if (_grids != null)
            {
                for (var i = 0; i < _grids.Length; i++)
                {
                    _grids[i].gameObject.SetActive(false);
                    RecycleImage2(_grids[i]);
                    _grids[i].transform.SetSiblingIndex(i);
                }
            }
            _grids = null;

            // Render new grid
            var containerSize = new Vector2(cellSize.x * inventory.width, cellSize.y * inventory.height);
            Image grid;
            switch (_renderMode)
            {
                case InventoryRenderMode.Single:
                    grid = CreateImage2(_cellSpriteEmpty, true);
                    grid.rectTransform.SetAsFirstSibling();
                    grid.type = Image.Type.Sliced;
                    grid.rectTransform.localPosition = Vector3.zero;
                    grid.rectTransform.sizeDelta = containerSize;
                    _grids = new[] { grid };
                    break;
                default:
                    // Spawn grid images
                    var topLeft = new Vector3(-containerSize.x / 2, -containerSize.y / 2, 0); // Calculate topleft corner
                    var halfCellSize = new Vector3(cellSize.x / 2, cellSize.y / 2, 0); // Calulcate cells half-size
                    _grids = new Image[inventory.width * inventory.height];
                    var c = 0;
                    for (int y = 0; y < inventory.height; y++)
                    {
                        for (int x = 0; x < inventory.width; x++)
                        {
                            grid = CreateImage2(_cellSpriteEmpty, true);
                            grid.gameObject.name = "Grid " + c;
                            grid.rectTransform.SetAsFirstSibling();
                            grid.type = Image.Type.Sliced;
                            grid.rectTransform.localPosition = topLeft + new Vector3(cellSize.x * ((inventory.width - 1) - x), cellSize.y * y, 0) + halfCellSize;
                            grid.rectTransform.sizeDelta = cellSize;
                            _grids[c] = grid;
                            c++;
                        }
                    }
                    break;
            }

            // Set the size of the main RectTransform
            // This is useful as it allowes custom graphical elements
            // suchs as a border to mimic the size of the inventory.
            rectTransform.sizeDelta = containerSize;
        }

        /*
        Clears and renders all items
        */
        private void ReRenderAllItems()
        {
            // Clear all items
            foreach (var item in _items.Values)
            {
                item.itemGO.SetActive(false);
                RecycleItems(item);
            }
            _items.Clear();

            // Add all items
            foreach (var item in inventory.allItems)
            {
                HandleItemAdded(item, item.CountItems);
            }
        }

        /*
        Handler for when inventory.OnItemAdded is invoked
        */
        private void HandleItemAdded(ItemDefinition item, int count)
        {
            var itemThis = CreateItem(item, false);

            if (_renderMode == InventoryRenderMode.Single)
            {
                itemThis.itemGO.transform.localPosition = rectTransform.rect.center;
            }
            else
            {
                itemThis.itemGO.transform.localPosition = GetItemOffset(item);
            }

            _items.Add(item, itemThis);
        }

        /*
        Handler for when inventory.OnItemRemoved is invoked
        */
        private void HandleItemRemoved(ItemDefinition item)
        {
            HandleItemRemoved(item, item.CountItems);
        }
        private void HandleItemRemoved(ItemDefinition item, int count)
        {
            if (_items.ContainsKey(item))
            {
                var itemThis = _items[item];
                itemThis.itemGO.gameObject.SetActive(false);
                RecycleItems(itemThis);
                _items.Remove(item);
            }
        }

        /*
        Handler for when inventory.OnResized is invoked
        */
        private void HandleResized()
        {
            ReRenderGrid();
            ReRenderAllItems();
        }

        /*
         * Create an image with given sprite and settings
         */
        private Image CreateImage2(Sprite sprite, bool raycastTarget)
        {
            var img = _imagePool2.Take();
            img.gameObject.SetActive(true);
            img.sprite = sprite;
            img.rectTransform.sizeDelta = new Vector2(img.sprite.rect.width, img.sprite.rect.height);
            img.transform.SetAsLastSibling();
            img.type = Image.Type.Simple;
            img.raycastTarget = raycastTarget;
            return img;
        }
        private ItemElementInGrid CreateItem(ItemDefinition _item, bool raycastTarget)
        {
            var item = _itemPool.Take();
            item.itemGO.SetActive(true);

            item.img.gameObject.SetActive(true);
            item.img.sprite = _item.sprite;
            item.img.rectTransform.sizeDelta = new Vector2(item.img.sprite.rect.width, item.img.sprite.rect.height);
            item.img.transform.SetAsLastSibling();
            item.img.type = Image.Type.Simple;
            item.img.raycastTarget = raycastTarget;
            item.settextFromPrice(_item.price);
            item.SetCount(((ItemDefinition)_item).CountItems);
            //item.TextUI.text = _item.price.ToString();

            return item;
        }

        /*
         * Recycles given image 
         */
        private void RecycleImage2(Image image)
        {
            image.gameObject.name = "Image";
            image.gameObject.SetActive(false);
            _imagePool2.Recycle(image);
        }
        private void RecycleItems(ItemElementInGrid item)
        {
            //item.itemGO.gameObject.name = "Image";
            item.itemGO.gameObject.SetActive(false);
            _itemPool.Recycle(item);
        }

        /// <summary>
        /// Selects a given item in the inventory
        /// </summary>
        /// <param name="item">Item to select</param>
        /// <param name="blocked">Should the selection be rendered as blocked</param>
        /// <param name="color">The color of the selection</param>
        public void SelectItem(ItemDefinition item, bool blocked, Color color)
        {
            if (item == null) { return; }
            ClearSelection();

            switch (_renderMode)
            {
                case InventoryRenderMode.Single:
                    _grids[0].sprite = blocked ? _cellSpriteBlocked : _cellSpriteSelected;
                    _grids[0].color = color;
                    break;
                default:
                    for (var x = 0; x < item.width; x++)
                    {
                        for (var y = 0; y < item.height; y++)
                        {
                            if (item.IsPartOfShape(new Vector2Int(x, y)))
                            {
                                var p = item.position + new Vector2Int(x, y);
                                if (p.x >= 0 && p.x < inventory.width && p.y >= 0 && p.y < inventory.height)
                                {
                                    var index = p.y * inventory.width + ((inventory.width - 1) - p.x);
                                    _grids[index].sprite = blocked ? _cellSpriteBlocked : _cellSpriteSelected;
                                    _grids[index].color = color;
                                }
                            }
                        }
                    }
                    break;
            }
        }

        /// <summary>
        /// Clears all selections made in this inventory
        /// </summary>
        public void ClearSelection()
        {
            for (var i = 0; i < _grids.Length; i++)
            {
                _grids[i].sprite = _cellSpriteEmpty;
                _grids[i].color = Color.white;
            }
        }

        /*
        Returns the appropriate offset of an item to make it fit nicely in the grid
        */
        internal Vector2 GetItemOffset(ItemDefinition item)
        {
            var x = (-(inventory.width * 0.5f) + item.position.x + item.width * 0.5f) * cellSize.x;
            var y = (-(inventory.height * 0.5f) + item.position.y + item.height * 0.5f) * cellSize.y;
            return new Vector2(x, y);
        }
    }
}