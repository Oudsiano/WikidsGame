using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RPG.Core;
using TMPro;
using FarrokhGames.Inventory;
using FarrokhGames.Inventory.Examples;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using static LevelChangeObserver;
using RPG.Combat;
using DG.Tweening;

public class UIManager : MonoBehaviour
{
    public DeathUI DeathUI;

    [Header("AgainUI")]
    [SerializeField] private GameObject _againUI;
    [SerializeField] private Button _buttonAgain;
    [SerializeField] private Button _buttonGoToSceneZero;
    [SerializeField] private Button _buttonCancelAgain;

    [Header("CoinUI")]
    [SerializeField] private TMPro.TextMeshProUGUI textCoin;
    [SerializeField] private TMPro.TextMeshProUGUI energyCharger;

    [Header("HelpUI")]
    [SerializeField] public HelpInFirstScene HelpInFirstScene;

    [Header("MarketUI")]
    [SerializeField] private Button _buttonMarket;
    [SerializeField] public UiMarketPanel UiMarketPanel;

    [Header("InventoryBugUI")]
    [SerializeField] private Button _buttonBug;
    [SerializeField] public UIBug uIBug;

    [Header("Map")]
    [SerializeField] public Button ButtonShowMap;
    [SerializeField] public GameObject MapCanvas;
    [SerializeField] public Button _btnCloseMap;

    [Header("PLayerBtns")]
    [SerializeField] private GameObject newWeaponScr;
    [SerializeField] private GameObject newArmorScr;
    [SerializeField] private Button UserInfoBtn;
    [SerializeField] private TMPro.TMP_Text textNamePlayer;

    [Header("PlayerInfoScr")]
    [SerializeField] private GameObject PlayerInfoScr;
    [SerializeField] private TMP_InputField _playerNameInputField;
    [SerializeField] private Button _btnClosePLayerInfoScr;
    [SerializeField] private Button _btnComfirmPLayerInfoScr;

    [Header("Information Icon")]
    [SerializeField] private GameObject IconMapPanel;
    [SerializeField] private TMPro.TMP_Text IconMapText;
    [SerializeField] private GameObject TestNotAvaible;
    [SerializeField] private GameObject ForAttackTest;


    [Header("QuestSector")]
    [SerializeField] private Image _btnQuestBack;
    [SerializeField] private Button _btnQuestScr;
    [SerializeField] private Button _btnQuestScrGray;
    [SerializeField] private Button _btnCloseQuestScr;
    [SerializeField] private GameObject QuestScr;
    [SerializeField] public GameObject OneQuestPref;
    [SerializeField] public ScrollRect QuestsContentScrollRect;

    [Header("OptionsSector")]
    [SerializeField] private Button _btnOptions;
    [SerializeField] private GameObject OptionScr;
    [SerializeField] private Button _btnCloseOptionScr;
    [SerializeField] private Toggle _toggleSound;
    [SerializeField] private Slider _sliderSound;
    [SerializeField] private Toggle _toggleMusic;
    [SerializeField] private Slider _sliderMusic;
    [SerializeField] private Button _btnTest;

    [Header("Zoom Buttons")]
    [SerializeField] private Button _buttonMaxZoom;
    [SerializeField] private Button _buttonMinZoom;

    [Header("Fast Test UI")]
    [SerializeField] private UIFastTest fastTestUI;

    [Header("PanelUI")]
    [SerializeField] private TestTableGenerator _testTableGenerator; // Панель, которую нужно активировать
    [SerializeField] private Button _buttonActivatePanel; // Кнопка для активации панели
    [SerializeField] private Button _buttonClosePanel; // Кнопка для закрытия па

    [Header("Weapon Charges")]
    [SerializeField] private Button _buttonIncreaseCharges;
    [SerializeField] public Weapon WeaponBow; // ?????? ?? ?????? ??????
    [SerializeField] public TMPro.TMP_Text arrowCharges;
    private FollowCamera followCamera;
    private SceneLoader sceneLoader;

    public SceneLoader SceneLoader { get => sceneLoader; set => sceneLoader = value; }
    public FollowCamera FollowCamera { get => followCamera; set => followCamera = value; }

