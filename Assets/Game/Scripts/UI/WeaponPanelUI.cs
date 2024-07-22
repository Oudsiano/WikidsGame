using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponPanelUI : MonoBehaviour
{


    [SerializeField] public Button CommonWeaponBTN;
    [SerializeField] public Button FireballBTN;


    [SerializeField] public GameObject CommonWeaponPanell;
    [SerializeField] public GameObject FireballPanell;

    [SerializeField] private TMPro.TextMeshProUGUI FireballText;

    public void Init()
    {
        CommonWeaponBTN.onClick.AddListener(OnCLickCommonWeaponBTN);
        FireballBTN.onClick.AddListener(OnCLickFireballBTN);

        ResetWeaponToDefault();
        ResetFireballCount();
    }

    private void OnCLickFireballBTN()
    {
        IGame.Instance.playerController.GetFighter().SetFireball();
        FireballPanell.SetActive(true);
        CommonWeaponPanell.SetActive(false);
    }

    private void OnCLickCommonWeaponBTN() => ResetWeaponToDefault();
    

    public void ResetWeaponToDefault()
    {

        IGame.Instance.playerController.GetFighter().SetCommonWeapon();
        FireballPanell.SetActive(false);
        CommonWeaponPanell.SetActive(true);
    }
    public void ResetFireballCount()
    {
        if (IGame.Instance.dataPlayer.playerData.chargeEnergy > 0)
        {
            FireballText.text = "Fireball";// (" + IGame.Instance.dataPLayer.playerData.chargeEnergy.ToString() + ")";
            FireballBTN.gameObject.SetActive(true);
        }
        else
        {
            ResetWeaponToDefault();
            FireballBTN.gameObject.SetActive(false);
        }
    }


}
