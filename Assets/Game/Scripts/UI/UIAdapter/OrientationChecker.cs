using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways] // скрипт работает всегда
/// этот аттрибут нужен, чтобы быть уверенным, в том, что
/// этот компонент будет инициализирован до OrientationController
[DefaultExecutionOrder(-10)] 
public class OrientationChecker : MonoBehaviour
{
    private int _lastWidth;
    private int _lastHeight;
    private void Awake()
    {
        // принудительное инициализационной обновление
        HandleOrientation();
    }

    private void Start()
    {
        _lastWidth = Screen.width;
        _lastWidth = Screen.height;
    }

    private void Update()
    {
        HandleOrientation();
    }

    private void HandleOrientation()
    {
        /// Если последняя ориентация была вертикальной
        /// и ширина экрана больше высоты ...
        if (OrientationController.isVertical &&
            Screen.width > Screen.height)
        {
            // ... то вызываем событие, и передаем isVertical = false
            OrientationController.FireOrientationChanged(this, false);
        }
        else
            /// Иначе, если последняя ориентация была горизонтальной
            /// и ширина экрана меньше высоты ...
        if (!OrientationController.isVertical &&
            Screen.width < Screen.height)
        {
            // ... то вызываем событие, и передаем isVertical = true
            OrientationController.FireOrientationChanged(this, true);
        }
    }
}
