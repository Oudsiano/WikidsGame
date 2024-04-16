using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RPG.Core;
using TMPro;

public class UIManager : MonoBehaviour
{
    public Button button1Goto1;
    public Button button1Goto2;
    public Button button1Goto3;
    public Button button1Goto4;
    public Button button1Goto5;
    public Button button1Goto6;

    public DeathUI DeathUI;

    [Header("AgainUI")]
    [SerializeField] private GameObject _againUI;
    [SerializeField] private Button _buttonAgain;
    [SerializeField] private Button _buttonGoToSceneZero;
    [SerializeField] private Button _buttonCancelAgain;

    [Header("CoinUI")]

    [SerializeField] private TMPro.TextMeshProUGUI textCoin;



    public SceneLoader sceneLoader;
    public void Init()
    {
        sceneLoader = FindObjectOfType<SceneLoader>();

        button1Goto1.onClick.AddListener(() => Click(1));
        button1Goto2.onClick.AddListener(() => Click(2));
        button1Goto3.onClick.AddListener(() => Click(3));
        button1Goto4.onClick.AddListener(() => Click(4));
        button1Goto5.onClick.AddListener(() => Click(5));
        button1Goto6.onClick.AddListener(() => Click(6));

        _buttonAgain.onClick.AddListener(OnClickAgainRegen);
        _buttonGoToSceneZero.onClick.AddListener(OnClickGoToSceneZero);
        _buttonCancelAgain.onClick.AddListener(OnCLickCancelAgain);

        DeathUI.gameObject.SetActive(false);
        _againUI.SetActive(false);
    }

    private void Click(int i) => sceneLoader.LoadScene(i);

    public void ShowAgainUi()
    {
        _againUI.SetActive(true);
        KeyBoardsEvents.escState = KeyBoardsEvents.EscState.againScr;

        if (IGame.Instance.dataPLayer.playerData.health > 0)
            _buttonCancelAgain.gameObject.SetActive(true);
        else
            _buttonCancelAgain.gameObject.SetActive(false);
    }

    private void closeAgainUI()
    {
        if (IGame.Instance.dataPLayer.playerData.health > 0)
        {
            _againUI.SetActive(false);
            KeyBoardsEvents.escState = KeyBoardsEvents.EscState.none;
        }
    }

    public void OnCLickCancelAgain() => closeAgainUI();
    
    public void OnClickGoToSceneZero()
    {
        closeAgainUI();
        IGame.Instance.gameAPI.SaveUpdater();


        sceneLoader.LoadScene(0);
    }


    private void OnClickAgainRegen()
    {
        var dataPlayer = FindObjectOfType<DataPlayer>();
        sceneLoader.LoadScene(dataPlayer.playerData.sceneToLoad);
        closeAgainUI();
    }

    public void setCoinCount(string c)
    {
        textCoin.text = "монет: " + c;
    }


}
