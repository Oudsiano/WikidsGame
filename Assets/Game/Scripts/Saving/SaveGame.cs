using System;
using System.Collections.Generic;
using Combat.Data;
using Combat.EnumsCombat;
using Core;
using Data;
using DG.Tweening;
using SceneManagement;
using SceneManagement.Enums;
using TMPro;
using UI;
using UI.Inventory.Data;
using UnityEngine;
using UnityEngine.UI;

namespace Saving
{
    public class SaveGame // TODO check saves before refactor
    {
        //Порталы в неправильном порядке работают. Их надо сохранять в начале след карты. А для этого запоминать.
        public Weapon bonusWeapon;
        public Armor bonusArmor;
        private string _sceneNameForPortal;

        private List<ItemDefinition> bugItems; // TODO rename bag
        private Armor equipedArmor;
        private Weapon equipedWeapon;

        private double coins;
        private string playerName;

        private WeaponArmorManager _weaponArmorManager;
        private CoinManager _coinManager;
        private DataPlayer _dataPlayer;
        private GameAPI _gameAPI;
        private UIManager _uiManager;

        public void Construct(GameAPI gameAPI,
            WeaponArmorManager weaponArmorManager,
            CoinManager coinManager,
            DataPlayer dataPlayer, UIManager uiManager)
        {
            Debug.Log("SaveGame constructed");
            _weaponArmorManager = weaponArmorManager;
            _coinManager = coinManager;
            _dataPlayer = dataPlayer;
            _gameAPI = gameAPI;
            _uiManager = uiManager;

            BugItems = new List<ItemDefinition>();

            // EquipedArmor = new Armor();
            // EquipedWeapon =
            //     (Weapon)_weaponArmorManager.TryGetWeaponByName("Sword")
            //         .CreateInstance(); // TODO expensive unboxing
        }

        public event Action<string> OnChangePlayerName;
        public event Action OnLoadItems;

        public Weapon EquipedWeapon
        {
            get => equipedWeapon;
            set { equipedWeapon = value; }
        }

        public Armor EquipedArmor
        {
            get => equipedArmor;
            set { equipedArmor = value; }
        }

        public double Coins
        {
            get => coins;
            set
            {
                coins = value;
                _coinManager.Coins.SetCount(value);
            }
        }

        public List<ItemDefinition> BugItems
        {
            get => bugItems;
            set => bugItems = value;
        }

        public string PlayerName
        {
            get => playerName;
            set
            {
                playerName = value;
                OnChangePlayerName?.Invoke(playerName);
            }
        }

        public void MakePortalSave(Weapon _bonusw, Armor _bonusA, SceneComponent _sceneComponent)
        {
            bonusWeapon = _bonusw;
            bonusArmor = _bonusA;
            _sceneNameForPortal = _sceneComponent.SceneName;
        }

        public void SetBonusWeaponAndArmorIfNeed()
        {
            if (_dataPlayer.PlayerData.FinishedRegionsName.Contains(_sceneNameForPortal))
            {
                return;
            }

            AddFinishedRegion(_sceneNameForPortal);

            if (bonusWeapon != null)
            {
                if (_uiManager.uIBug.AddEquipInBugIfNotExist(
                        _weaponArmorManager.TryGetItemByName(bonusWeapon.name)))
                {
                    Debug.Log("show weapon");
                    _uiManager.ShowNewWeapon();
                    _gameAPI.SaveUpdater();

                    if (_dataPlayer.PlayerData.alreadyExistWeapons.Contains(bonusWeapon.name) ==
                        false) // TODO try add
                    {
                        _dataPlayer.PlayerData.alreadyExistWeapons.Add(bonusWeapon.name);
                    }
                }
                else
                {
                    if (bonusWeapon.price > 0)
                    {
                        TextDisplay(bonusWeapon.price / 4, "Вы получили бонусные деньги за прохождение");
                    }
                }
            }

            if (bonusArmor != null) // TODO DUPLICATE
            {
                if (_uiManager.uIBug.AddEquipInBugIfNotExist(
                        _weaponArmorManager.TryGetItemByName(bonusArmor.name)))
                {
                    _uiManager.ShowNewArmor();
                    _gameAPI.SaveUpdater();

                    if (_dataPlayer.PlayerData.alreadyExistWeapons.Contains(bonusArmor.name) ==
                        false) // TODO try add DUPLICATE
                    {
                        _dataPlayer.PlayerData.alreadyExistWeapons.Add(bonusArmor.name);
                    }
                }
                else
                {
                    if (bonusArmor.price > 0)
                    {
                        TextDisplay(bonusArmor.price / 4, "Вы получили бонусные деньги за прохождение");
                    }
                }
            }

            bonusWeapon = null;
            bonusArmor = null;
            _sceneNameForPortal = null;
        }

