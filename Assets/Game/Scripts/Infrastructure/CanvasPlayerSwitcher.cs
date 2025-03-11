using System.Collections;
using System.Collections.Generic;
using AINavigation;
using Data;
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
    
    public void ChangePlayerCanvases()
    {
        bool isMobile = DeviceChecker.IsMobileDevice();

        _canvas = isMobile ? _canvasMobile : _canvasDesktop;
        _canvas.gameObject.SetActive(true);
        
        _weaponPanelUI = isMobile ? _weaponPanelUIModile : _weaponPanelUIDesktop;
        _weaponPanelUI.gameObject.SetActive(true); 
    }
    
    public WeaponPanelUI GetWeaponPanelUI()
    {
        if (_weaponPanelUI == null)
        {
            Debug.LogError(("weaponPanel is null"));
            return null;
        }

        return _weaponPanelUI;
    }
}
