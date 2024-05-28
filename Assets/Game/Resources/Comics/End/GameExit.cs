using UnityEngine;

public class GameExit : MonoBehaviour
{
    // Этот метод можно привязать к кнопке выхода
    public void ExitGame()
    {
#if UNITY_EDITOR
        // Для остановки игры в редакторе Unity
        UnityEditor.EditorApplication.isPlaying = false;
#else
        // Для завершения игры при сборке
        Application.Quit();
#endif
    }
}
