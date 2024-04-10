using UnityEngine;
using UnityEngine.UI;

public class ImageSetter : MonoBehaviour
{
    public Image imageComponent; // Ссылка на компонент Image в вашем объекте

    public Sprite[] imageArray; // Массив изображений, которые вы хотите использовать

    public int imageIndex; // Индекс изображения в массиве, которое вы хотите установить

    void Start()
    {
        // Проверка наличия компонента Image
        if (imageComponent == null)
        {
            Debug.LogError("Image component is not assigned!");
            return;
        }

        // Проверка наличия массива изображений
        if (imageArray == null || imageArray.Length == 0)
        {
            Debug.LogError("Image array is empty or not assigned!");
            return;
        }

        // Проверка корректности индекса изображения
        if (imageIndex < 0 || imageIndex >= imageArray.Length)
        {
            Debug.LogError("Invalid image index!");
            return;
        }

        // Установка изображения из массива по индексу
        imageComponent.sprite = imageArray[imageIndex];
    }
}
