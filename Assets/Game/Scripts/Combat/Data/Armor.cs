using Combat.EnumsCombat;
using UI.Inventory.Data;
using UnityEngine;
using UnityEngine.Serialization;

namespace Combat.Data
{
    [CreateAssetMenu(fileName = "Armor", menuName = "Armors", order = 0)]
    public class Armor : ItemDefinition // TODO data -> its a data(SO)
    {
        [FormerlySerializedAs("armorType")] [Header("Core")] [SerializeField]
        private ArmorType _armorType;

        [FormerlySerializedAs("armorName")] [SerializeField]
        private armorID _armorName;

        [FormerlySerializedAs("armor")] [Header("Stats")] [SerializeField]
        private float _armor;

        [FormerlySerializedAs("description")] [Header("Description")] [SerializeField] [TextArea]
        private string _description; // Описание брони

        public float ArmorValue => _armor;
        public armorID ArmorName => _armorName;

        //public string Description { get => description; set => description = value; } // Свойство для описания

        public void EquipIt() // TODO cannot be dynamically used 
        {
            UnEquipOtherArmorFromPlayer();

            IGame.Instance.saveGame.EquipedArmor = this;
            
            foreach (var armor in IGame.Instance.playerController.PlayerArmorManager.AllArmors)
            {
                if (ArmorName == armor.armorID)
                {
                    foreach (var item in armor.armorGO)
                    {
                        item.SetActive(true);
                    }
                }
            }
        }

        public void UnEquip() // TODO cannot be dynamically used 
        {
            foreach (var armor in IGame.Instance.playerController.PlayerArmorManager.AllArmors)
            {
                if (ArmorName == armor.armorID)
                {
                    foreach (var item in armor.armorGO)
                    {
                        item.SetActive(false);
                    }
                }
            }

            IGame.Instance.WeaponArmorManager.GerArmorById(armorID.none).EquipIt();
        }

        public void UnEquipOtherArmorFromPlayer() // TODO cannot be dynamically used 
        {
            foreach (var armor in IGame.Instance.playerController.PlayerArmorManager.AllArmors)
            {
                foreach (var item in armor.armorGO)
                {
                    item.SetActive(false);
                }
            }
        }
    }
}