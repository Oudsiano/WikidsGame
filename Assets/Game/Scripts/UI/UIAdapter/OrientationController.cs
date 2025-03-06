using System;
using UnityEngine;

[ExecuteAlways] // выполняем скрипт и в Play, и в Edit моде
[RequireComponent(typeof(RectTransform))] // нужен RectTransform на GameObject
public sealed class OrientationController : MonoBehaviour
{
    // Хранилище для вертикальной ориентации
    [SerializeField] private SavedRect _verticalRect = new SavedRect();
    // Хранилище для горизонтальной ориентации
    [SerializeField] private SavedRect _horizontalRect = new SavedRect();
		
    // Закэшированный RectTransform
    private RectTransform _rect;

    private void Awake()
    {
        _rect = GetComponent<RectTransform>();
        // Подписываемся на событие
        OrientationChanged += OnOrientationChanged;
        // Проводим инициализационное принудительное обновление
        OnOrientationChanged(this, isVertical);
    }

    public void SaveCurrentState()
    {
        if (isVertical)
            _verticalRect.SaveDataFromRectTransform(_rect);
        else
            _horizontalRect.SaveDataFromRectTransform(_rect);
    }

    public void PutCurrentState()
    {
        OnOrientationChanged(this, isVertical);
    }

    private void OnOrientationChanged(object sender, bool isVertical)
    {
        if (isVertical)
            _verticalRect.PutDataToRectTransform(_rect);
        else
            _horizontalRect.PutDataToRectTransform(_rect);
    }

    private void OnDestroy()
    {
        // отписываемся от события
        OrientationChanged -= OnOrientationChanged;
    }


    // Static
    public static bool isVertical;
    // событие смены ориентации
    private static event EventHandler<bool> OrientationChanged;
    // метод для вызова события извне
    public static void FireOrientationChanged(object s, bool isVertical) => OrientationChanged?.Invoke(s, isVertical);
		
    /// статический конструктор нестатического класса вызывается 1 раз
    /// до первой инициализации объекта этого типа
    static OrientationController()
    {
        // обновляем isVertical при срабатывании события
        OrientationChanged += (s, e) => isVertical = e;
    }
}