    public void Init()
    {
        fastTestUI.gameObject.SetActive(false);
        SceneLoader = FindObjectOfType<SceneLoader>();

        _buttonAgain.onClick.AddListener(OnClickAgainRegen);
        _buttonGoToSceneZero.onClick.AddListener(OnClickGoToSceneZero);
        _buttonCancelAgain.onClick.AddListener(OnCLickCancelAgain);

        DeathUI.gameObject.SetActive(false);
        _againUI.SetActive(false);

        UiMarketPanel.Init();
        uIBug.Init();
        _buttonMarket.onClick.AddListener(OnClickButtonMarket);
        _buttonBug.onClick.AddListener(OnClickButtonBug);
        _btnQuestScr.onClick.AddListener(OnClickBtnQuest);
        _btnQuestScrGray.onClick.AddListener(OnClickBtnQuest);
        _btnCloseQuestScr.onClick.AddListener(OnClickBtnCloseQuest);

        ButtonShowMap.onClick.AddListener(OnClickButtonMap);
        _btnCloseMap.onClick.AddListener(OnClickBtnCloseMap);

        UserInfoBtn.onClick.AddListener(OnClickUserInfo);
        _btnClosePLayerInfoScr.onClick.AddListener(ClosePlayerScr);
        _btnComfirmPLayerInfoScr.onClick.AddListener(onClickConfirmPLayersScr);

        IGame.Instance.CoinManager.Coins.OnChangeCount += OnChangeMoney;
        IGame.Instance.saveGame.OnChangePlayerName += SaveGame_OnChangePlayerName;

        _toggleSound.onValueChanged.AddListener(OnChangeSoundState);
        _toggleMusic.onValueChanged.AddListener(OnChangeMusicState);
        _sliderSound.onValueChanged.AddListener(OnChangeSoundVolume);
        _sliderMusic.onValueChanged.AddListener(OnChangeMusicVolume);

        _btnOptions.onClick.AddListener(OnClickBtnOption);
        _btnCloseOptionScr.onClick.AddListener(OnCLickCloseOption);
        QuestsContentScrollRect.scrollSensitivity = 20.0f;
        SaveGame_OnChangePlayerName(IGame.Instance.saveGame.PlayerName);

        _buttonMaxZoom.onClick.AddListener(OnClickMaxZoom);
        _buttonMinZoom.onClick.AddListener(OnClickMinZoom);

        // Добавляем слушателя на кнопку активации панели
        _buttonActivatePanel.onClick.AddListener(OnClickActivatePanel);

        // Добавляем слушателя на кнопку закрытия панели
        _buttonClosePanel.onClick.AddListener(OnClickClosePanel);

        _buttonIncreaseCharges.onClick.AddListener(OnButtonIncreaseChargesClick);
        SceneManager.sceneLoaded += SceneLoader_LevelChanged;

        // ?????????? ?????? ?? UIManager ? UIFastTest
        if (WeaponBow != null)
        {
            WeaponBow.OnShotFired += UpdateArrowCharges;
        }
    }

    private void Start()
    {
        FollowCamera = FindObjectOfType<FollowCamera>();
    }

    private void SceneLoader_LevelChanged(Scene arg0, LoadSceneMode arg1)
    {
        GameObject MapCamera = GameObject.Find("CameraForMainMap");
        if (MapCamera != null)
            MapCamera.GetComponent<Camera>().enabled = false;

        SceneComponent sceneComponent = FindObjectOfType<SceneComponent>();
        if (sceneComponent != null)
        {
            if ((sceneComponent.IdScene == allScenes.library) ||
                 (sceneComponent.IdScene == allScenes.holl) ||
                 (sceneComponent.IdScene == allScenes.town1))
            {
                _buttonMaxZoom.gameObject.SetActive(false);
                _buttonMinZoom.gameObject.SetActive(false);
            }
            else
            {
                _buttonMaxZoom.gameObject.SetActive(true);
                _buttonMinZoom.gameObject.SetActive(true);
            }
        }
    }

    private void OnButtonIncreaseChargesClick()
        => IGame.Instance.FastTestsManager.NeedTestForArrows(1);


    public void RegenFastTestUI(int stratIndexFastTests, int endIndexFastTests, int count_arrows, Health targetKillAterTest)
        => fastTestUI.ShowTest(stratIndexFastTests, endIndexFastTests, count_arrows, targetKillAterTest);

    private void OnCLickCloseOption()
    {
        OptionScr.SetActive(false);
        IGame.Instance.SavePlayerPosLikeaPause(false);
        pauseClass.IsOpenUI = false;
    }

    private void OnClickBtnTest()
    {
        uIBug.TryAddEquipToBug(IGame.Instance.QuestManager.allQuestsItems[0]);
    }

    private void OnClickBtnOption()
    {
        OptionScr.SetActive(true);
        IGame.Instance.SavePlayerPosLikeaPause(true);
        pauseClass.IsOpenUI = true;
    }

