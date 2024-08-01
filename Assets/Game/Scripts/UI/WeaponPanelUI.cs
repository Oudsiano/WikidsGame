using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponPanelUI : MonoBehaviour
{
    [SerializeField] public Button CommonWeaponBTN;
    [SerializeField] public Button FireballBTN;
    [SerializeField] public Button BowBTN;  // ????? ?????? ??? ????

    [SerializeField] public GameObject CommonWeaponPanell;
    [SerializeField] public GameObject FireballPanell;
    [SerializeField] public GameObject BowPanell;  // ????? ?????? ??? ????

    [SerializeField] private TMPro.TextMeshProUGUI FireballText;

    public void Init()
    {
        CommonWeaponBTN.onClick.AddListener(OnCLickCommonWeaponBTN);
        FireballBTN.onClick.AddListener(OnCLickFireballBTN);
        BowBTN.onClick.AddListener(OnClickBowBTN);  // ????????? ?????????? ??? ?????? ????

        ResetWeaponToDefault();
        ResetFireballCount();
    }

    private void OnCLickFireballBTN()
    {
        IGame.Instance.playerController.GetFighter().SetFireball();
        FireballPanell.SetActive(true);
        CommonWeaponPanell.SetActive(false);
        BowPanell.SetActive(false);  // ???????? ?????? ????
    }

    private void OnCLickCommonWeaponBTN() => ResetWeaponToDefault();

    private void OnClickBowBTN()
    {
        IGame.Instance.playerController.GetFighter().SetBow();  // ??????????????, ??? ? ??? ???? ????? SetBow() ? ????? Fighter ??????
        BowPanell.SetActive(true);
        CommonWeaponPanell.SetActive(false);
        FireballPanell.SetActive(false);  // ???????? ?????? ????????? ????
    }

    public void ResetWeaponToDefault()
    {
        IGame.Instance.playerController.GetFighter().SetCommonWeapon();
        FireballPanell.SetActive(false);
        CommonWeaponPanell.SetActive(true);
        BowPanell.SetActive(false);  // ???????? ?????? ????
    }

    public void ResetFireballCount()
    {
        if (IGame.Instance.dataPlayer.playerData.chargeEnergy > 0)
        {
            FireballText.text = "Fireball";  // (" + IGame.Instance.dataPLayer.playerData.chargeEnergy.ToString() + ")";
            FireballBTN.gameObject.SetActive(true);
        }
        else
        {
            ResetWeaponToDefault();
            FireballBTN.gameObject.SetActive(false);
        }
    }
}