        public void SaveItemToBug(ItemDefinition item)
        {
            BugItems.Add(item);
        }

        public void RemoveItemFromBug(ItemDefinition item)
        {
            if (BugItems.Contains(item))
            {
                BugItems.Remove(item);
            }
            else
            {
                Debug.LogError("Потенциально ошибка");
            }
        }


        public void MakeSave()
        {
            _dataPlayer.PlayerData.armorIdToload = (int)EquipedArmor.ArmorName;
            _dataPlayer.PlayerData.weaponToLoad = EquipedWeapon.name;
            
            Debug.Log($"[SaveGame] Сохраняем weaponToLoad: {_dataPlayer.PlayerData.weaponToLoad}");


            List<OneItemForSave> tempBug = new List<OneItemForSave>();
            /*for (int i = BugItems.Count-1; i >= 0; i--)
        {
            for (int i2  = i-1; i2 >= 0; i2--)
            {
                if (BugItems[i].name == BugItems[i2].name)
                {
                    BugItems[i].CountItems += BugItems[i2].CountItems;
                    BugItems[i2].CountItems = 0;
                }
            }
        }
        for (int i = BugItems.Count-1; i >= 0; i--)
            if (BugItems[i].CountItems == 0) bugItems.RemoveAt(i);*/

            foreach (var item in BugItems)
            {
                item.CountItems = 1;

                tempBug.Add(new OneItemForSave(item.CountItems, item.name));
            }

            //_dataPLayer.playerData.containsBug = tempBug.ToArray();
            _dataPlayer.PlayerData.containsBug = null;
            _dataPlayer.PlayerData.containsBug2 = tempBug;
            _dataPlayer.PlayerData.coins = Coins;

            _dataPlayer.PlayerData.playerName = PlayerName;

            _dataPlayer.PlayerData.soundOn = AudioManager.Instance.SoundON;
            _dataPlayer.PlayerData.soundVol = AudioManager.Instance.SoundVol;
            _dataPlayer.PlayerData.musicOn = !SoundManager.GetMusicMuted();
            _dataPlayer.PlayerData.musicVol = SoundManager.GetMusicVolume();

            _dataPlayer.PlayerData.arrowsCount = _uiManager.WeaponBow._currentCharges;

            _gameAPI.SaveUpdater();
        }

        public void MakeLoad()
        {
            _uiManager.UpdateParamsUI();

            BugItems.Clear();
            EquipedArmor =
                _weaponArmorManager.GerArmorById((armorID)_dataPlayer.PlayerData
                    .armorIdToload);
            // if (_dataPlayer.PlayerData.weaponToLoad == "Basic Bow")
            // {
            //     _dataPlayer.PlayerData.weaponToLoad = "";
            // }

            if (_dataPlayer.PlayerData.weaponToLoad == "")
            {
                // _dataPlayer.PlayerData.weaponToLoad = "Sword";
                _dataPlayer.PlayerData.weaponToLoad = "Basic Bow";
            }
            
            EquipedWeapon =
                _weaponArmorManager.TryGetWeaponByName(_dataPlayer.PlayerData.weaponToLoad);
            
            Debug.Log($"[SaveGame] Loaded weapon: {_dataPlayer.PlayerData.weaponToLoad} → {EquipedWeapon}");
            

            if (_dataPlayer.PlayerData.playerName != "")
            {
                PlayerName = _dataPlayer.PlayerData.playerName;
            }

            if (_dataPlayer.PlayerData.containsBug2 == null)
            {
                _dataPlayer.PlayerData.containsBug2 = new List<OneItemForSave>();
            }

            if (_dataPlayer.PlayerData.containsBug != null)
            {
                if (_dataPlayer.PlayerData.containsBug.Length > 99)
                {
                    _dataPlayer.PlayerData.containsBug = new string[0];
                    MakeSave();
                }
                else if (_dataPlayer.PlayerData.containsBug != null)
                {
                    foreach (var item in _dataPlayer.PlayerData.containsBug)
                    {
                        BugItems.Add((ItemDefinition)_weaponArmorManager.TryGetItemByName(item)
                            .CreateInstance()
                        );
                    }
                }
            }

            _dataPlayer.PlayerData.containsBug2.RemoveAll(item =>
                item.name == "Basic Bow"); // TODO can be cached

            foreach (var item in _dataPlayer.PlayerData.containsBug2)
            {
                ItemDefinition newI = _weaponArmorManager
                    .TryGetItemByName(item.name.Replace("(Clone)", ""))
                    .CreateInstance();
                newI.CountItems = item.count;

                BugItems.Add(newI);
            }

            OnLoadItems?.Invoke();
            Coins = _dataPlayer.PlayerData.coins;
            _uiManager.SetArrowsCount();
        }
        
