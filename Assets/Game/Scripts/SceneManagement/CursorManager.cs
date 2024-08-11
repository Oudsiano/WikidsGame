using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorManager : MonoBehaviour
{

    public Texture2D cursorTextureSword;
    public Texture2D cursorTextureManuscript;
    public Texture2D cursorTextureSave;
    public Texture2D cursorTextureExit;
    public Texture2D cursorTexturePickUp;

    public void SetCursorDefault()
    {
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }
    public void SetCursorManuscript()
    {
        Cursor.SetCursor(cursorTextureManuscript, Vector2.zero, CursorMode.Auto);
    }
    public void SetCursorSword()
    {
        Cursor.SetCursor(cursorTextureSword, Vector2.zero, CursorMode.Auto);
    }
    public void SetCursorSave()
    {
        Cursor.SetCursor(cursorTextureSave, Vector2.zero, CursorMode.Auto);
    }
    public void SetCursorExit()
    {
        Cursor.SetCursor(cursorTextureExit, Vector2.zero, CursorMode.Auto);
    }
    public void SetCursorPickUp()
    {
        Cursor.SetCursor(cursorTexturePickUp, Vector2.zero, CursorMode.Auto);
    }
}
