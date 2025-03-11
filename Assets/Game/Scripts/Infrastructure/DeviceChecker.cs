using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

[ExecuteInEditMode] // -тестирование
public class DeviceChecker : MonoBehaviour
{
    [DllImport("__Internal")]
    private static extern bool IsMobile();
    
    private static bool _isMobileTesting =false; // -тестирование

    
    public static bool IsMobileDevice()
    {
      
        
#if UNITY_WEBGL && !UNITY_EDITOR
            return IsMobile();
#else
        if (_isMobileTesting)
        {
            return true;
            
        }
        else
        {
            return false;    // -тестирование
        }
#endif
    }
}
