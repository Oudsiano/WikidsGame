using UnityEngine;
using DG.Tweening;

public class RotationObjects : MonoBehaviour
{
    public Vector3 rotationAxis = Vector3.up; // Ось вращения (по умолчанию вокруг оси Y)
    public float rotationDuration = 1f; // Продолжительность анимации вращения
    public Ease easeType = Ease.Linear; // Тип анимации

    void Start()
    {
        // Начать вращение объекта при запуске скрипта
        Rotate();
    }

    void Rotate()
    {
        // Используем DoTween для вращения объекта вокруг его центра
        transform.DOLocalRotate(transform.localRotation.eulerAngles + rotationAxis * 360f, rotationDuration, RotateMode.FastBeyond360)
            .SetEase(easeType)
            .SetLoops(-1, LoopType.Restart); // Зацикливаем вращение бесконечно
    }
}


