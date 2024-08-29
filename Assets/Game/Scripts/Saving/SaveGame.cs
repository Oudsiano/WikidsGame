using DG.Tweening;
using FarrokhGames.Inventory.Examples;
using RPG.Combat;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class OneItemForSave
{
    public int count;
    public string name;

    public OneItemForSave(int _c, string _n)
    {
        count = _c;
        name = _n;
    }
}

public class SaveGame
{
    //Заготовка для сейв гейма

    //Сумка
    private List<ItemDefinition> bugItems;
    private Armor equipedArmor;
    private Weapon equipedWeapon;

    private double coins;

    private string playerName;
    public event Action<string> OnChangePlayerName;
    public event Action OnLoadItems;

    public Weapon EquipedWeapon { get => equipedWeapon; set { equipedWeapon = value; } }
    public Armor EquipedArmor { get => equipedArmor; set { equipedArmor = value; } }
    public double Coins { get => coins; set { coins = value; IGame.Instance.CoinManager.Coins.SetCount(value); } }

    public List<ItemDefinition> BugItems
    {
        get => bugItems;
        set => bugItems = value;
    }
    public string PlayerName { get => playerName; set { playerName = value; OnChangePlayerName?.Invoke(playerName); } }

    //Порталы в неправильном порядке работают. Их надо сохранять в начале след карты. А для этого запоминать.
    public RPG.Combat.Weapon bonusWeapon;
    public Armor bonusArmor;
    private SceneComponent sceneComponent;

    public SaveGame()
    {
        BugItems = new List<ItemDefinition>();


        EquipedArmor = new Armor();
        EquipedWeapon = (Weapon)IGame.Instance.WeaponArmorManager.TryGetWeaponByName("Sword").CreateInstance();
    }

    public void MakePortalSave(Weapon _bonusw, Armor _bonusA, SceneComponent _sceneComponent)
    {
        bonusWeapon = _bonusw;
        bonusArmor = _bonusA;
        sceneComponent = _sceneComponent;
    }
    public void SetBonusWeaponAndArmorIfNeed()
    {
        if (IGame.Instance.dataPlayer.playerData.FinishedRegionsIDs.Contains((int)sceneComponent.IdScene))
            return;

        if (bonusWeapon != null)
        {
            if (IGame.Instance.UIManager.uIBug.AddEquipInBugIfNotExist(IGame.Instance.WeaponArmorManager.TryGetItemByName(bonusWeapon.name)))
            {
                Debug.Log("show weapon");
                IGame.Instance.UIManager.ShowNewWeapon();
                IGame.Instance.gameAPI.SaveUpdater();

                if (!IGame.Instance.dataPlayer.playerData.alreadyExistWeapons.Contains(bonusWeapon.name))
                {
                    IGame.Instance.dataPlayer.playerData.alreadyExistWeapons.Add(bonusWeapon.name);
                }
            }
            else
            {
                if (bonusWeapon.price > 0)
                    TextDisplay(bonusWeapon.price / 4, "Вы получили бонусные деньги за прохождение");
            }
        }

        if (bonusArmor != null)
        {
            if (IGame.Instance.UIManager.uIBug.AddEquipInBugIfNotExist(IGame.Instance.WeaponArmorManager.TryGetItemByName(bonusArmor.name)))
            {
                IGame.Instance.UIManager.ShowNewArmor();
                IGame.Instance.gameAPI.SaveUpdater();

                if (!IGame.Instance.dataPlayer.playerData.alreadyExistWeapons.Contains(bonusArmor.name))
                {
                    IGame.Instance.dataPlayer.playerData.alreadyExistWeapons.Add(bonusArmor.name);
                }
            }
            else
            {
                if (bonusArmor.price > 0)
                    TextDisplay(bonusArmor.price / 4, "Вы получили бонусные деньги за прохождение");
            }
        }

        bonusWeapon = null;
        bonusArmor = null;
    }
    private void TextDisplay(int coins, string text)
    {
        IGame.Instance.CoinManager.Coins.ChangeCount(coins);

        TextMeshProUGUI messageText;
        Canvas canvas;
        GameObject panel;

        // Создание Canvas
        GameObject canvasObj = new GameObject("Canvas");
        canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 100; // Установка sortOrder
        canvasObj.AddComponent<CanvasScaler>();
        canvasObj.AddComponent<GraphicRaycaster>();

        // Создание панели
        panel = new GameObject("MessagePanel");
        panel.transform.SetParent(canvas.transform, false);
        RectTransform rectTransform = panel.AddComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(800, 300);
        panel.AddComponent<CanvasRenderer>();
        Image image = panel.AddComponent<Image>();
        image.color = new Color(0.5f, 0.5f, 0.5f, 0.5f); // Серый полупрозрачный цвет

        // Создание текста
        GameObject textObj = new GameObject("MessageText");
        textObj.transform.SetParent(panel.transform, false);
        messageText = textObj.AddComponent<TextMeshProUGUI>();
        if (coins != 0)
            messageText.text = text + " " + coins;
        else
            messageText.text = text;
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
        IGame.Instance.dataPlayer.playerData.armorIdToload = (int)EquipedArmor.ArmorName;
        IGame.Instance.dataPlayer.playerData.weaponToLoad = EquipedWeapon.name;

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
        //IGame.Instance.dataPLayer.playerData.containsBug = tempBug.ToArray();
        IGame.Instance.dataPlayer.playerData.containsBug = null;
        IGame.Instance.dataPlayer.playerData.containsBug2 = tempBug;
        IGame.Instance.dataPlayer.playerData.coins = Coins;

        IGame.Instance.dataPlayer.playerData.playerName = PlayerName;

        IGame.Instance.dataPlayer.playerData.soundOn = AudioManager.instance.SoundON;
        IGame.Instance.dataPlayer.playerData.soundVol = AudioManager.instance.SoundVol;
        IGame.Instance.dataPlayer.playerData.musicOn = !SoundManager.GetMusicMuted();
        IGame.Instance.dataPlayer.playerData.musicVol = SoundManager.GetMusicVolume();

        IGame.Instance.dataPlayer.playerData.arrowsCount = IGame.Instance.UIManager.WeaponBow.currentCharges;

        IGame.Instance.gameAPI.SaveUpdater();
    }

