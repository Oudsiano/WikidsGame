using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class pauseClass
{
    private static bool isOpenUI;
    private static bool isDialog;

    public static bool IsOpenUI
    {
        get
        {
            return isOpenUI;
        }
        set
        {
            isOpenUI = value;
        }
    }
    public static bool IsDialog { get => isDialog; set => isDialog = value; }

    public static bool GetPauseState()
    {
        return isOpenUI || isDialog;
    }
}
