using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenOrientationChecker : MonoBehaviour
{
    [SerializeField] private Canvas _canvas;
    public static event Action OnPortrait;
    public static event Action OnLandscape;
    
    private int _lastHeight;
    private int _lastWidth;
    
    void Start()
    {
        CheckOrientation();
        
        _lastHeight = Screen.height;
        _lastWidth = Screen.width;
    }

    void Update()
    {
        if (_lastHeight != Screen.height || _lastWidth != Screen.width)
        {
            CheckOrientation();
            _lastHeight = Screen.height;
            _lastWidth = Screen.width;
        }
    }

    private void CheckOrientation()
    {
        if (Screen.height > Screen.width)
        {
            Debug.Log("Экран в вертикальной ориентации (по размеру)");
            _canvas.gameObject.SetActive(true);
            OnPortrait?.Invoke();
        }
        else
        {
            Debug.Log("Экран в горизонтальной ориентации (по размеру)");
            _canvas.gameObject.SetActive(false);
            OnLandscape?.Invoke();
        }
    }
}
