using UnityEngine;
using UnityEngine.SceneManagement;

namespace EndGame
{
    public class LoadSceneOnDestroy : MonoBehaviour
    {
        private void OnDestroy()
        {
            // Проверка на то, что игра не завершена
            if (Application.isPlaying == false)
            {
                return;
            }

            // Загружаем сцену номер 8
            SceneManager.LoadScene(8); // TODO magic number
        }
    }
}