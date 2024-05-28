using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTester : MonoBehaviour
{
    // Цель, за которой следует камера
    public Transform target;

    // Плавность следования камеры
    public float smoothSpeed;

    // Смещение камеры относительно цели
    public Vector3 offSet;

    void LateUpdate()
    {
        // Устанавливаем позицию камеры как смещенную относительно позиции цели
        transform.position = target.position + offSet;
    }
}
