using UI;
using UI.Inventory;
using UI.Inventory.Data;
using UnityEngine;
using UnityEngine.Serialization;

namespace AINavigation
{
    public class GiveItem : MonoBehaviour // TODO rename
    {
        [FormerlySerializedAs("Item")] [SerializeField] private ItemDefinition _item; // TODO rename

        private UIManager _uiManager;
        
        public void Construct(UIManager uiManager)
        {
            _uiManager = uiManager;
        }
        
        public void TryAddItemItem()
        {
            if (_item != null)
                _uiManager.uIBug.TryAddEquipToBug(_item.CreateInstance());
        }
    }
}