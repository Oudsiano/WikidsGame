using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasSwitcher : MonoBehaviour
{
    [SerializeField] private Canvas _canvasDesktop;
    [SerializeField] private Canvas _canvasMobile;

    private Canvas _mapSceneCanvas;

    private void OnEnable()
    {
        ScreenOrientationChecker.OnPortrait += HideUI;
        ScreenOrientationChecker.OnLandscape +=ShowUI;
    }

    private void OnDisable()
    {
        ScreenOrientationChecker.OnPortrait -= HideUI;
        ScreenOrientationChecker.OnLandscape -=ShowUI;
    }
    
    private void ShowUI()
    {
        _canvasDesktop.gameObject.SetActive(true);
    }

    private void HideUI()
    {
        _canvasDesktop.gameObject.SetActive(false);
    }
}
