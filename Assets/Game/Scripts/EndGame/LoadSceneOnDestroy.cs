using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneOnDestroy : MonoBehaviour
{
    private void OnDestroy()
    {
        // Проверка на то, что игра не завершена
        if (!Application.isPlaying) return;

        // Загружаем сцену номер 8
        SceneManager.LoadScene(8);
    }
}
