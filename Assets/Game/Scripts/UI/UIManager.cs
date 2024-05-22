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

    [SerializeField] private Toggle _toggleSound;
    [SerializeField] private Slider _sliderSound;
    [SerializeField] private Toggle _toggleMusic;
    [SerializeField] private Slider _sliderMusic;

    private SceneLoader sceneLoader;

    public SceneLoader SceneLoader { get => sceneLoader; set => sceneLoader = value; }

    public void Init()
    {
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

        ButtonShowMap.onClick.AddListener(OnClickButtonMap);
        _btnCloseMap.onClick.AddListener(OnClickBtnCloseMap);

        UserInfoBtn.onClick.AddListener(OnClickUserInfo);

        _btnClosePLayerInfoScr.onClick.AddListener( ClosePlayerScr);
        _btnComfirmPLayerInfoScr.onClick.AddListener(onClickConfirmPLayersScr);

        IGame.Instance.CoinManager.Coins.OnChangeCount += OnChangeMoney;
        IGame.Instance.saveGame.OnChangePlayerName += SaveGame_OnChangePlayerName;

        _toggleSound.onValueChanged.AddListener(OnChangeSoundState);
        _toggleMusic.onValueChanged.AddListener(OnChangeMusicState);
        _sliderSound.onValueChanged.AddListener(OnChangeSoundVolume);
        _sliderMusic.onValueChanged.AddListener(OnChangeMusicVolume);


        SaveGame_OnChangePlayerName(IGame.Instance.saveGame.PlayerName);
    }

    public void UpdateParamsUI()
    {
        _toggleSound.isOn = IGame.Instance.dataPLayer.playerData.soundOn;
        _toggleMusic.isOn = IGame.Instance.dataPLayer.playerData.musicOn;
        _sliderSound.value = IGame.Instance.dataPLayer.playerData.soundVol;
        _sliderMusic.value = IGame.Instance.dataPLayer.playerData.musicVol;
    }

    private void OnChangeMusicVolume(float arg0)
    {
        AudioManager.instance.MusicVol = arg0;
    }

    private void OnChangeSoundVolume(float arg0)
    {
        AudioManager.instance.SoundVol = arg0;
    }

    private void OnChangeMusicState(bool arg0)
    {
        AudioManager.instance.MusicON = arg0;
    }

    private void OnChangeSoundState(bool arg0)
    {
        AudioManager.instance.SoundON = arg0;
    }

    private void SaveGame_OnChangePlayerName(string obj)
    {
        _playerNameInputField.text = obj;
        textNamePlayer.text = obj;
    }

    private void Start()
    {
    }

    private void ClosePlayerScr() 
    {
        PlayerInfoScr.SetActive(false); IGame.Instance.SavePlayerPosLikeaPause(false);
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
        MapCanvas.SetActive(false);
    }

    private void OnClickButtonMap()
    {
        if (!MapCanvas.gameObject.activeSelf) MapCanvas.gameObject.SetActive(true);
        IGame.Instance.SavePlayerPosLikeaPause(true);
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

    }

    public void ShowAgainUi()
    {
        _againUI.SetActive(true);
        KeyBoardsEvents.escState = KeyBoardsEvents.EscState.againScr;

        if (IGame.Instance.dataPLayer.playerData.health > 0)
            _buttonCancelAgain.gameObject.SetActive(true);
        else
            _buttonCancelAgain.gameObject.SetActive(false);


        IGame.Instance.SavePlayerPosLikeaPause(true);
    }

    private void closeAgainUI(bool force = false)
    {
        if ((IGame.Instance.dataPLayer.playerData.health > 0) || (force))
        {
            _againUI.SetActive(false);
            KeyBoardsEvents.escState = KeyBoardsEvents.EscState.none;
        }


        IGame.Instance.SavePlayerPosLikeaPause(false);
    }

    public void OnClickButtonMarket() => OpenMarket(0, int.MaxValue);

    public void OpenMarket(int minPrice, int maxPrice)
    {
        UiMarketPanel.Regen(minPrice, maxPrice);
        UiMarketPanel.gameObject.SetActive(true);
        IGame.Instance.SavePlayerPosLikeaPause(true);
    }

    public void OnCLickCancelAgain() => closeAgainUI();

    public void OnClickGoToSceneZero()
    {
        closeAgainUI(true);
        IGame.Instance.gameAPI.SaveUpdater();

        SceneLoader.TryChangeLevel(LevelChangeObserver.allScenes.regionSCene);
        AudioManager.instance.Play("ButtonClick");
    }


    private void OnClickAgainRegen()
    {
        SceneLoader.UpdateCurrentLevel();
        closeAgainUI(true);
        AudioManager.instance.Play("ButtonClick");
    }

    public void setEnergyCharger(string c)
    {
        energyCharger.text = c;
    }

}
