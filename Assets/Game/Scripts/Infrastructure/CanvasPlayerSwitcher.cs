using System.Collections;
using System.Collections.Generic;
using UI;
using UnityEngine;

public class CanvasPlayerSwitcher : MonoBehaviour
{
    [SerializeField] private Canvas _canvasMobile;
    [SerializeField] private Canvas _canvasDesktop;

    [SerializeField] private WeaponPanelUI _weaponPanelUIModile;
    [SerializeField] private WeaponPanelUI _weaponPanelUIDesktop;

    private Canvas _canvas;
    private WeaponPanelUI _weaponPanelUI;
    
    public void SetCanvasChange()
    {
        bool isMobile = DeviceChecker.IsMobileDevice();

        _canvas = isMobile ? _canvasMobile : _canvasDesktop;
        _canvas.gameObject.SetActive(true);

        _weaponPanelUI = isMobile ? _weaponPanelUIModile : _weaponPanelUIDesktop;
        _weaponPanelUI.gameObject.SetActive(true); 
    }
    
}
