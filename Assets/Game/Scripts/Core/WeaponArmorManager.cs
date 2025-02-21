using System.Collections.Generic;
using System.Linq;
using Combat;
using Combat.Data;
using Combat.EnumsCombat;
using FarrokhGames.Inventory.Examples;
using UnityEngine;
using UnityEngine.Serialization;

namespace Core
{
    public class WeaponArmorManager : MonoBehaviour
    {
        [FormerlySerializedAs("allWeaponsInGame")] [Header("Weapon")] [SerializeField]
        private List<Weapon> _allWeaponsInGame;

        [FormerlySerializedAs("allArmorsInGame")] [Header("Armors")] [SerializeField]
        private List<Armor> _allArmorsInGame;

        public List<Weapon> AllWeaponsInGame
        {
            get => _allWeaponsInGame;
            set => _allWeaponsInGame = value;
        }

        public List<Armor> AllArmorsInGame
        {
            get => _allArmorsInGame;
            set => _allArmorsInGame = value;
        }

        [FormerlySerializedAs("dafaultPrefab")]
        public PickableEquip DefaultPrefab;

        public Armor GerArmorById(armorID armrId) // TODO rename
        {
            return _allArmorsInGame.Find((item) => item.ArmorName == armrId);
        }

        public bool IsWeaponInGame(string name)
        {
            return _allWeaponsInGame.Any(item => item.name == name);
        }

        public Weapon TryGetWeaponByName(string _name) // TODO need return bool
        {
            if (_name == "" || _name == null)
            {
                _name = "Unarmed"; // TODO can be cached
            }

            foreach (var item in _allWeaponsInGame)
            {
                if (item.name == _name)
                {
                    return item;
                }
            }

            Debug.LogError("Can't return weapon. It can be mistake");
            return null;
        }

        public Armor TryGetArmorByName(string _name) // TODO need return bool
        {
            foreach (var item in _allArmorsInGame)
            {
                if (item.name == _name)
                {
                    return item;
                }
            }

            Debug.LogError("Can't return armor. It can be mistake");
            return null;
        }

        public ItemDefinition TryGetItemByName(string name) // TODO need return bool
        {
            foreach (var item in _allWeaponsInGame)
            {
                if (item.name == name)
                {
                    return item;
                }
            }

            foreach (var item in _allArmorsInGame)
            {
                if (item.name == name)
                {
                    return item;
                }
            }

            foreach (var item in IGame.Instance.QuestManager.AllQuestsItems)
            {
                if (item.name == name)
                {
                    return item;
                }
            }

            Debug.LogError("Can't return item. It can be mistake");
            return null;
        }
    }
}