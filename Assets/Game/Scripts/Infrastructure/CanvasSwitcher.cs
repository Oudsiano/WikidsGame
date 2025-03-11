using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasSwitcher : MonoBehaviour
{
    [SerializeField] private Canvas _canvasDesktop;
    [SerializeField] private Canvas _canvasMobile;

    private Canvas _mapSceneCanvas;
    
    public void SetCanvasChange(bool isMobile)
    {
        _mapSceneCanvas = isMobile ? _canvasMobile : _canvasDesktop;
        _mapSceneCanvas.gameObject.SetActive(true);
    }
}
