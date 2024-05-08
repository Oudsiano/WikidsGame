using UnityEngine;
using UnityEngine.UI;

namespace FarrokhGames.Inventory.Examples
{
    public class InventorySelection : MonoBehaviour
    {
        Text _text;
        InventoryController[] allControllers;

        void Start()
        {
            _text = GetComponentInChildren<Text>();
            _text.text = string.Empty;

            allControllers = GameObject.FindObjectsOfType<InventoryController>();

            foreach (var controller in allControllers)
            {
                controller.onItemHovered += HandleItemHover;
            }
        }

        private void OnDestroy()
        {
            if (allControllers != null)
            foreach (var controller in allControllers)
            {
                    Debug.Log("destr");
                controller.onItemHovered -= HandleItemHover;
            }
        }

        private void HandleItemHover(IInventoryItem item)
        {
            if (item != null)
            {
                _text.text = (item as ItemDefinition).Name;
            }
            else
            {
                _text.text = string.Empty;
            }
        }
    }
}