    public void MakeLoad()
    {
        IGame.Instance.UIManager.UpdateParamsUI();


        BugItems.Clear();
        EquipedArmor = IGame.Instance.WeaponArmorManager.GerArmorById((armorID)IGame.Instance.dataPlayer.playerData.armorIdToload);
        if (IGame.Instance.dataPlayer.playerData.weaponToLoad == "Basic Bow")
            IGame.Instance.dataPlayer.playerData.weaponToLoad = "";
        if (IGame.Instance.dataPlayer.playerData.weaponToLoad == "")
            IGame.Instance.dataPlayer.playerData.weaponToLoad = "Sword";

        EquipedWeapon = IGame.Instance.WeaponArmorManager.TryGetWeaponByName(IGame.Instance.dataPlayer.playerData.weaponToLoad);

        if (IGame.Instance.dataPlayer.playerData.playerName != "")
            PlayerName = IGame.Instance.dataPlayer.playerData.playerName;

        if (IGame.Instance.dataPlayer.playerData.containsBug2 == null)
            IGame.Instance.dataPlayer.playerData.containsBug2 = new List<OneItemForSave>();

        if (IGame.Instance.dataPlayer.playerData.containsBug != null)
        {
            if (IGame.Instance.dataPlayer.playerData.containsBug.Length > 99)
            {
                IGame.Instance.dataPlayer.playerData.containsBug = new string[0];
                MakeSave();
            }
            else
            if (IGame.Instance.dataPlayer.playerData.containsBug != null)
                foreach (var item in IGame.Instance.dataPlayer.playerData.containsBug)
                {
                    BugItems.Add((ItemDefinition)IGame.Instance.WeaponArmorManager.TryGetItemByName(item)
                        .CreateInstance()
                        );
                }
        }

        IGame.Instance.dataPlayer.playerData.containsBug2.RemoveAll(item => item.name == "Basic Bow");

        foreach (var item in IGame.Instance.dataPlayer.playerData.containsBug2)
        {
            ItemDefinition newI = (ItemDefinition)IGame.Instance.WeaponArmorManager.TryGetItemByName(item.name.Replace("(Clone)", ""))
                .CreateInstance();
            newI.CountItems = item.count;

            BugItems.Add(newI);
        }

        OnLoadItems?.Invoke();
        Coins = IGame.Instance.dataPlayer.playerData.coins;
        IGame.Instance.UIManager.SetArrowsCount();
    }
}
