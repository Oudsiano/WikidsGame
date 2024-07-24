using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorManager : MonoBehaviour
{

    public Texture2D cursorTextureSword;
    //public Texture2D cursorTextureFinger;
    public Texture2D cursorTextureManuscript;

    public void SetCursorManuscript()
    {
        Cursor.SetCursor(cursorTextureManuscript, Vector2.zero, CursorMode.Auto);
    }
    public void SetCursorFinger()
    {
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }
    public void SetCursorSword()
    {
        Cursor.SetCursor(cursorTextureSword, Vector2.zero, CursorMode.Auto);
    }
}
