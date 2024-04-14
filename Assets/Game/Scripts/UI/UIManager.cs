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

    [SerializeField] private GameObject _againUI;
    [SerializeField] private Button _buttonAgain;
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

        _buttonAgain.onClick.AddListener(OnClickAgain);
    }

    private void Click(int i) => sceneLoader.LoadScene(i);

    public void ShowAgainUi() => _againUI.SetActive(true);


    private void OnClickAgain()
    {
        var dataPlayer = FindObjectOfType<DataPlayer>();
        sceneLoader.LoadScene(dataPlayer.playerData.sceneToLoad);
        _againUI.SetActive(false);
    }

    public void setCoinCount(string c)
    {
        textCoin.text = "монет: " +c;
    }


}
