using RPG.Combat;
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
                    controller.onItemHovered -= HandleItemHover;
                }
        }

        private void HandleItemHover(IInventoryItem item)
        {
            if (item != null)
            {
                ItemDefinition itm = (item as ItemDefinition);

                if (itm.Type == ItemType.Weapons)
                {
                    Weapon wpn = (itm as Weapon);
                    _text.text = (wpn.GetWeaponRange() > 2.5f) ?
                      $"Оружие: {itm.Name} Урон: {wpn.GetWeaponDamage()} Тип атаки: Дистанционная Дальность: {wpn.GetWeaponRange()}\nОписание: {wpn.GetDescription()}" :
                      $"Оружие: {itm.Name}Урон: {wpn.GetWeaponDamage()} Тип атаки: Ближняя Дальность: {wpn.GetWeaponRange()} \nОписание: {wpn.GetDescription()}";
                }
                else if (itm.Type == ItemType.Armor)
                {
                    Armor armr = (itm as Armor);
                    _text.text = $"{itm.Name} Броня: {armr.ArmorValue}\nОписание: {armr.Description}";
                }
            }
            else
            {
                _text.text = string.Empty;
            }
        }
    }
}