    public void UpdateIconMapPanel(string text)
    {
        if (text.Length > 0)
        {
            IconMapPanel.SetActive(true);
            IconMapText.text = text;
        }
        else
        {
            IconMapPanel.SetActive(false);
        }
    }

    public void UpdateParamsUI()
    {
        _toggleSound.isOn = IGame.Instance.dataPlayer.playerData.soundOn;
        _toggleMusic.isOn = IGame.Instance.dataPlayer.playerData.musicOn;
        _sliderSound.value = IGame.Instance.dataPlayer.playerData.soundVol;
        _sliderMusic.value = IGame.Instance.dataPlayer.playerData.musicVol;
    }

    private void OnChangeMusicVolume(float arg0)
    {
        SoundManager.SetMusicVolume(arg0);
        AudioManager.instance.MusicVol = arg0;
    }

    private void OnChangeSoundVolume(float arg0)
    {
        AudioManager.instance.SoundVol = arg0;
    }

    private void OnChangeMusicState(bool arg0)
    {
        SoundManager.SetMusicMuted(!arg0);
        AudioManager.instance.MusicON = arg0;
    }

    private void OnChangeSoundState(bool arg0)
    {
        AudioManager.instance.SoundON = arg0;
    }

    private void SaveGame_OnChangePlayerName(string obj)
    {
        if (obj != null && obj.Length < 1)
        {
            textNamePlayer.text = "??????? ???";
            return;
        }

        _playerNameInputField.text = obj;
        textNamePlayer.text = obj;
    }

    private void ClosePlayerScr()
    {
        PlayerInfoScr.SetActive(false);
        IGame.Instance.SavePlayerPosLikeaPause(false);
        pauseClass.IsOpenUI = false;
    }

    private void onClickConfirmPLayersScr()
    {
        IGame.Instance.saveGame.PlayerName = _playerNameInputField.text;
        IGame.Instance.saveGame.MakeSave();
        ClosePlayerScr();
    }

    private void OnClickUserInfo()
    {
        IGame.Instance.SavePlayerPosLikeaPause(true);
        pauseClass.IsOpenUI = true;
        PlayerInfoScr.SetActive(true);
        RegenPLayerInfoScr();
    }

    private void RegenPLayerInfoScr()
    {
        _playerNameInputField.text = IGame.Instance.saveGame.PlayerName;
    }

    public void ShowNewArmor() => newArmorScr.SetActive(true);
    public void ShowNewWeapon() => newWeaponScr.SetActive(true);

    public void OnClickBtnCloseMap()
    {
        IGame.Instance.SavePlayerPosLikeaPause(false);
        pauseClass.IsOpenUI = false;
        MapCanvas.SetActive(false);

        GameObject MapCamera = GameObject.Find("CameraForMainMap");
        if (MapCamera != null)
            MapCamera.GetComponent<Camera>().enabled = false;
    }

    private void OnClickButtonMap()
    {
        if (!MapCanvas.gameObject.activeSelf)
        {
            GameObject MapCamera = GameObject.Find("CameraForMainMap");
            if (MapCamera != null)
                MapCamera.GetComponent<Camera>().enabled = true;
            MapCanvas.gameObject.SetActive(true);
        }
        IGame.Instance.SavePlayerPosLikeaPause(true);
        pauseClass.IsOpenUI = true;
    }

    private void OnChangeMoney(double newValue)
    {
        textCoin.text = newValue.ToString();
    }

    private void OnClickButtonBug()
    {
        uIBug.regen();
        uIBug.gameObject.SetActive(true);
        IGame.Instance.SavePlayerPosLikeaPause(true);
        pauseClass.IsOpenUI = true;
    }

    public void ShowAgainUi()
    {
        _againUI.SetActive(true);
        KeyBoardsEvents.escState = KeyBoardsEvents.EscState.againScr;

        if (IGame.Instance.dataPlayer.playerData.health > 0)
            _buttonCancelAgain.gameObject.SetActive(true);
        else
            _buttonCancelAgain.gameObject.SetActive(false);

        IGame.Instance.SavePlayerPosLikeaPause(true);
        pauseClass.IsOpenUI = true;
    }

    private void closeAgainUI(bool force = false)
    {
        if ((IGame.Instance.dataPlayer.playerData.health > 0) || (force))
        {
            _againUI.SetActive(false);
            KeyBoardsEvents.escState = KeyBoardsEvents.EscState.none;
        }

        IGame.Instance.SavePlayerPosLikeaPause(false);
        pauseClass.IsOpenUI = false;
    }

    public void OnClickButtonMarket() => OpenMarket(0, int.MaxValue);

