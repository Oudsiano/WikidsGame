using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ImageSetter : MonoBehaviour
{
    public Image imageComponent; // Ссылка на компонент Image в вашем объекте
    public Sprite[] imageArray; // Массив изображений, которые вы хотите использовать

    void Start()
    {
        // Проверка наличия компонента Image
        if (imageComponent == null)
        {
            Debug.LogError("Image component is not assigned!");
            return;
        }

        // Добавляем метод UpdateImage как слушатель события перехода на новую сцену
        SceneManager.sceneLoaded += UpdateImage;

        // Вызываем UpdateImage для установки изображения на текущей сцене
        UpdateImage(SceneManager.GetActiveScene(), LoadSceneMode.Single);
    }

    // Метод для обновления изображения при переходе на новую сцену
    void UpdateImage(Scene scene, LoadSceneMode mode)
    {
        // Получаем номер текущей сцены
        int sceneIndex = scene.buildIndex;

        // Проверяем, существует ли изображение для текущей сцены
        if (sceneIndex >= 0 && sceneIndex < imageArray.Length)
        {
            // Устанавливаем изображение из массива, соответствующее номеру сцены
            imageComponent.sprite = imageArray[sceneIndex];
        }
        else
        {
            // Если изображение для текущей сцены не найдено, выводим сообщение об ошибке
            Debug.LogError("No image found for scene index: " + sceneIndex);
        }
    }

    // Отключаем слушатель события при уничтожении объекта
    void OnDestroy()
    {
        SceneManager.sceneLoaded -= UpdateImage;
    }
}
