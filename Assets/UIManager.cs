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


    public SceneLoader sceneLoader;
    void Start()
    {
        sceneLoader = FindObjectOfType<SceneLoader>();

        button1Goto1.onClick.AddListener(() => Click(1));
        button1Goto2.onClick.AddListener(() => Click(2));
        button1Goto3.onClick.AddListener(() => Click(3));
        button1Goto4.onClick.AddListener(() => Click(4));
        button1Goto5.onClick.AddListener(() => Click(5));
        button1Goto6.onClick.AddListener(() => Click(6));
    }

    private void Click(int i) => sceneLoader.LoadScene(i);


    // Update is called once per frame
    void Update()
    {

    }
}
