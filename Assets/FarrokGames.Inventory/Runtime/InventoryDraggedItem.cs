using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using UnityEngine.UI;

namespace FarrokhGames.Inventory
{
    /// <summary>
    /// Class for keeping track of dragged items
    /// </summary>
    public class InventoryDraggedItem
    {
        public enum DropMode
        {
            Added,
            Swapped,
            Returned,
            Dropped,
        }

        /// <summary>
        /// Returns the InventoryController this item originated from
        /// </summary>
        public InventoryController originalController { get; private set; }

        /// <summary>
        /// Returns the point inside the inventory from which this item originated from
        /// </summary>
        public Vector2Int originPoint { get; private set; }

        /// <summary>
        /// Returns the item-instance that is being dragged
        /// </summary>
        public IInventoryItem item { get; private set; }

        /// <summary>
        /// Gets or sets the InventoryController currently in control of this item
        /// </summary>
        public InventoryController currentController;

        private readonly Canvas _canvas;
        private readonly RectTransform _canvasRect;
        private readonly Image _image;
        private Vector2 _offset;


        private System.Action<DropMode> _actionAfterDrop;
        private DropMode _dropModeForAction;


        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="canvas">The canvas</param>
        /// <param name="originalController">The InventoryController this item originated from</param>
        /// <param name="originPoint">The point inside the inventory from which this item originated from</param>
        /// <param name="item">The item-instance that is being dragged</param>
        /// <param name="offset">The starting offset of this item</param>
        [SuppressMessage("ReSharper", "Unity.InefficientPropertyAccess")]
        public InventoryDraggedItem(
            Canvas canvas,
            InventoryController originalController,
            Vector2Int originPoint,
            IInventoryItem item,
            Vector2 offset)
        {
            this.originalController = originalController;
            currentController = this.originalController;
            this.originPoint = originPoint;
            this.item = item;

            _canvas = canvas;
            _canvasRect = canvas.transform as RectTransform;

            _offset = offset;

            // Create an image representing the dragged item
            _image = new GameObject("DraggedItem").AddComponent<Image>();
            _image.raycastTarget = false;
            _image.transform.SetParent(_canvas.transform);
            _image.transform.SetAsLastSibling();
            _image.transform.localScale = Vector3.one;
            _image.sprite = item.sprite;
            _image.SetNativeSize();
        }

        /// <summary>
        /// Gets or sets the position of this dragged item
        /// </summary>
        public Vector2 position
        {
            set
            {
                // Move the image
                var camera = _canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : _canvas.worldCamera;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(_canvasRect, value + _offset, camera, out var newValue);
                _image.rectTransform.localPosition = newValue;


                // Make selections
                if (currentController != null)
                {
                    item.position = currentController.ScreenToGrid(value + _offset + GetDraggedItemOffset(currentController.inventoryRenderer, item));
                    var canAdd = currentController.inventory.CanAddAt(item, item.position) || CanSwap();
                    currentController.inventoryRenderer.SelectItem(item, !canAdd, Color.white);
                }

                // Slowly animate the item towards the center of the mouse pointer
                _offset = Vector2.Lerp(_offset, Vector2.zero, Time.deltaTime * 10f);
            }
        }

        /// <summary>
        /// Drop this item at the given position
        /// </summary>
        public void Drop(Vector2 pos, System.Action<DropMode> afterDrop)
        {
            _actionAfterDrop = afterDrop;

            DropMode mode;
            if (currentController != null)
            {
                

                var grid = currentController.ScreenToGrid(pos + _offset + GetDraggedItemOffset(currentController.inventoryRenderer, item));

                // Try to add new item
                if (currentController.inventory.CanAddAt(item, grid))
                {
                    if (originalController.inventory.isMarket && originalController!=currentController)
                    {
                        IGame.Instance.UIManager.UiMarketPanel.InitConfirmMarketUI(OnMarketAccept, OnMarketDecline, grid, item);
                    }
                    else
                    {
                        currentController.inventory.TryAddAt(item, grid); // Place the item in a new location
                        mode = DropMode.Added;
                        _actionAfterDrop(mode);
                    }
                }
                // Adding did not work, try to swap
                else if (!originalController.inventory.isMarket && CanSwap())
                {
                    var otherItem = currentController.inventory.allItems[0];
                    currentController.inventory.TryRemove(otherItem);
                    originalController.inventory.TryAdd(otherItem);
                    currentController.inventory.TryAdd(item);
                    mode = DropMode.Swapped;
                    _actionAfterDrop(mode);
                }
                // Could not add or swap, return the item
                else
                {
                    originalController.inventory.TryAddAt(item, originPoint); // Return the item to its previous location
                    mode = DropMode.Returned;
                    _actionAfterDrop(mode);
                }

                currentController.inventoryRenderer.ClearSelection();
            }
            else if (!originalController.inventory.isMarket)
            {
                mode = DropMode.Dropped;
                if (!originalController.inventory.TryForceDrop(item)) // Drop the item on the ground
                {
                    originalController.inventory.TryAddAt(item, originPoint);
                }
            }

            // Destroy the image representing the item
            Object.Destroy(_image.gameObject);
        }

        public void OnMarketAccept(Vector2Int grid)
        {
            currentController.inventory.TryAddAt(item, grid); // Place the item in a new location
            _dropModeForAction = DropMode.Added;
        }

        public void OnMarketDecline()
        {
            originalController.inventory.TryAddAt(item, originPoint); // Return the item to its previous location
            _dropModeForAction = DropMode.Returned;
            _actionAfterDrop(_dropModeForAction);
        }

        public void DelayedDrop(Vector2 pos)
        {

        }

        /*
         * Returns the offset between dragged item and the grid 
         */
        private Vector2 GetDraggedItemOffset(InventoryRenderer renderer, IInventoryItem item)
        {
            var scale = new Vector2(
                Screen.width / _canvasRect.sizeDelta.x,
                Screen.height / _canvasRect.sizeDelta.y
            );
            var gx = -(item.width * renderer.cellSize.x / 2f) + (renderer.cellSize.x / 2);
            var gy = -(item.height * renderer.cellSize.y / 2f) + (renderer.cellSize.y / 2);
            return new Vector2(gx, gy) * scale;
        }
        
        /* 
         * Returns true if its possible to swap
         */
        private bool CanSwap()
        {
            if (!currentController.inventory.CanSwap(item)) return false;
            var otherItem = currentController.inventory.allItems[0];
            return originalController.inventory.CanAdd(otherItem) && currentController.inventory.CanRemove(otherItem);
        }
    }
}