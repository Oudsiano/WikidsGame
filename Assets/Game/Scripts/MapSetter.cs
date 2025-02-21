using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class ImageSetter : MonoBehaviour
{
    [FormerlySerializedAs("imageComponent")] [SerializeField]
    private RawImage _imageComponent; // Ссылка на компонент Image в вашем объекте

    //public Texture2D[] imageArray; // Массив изображений, которые вы хотите использовать
    [FormerlySerializedAs("renderTexturesArray")] [SerializeField]
    private RenderTexture _renderTexturesArray;

    private void Start() // TODO construct
    {
        if (_imageComponent == null)
        {
            _imageComponent = gameObject.GetComponent<RawImage>();

            if (_imageComponent == null)
            {
                Debug.LogError("Image component is not assigned!");

                return;
            }
        }

        // Добавляем метод UpdateImage как слушатель события перехода на новую сцену
        SceneManager.sceneLoaded += UpdateImage;

        // Вызываем UpdateImage для установки изображения на текущей сцене
        UpdateImage(SceneManager.GetActiveScene(), LoadSceneMode.Single);
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= UpdateImage;
    }

    private void UpdateImage(Scene scene, LoadSceneMode mode)
    {
        int sceneIndex = scene.buildIndex;

        // Проверяем, существует ли изображение для текущей сцены // TODO idk
        // Устанавливаем изображение из массива, соответствующее номеру сцены
        _imageComponent.texture = _renderTexturesArray;
    }
}