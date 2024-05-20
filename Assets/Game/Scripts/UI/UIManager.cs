using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RPG.Core;
using TMPro;
using FarrokhGames.Inventory;
using FarrokhGames.Inventory.Examples;

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


        IGame.Instance.CoinManager.Coins.OnChangeCount += OnChangeMoney;
    }

    private void OnDestroy()
    {
        IGame.Instance.CoinManager.Coins.OnChangeCount -= OnChangeMoney;
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
