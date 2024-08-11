using TMPro;
using RPG.Combat;
using UnityEngine;

namespace FarrokhGames.Inventory.Examples
{
    public class InventorySelection : MonoBehaviour
    {
        [SerializeField] TMP_Text _text;
        InventoryController[] allControllers;

        void Start()
        {
            _text = GetComponentInChildren<TMP_Text>();
            if (_text != null)
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
                ItemDefinition itm = item as ItemDefinition;

                if (itm != null)
                {
                    switch (itm.Type)
                    {
                        case ItemType.Weapons:
                            Weapon wpn = itm as Weapon;
                            if (_text != null && wpn != null)
                                _text.text = (wpn.GetWeaponRange() > 2.5f) ?
                                  $"Оружие: {itm.Name} Урон: {wpn.GetWeaponDamage()} Тип атаки: Дистанционная Дальность: {wpn.GetWeaponRange()}\nОписание: {wpn.GetDescription()}" :
                                  $"Оружие: {itm.Name} Урон: {wpn.GetWeaponDamage()} Тип атаки: Ближняя Дальность: {wpn.GetWeaponRange()}\nОписание: {wpn.GetDescription()}";
                            break;

                        case ItemType.Armor:
                            Armor armr = itm as Armor;
                            if (_text != null && armr != null)
                                _text.text = $"{itm.Name} Броня: {armr.ArmorValue}\nОписание: {armr.Description}";
                            break;

                        case ItemType.QuestItem:
                            if (_text != null)
                                _text.text = $"{itm.Name} Квестовый предмет\nОписание: {itm.Description}";
                            break;

                        default:
                            if (_text != null)
                                _text.text = $"{itm.Name}\nОписание: {itm.Description}";
                            break;
                    }
                }
            }
            else
            {
                if (_text != null)
                    _text.text = string.Empty;
            }
        }
    }
}