    public void OpenMarket(int minPrice, int maxPrice)
    {
        UiMarketPanel.Regen(minPrice, maxPrice);
        UiMarketPanel.gameObject.SetActive(true);
        IGame.Instance.SavePlayerPosLikeaPause(true);
        pauseClass.IsOpenUI = true;
    }

    public void OnCLickCancelAgain() => closeAgainUI();

    public void OnClickGoToSceneZero()
    {
        closeAgainUI(true);
        IGame.Instance.gameAPI.SaveUpdater();
        SceneLoader.TryChangeLevel(LevelChangeObserver.allScenes.regionSCene,0);
        AudioManager.instance.PlaySound("ButtonClick");
    }

    private void OnClickAgainRegen()
    {
        SceneLoader.UpdateCurrentLevel();
        closeAgainUI(true);
        AudioManager.instance.PlaySound("ButtonClick");
    }

    public void setEnergyCharger(string c)
    {
        energyCharger.text = c;
    }

    private void ShowQuestPanel()
    {
        QuestScr.SetActive(true);
        IGame.Instance.SavePlayerPosLikeaPause(true);
        pauseClass.IsOpenUI = true;
    }

    private void HideQuestPanel()
    {
        QuestScr.SetActive(false);
        IGame.Instance.SavePlayerPosLikeaPause(false);
        pauseClass.IsOpenUI = false;
    }

    private void OnClickBtnQuest()
    {
        ShowQuestPanel();
    }

    private void OnClickBtnCloseQuest()
    {
        HideQuestPanel();
    }

    public void UpdateGreyBtnQuest(bool showGray)
    {
        if (showGray)
            _btnQuestScrGray.gameObject.SetActive(true);
        else
            _btnQuestScrGray.gameObject.SetActive(false);
    }

    public void UpdateQuestBackImg()
    {
        _btnQuestBack.enabled = IGame.Instance.QuestManager.ShowBackImgForBtn();
    }

    public void IncreaseWeaponCharges(int count)
    {
        WeaponBow.ReloadCharges(count);
        Debug.Log($"Charges increased by {count}. Current charges: " + WeaponBow.GetCurrentCharges());
        UpdateArrowCharges();
    }

    private void OnDestroy()
    {
        // Отписка от события при уничтожении объекта
        if (WeaponBow != null)
        {
            WeaponBow.OnShotFired -= UpdateArrowCharges;
        }
    }
    public void SetArrowsCount()
    {
        if (WeaponBow != null)
        {
            WeaponBow.currentCharges = IGame.Instance.dataPlayer.playerData.arrowsCount;
            arrowCharges.text = WeaponBow.GetCurrentCharges().ToString();
        }
    }
    public void UpdateArrowCharges()
    {
        // Обновление текста количества стрел
        if (arrowCharges != null)
        {
            arrowCharges.text = WeaponBow.GetCurrentCharges().ToString();
            IGame.Instance.saveGame.MakeSave();
        }
    }
    private void OnClickActivatePanel()
    {
        _testTableGenerator.gameObject.SetActive(true);
        pauseClass.IsOpenUI = true;
        _testTableGenerator.Regen();
    }

    private void OnClickClosePanel()
    {
        _testTableGenerator.gameObject.SetActive(false);
        pauseClass.IsOpenUI = false;
    }
    private void Update()
    {
        if (_btnQuestBack.enabled)
            _btnQuestBack.transform.Rotate(Vector3.forward, 25 * Time.deltaTime);
    }

    public void DisplayEmptyTestMessage()
    {
        Debug.Log("Тесты пусты");

        // Убедитесь, что объект ForAttackTest существует
        if (ForAttackTest != null)
        {
            // Найдите CanvasGroup, если он уже существует, или добавьте его, если его нет
            CanvasGroup canvasGroup = ForAttackTest.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = ForAttackTest.AddComponent<CanvasGroup>();
            }

            // Убедитесь, что все предыдущие анимации остановлены
            canvasGroup.DOKill();

            // Активируйте объект и установите прозрачность на 1
            ForAttackTest.SetActive(true);
            canvasGroup.alpha = 1;

            // Начните анимацию исчезновения
            canvasGroup.DOFade(0, 1).SetDelay(2).OnComplete(() =>
            {
                // Деактивируйте объект и восстановите прозрачность
                ForAttackTest.SetActive(false);
                canvasGroup.alpha = 1;
            });
        }
    }


    private void OnClickMaxZoom() => FollowCamera.MaxZoom();
    private void OnClickMinZoom() => FollowCamera.MinZoom();
}
