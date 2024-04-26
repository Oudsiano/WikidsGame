using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUIManager : MonoBehaviour
{

    [SerializeField] public Button ButtonShowMap;
    [SerializeField] public GameObject MapCanvas;

    [SerializeField] private GameObject newWeaponScr;
    [SerializeField] private GameObject newArmorScr;

    public void ShowNewArmor() => newArmorScr.SetActive(true);
    public void ShowNewWeapon() => newWeaponScr.SetActive(true);


    public void Init()
    {
        ButtonShowMap.onClick.AddListener(OnClickButtonMap);
    }

    private void OnClickButtonMap()
    {
        if (!MapCanvas.gameObject.activeSelf) MapCanvas.gameObject.SetActive(true);
    }

}
