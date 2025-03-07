using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class DeviceChecker : MonoBehaviour
{
    [DllImport("__Internal")]
    private static extern bool IsMobile();

    public static bool IsMobileDevice()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
            return IsMobile();
#else
        return false; 
#endif
    }
}
