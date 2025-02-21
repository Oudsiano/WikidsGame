using UnityEngine;
using UnityEngine.Serialization;

namespace SceneManagement
{
    public class CursorManager : MonoBehaviour
    {
        [FormerlySerializedAs("cursorTextureSword")] [SerializeField]
        private Texture2D CursorTextureSword;

        [FormerlySerializedAs("cursorTextureManuscript")] [SerializeField]
        private Texture2D _cursorTextureManuscript;

        [FormerlySerializedAs("cursorTextureSave")] [SerializeField]
        private Texture2D _cursorTextureSave;

        [FormerlySerializedAs("cursorTextureExit")] [SerializeField]
        private Texture2D _cursorTextureExit;

        [FormerlySerializedAs("cursorTexturePickUp")] [SerializeField]
        private Texture2D _cursorTexturePickUp;

        public void SetCursorDefault()
        {
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto); // TODO Duplicate
        }

        public void SetCursorManuscript()
        {
            Cursor.SetCursor(_cursorTextureManuscript, Vector2.zero, CursorMode.Auto); // TODO Duplicate
        }

        public void SetCursorSword()
        {
            Cursor.SetCursor(CursorTextureSword, Vector2.zero, CursorMode.Auto); // TODO Duplicate
        }

        public void SetCursorSave()
        {
            Cursor.SetCursor(_cursorTextureSave, Vector2.zero, CursorMode.Auto); // TODO Duplicate
        }

        public void SetCursorExit()
        {
            Cursor.SetCursor(_cursorTextureExit, Vector2.zero, CursorMode.Auto); // TODO Duplicate
        }

        public void SetCursorPickUp()
        {
            Cursor.SetCursor(_cursorTexturePickUp, Vector2.zero, CursorMode.Auto); // TODO Duplicate
        }
    }
}