        public void AddFinishedRegion(string regionName)
        {
             if (string.IsNullOrEmpty(regionName))
            {
                return;
            }

            if (_dataPlayer == null || _dataPlayer.PlayerData == null)
            {
                return;
            }

            if (_dataPlayer.PlayerData.FinishedRegionsName == null)
            {
                return;
            }

            int regionHash = regionName.GetHashCode();

            // Проверяем, есть ли уже такой хэш в списке
            bool isDuplicate = _dataPlayer.PlayerData.FinishedRegionsName
                .Exists(region => region.GetHashCode() == regionHash && region == regionName);

            if (!isDuplicate) // Если дубликата нет, добавляем
            {
                _dataPlayer.PlayerData.FinishedRegionsName.Add(regionName);
            }
        }
        
        private void TextDisplay(int coins, string text) // TODO refactor and create Canvas Factory
        {
            _coinManager.Coins.ChangeCount(coins);

            TextMeshProUGUI messageText;
            Canvas canvas;
            GameObject panel;

            // Создание Canvas
            GameObject canvasObj = new GameObject("Canvas");
            canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 100; // Установка sortOrder // TODO magic numbers
            canvasObj.AddComponent<CanvasScaler>();
            canvasObj.AddComponent<GraphicRaycaster>();

            // Создание панели
            panel = new GameObject("MessagePanel");
            panel.transform.SetParent(canvas.transform, false);
            RectTransform rectTransform = panel.AddComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(800, 300); // TODO magic numbers
            panel.AddComponent<CanvasRenderer>();
            Image image = panel.AddComponent<Image>();
            image.color = new Color(0.5f, 0.5f, 0.5f, 0.5f); // Серый полупрозрачный цвет // TODO magic numbers

            // Создание текста
            GameObject textObj = new GameObject("MessageText");
            textObj.transform.SetParent(panel.transform, false);
            messageText = textObj.AddComponent<TextMeshProUGUI>();

            if (coins != 0)
            {
                messageText.text = text + " " + coins;
            }
            else
            {
                messageText.text = text;
            }

            messageText.alignment = TextAlignmentOptions.Center;
            messageText.color = Color.black;
            RectTransform textRectTransform = textObj.GetComponent<RectTransform>();
            textRectTransform.sizeDelta = new Vector2(500, textRectTransform.sizeDelta.y);
            textRectTransform.anchoredPosition = Vector2.zero;

            // Запуск анимации для исчезновения и удаления панели
            DOVirtual.DelayedCall(3, () =>
            {
                // Плавное исчезновение за 2 секунды
                Image panelImage = panel.GetComponent<Image>();
                TextMeshProUGUI textMesh = messageText;

                // Анимация исчезновения панели
                panelImage.DOFade(0, 2);

                // Анимация исчезновения текста
                textMesh.DOFade(0, 2).OnComplete(() =>
                {
                    // Удаление панели после завершения анимации
                    IGame.Destroy(panel);
                });
            });
        }
    }
}