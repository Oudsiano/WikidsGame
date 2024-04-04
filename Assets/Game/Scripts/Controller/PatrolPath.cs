using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Controller
{
    public class PatrolPath : MonoBehaviour
    {
        // Функция Unity, автоматически вызываемая в редакторе для рисования gizmos
        private void OnDrawGizmos()
        {
            // Устанавливаем цвет gizmos в белый
            Gizmos.color = Color.white;

            // Цикл по всем дочерним точкам пути этого объекта
            for (int i = 0; i <= transform.childCount - 1; i++)
            {
                // Получаем Transform текущей точки пути
                Transform firstWaypoint = transform.GetChild(i);

                // Если это последняя точка пути, соединяем ее с первой, чтобы замкнуть цикл
                if (i + 1 >= transform.childCount)
                {
                    Gizmos.DrawLine(firstWaypoint.position, transform.GetChild(0).position);
                    break;
                }

                // В противном случае получаем Transform следующей точки пути
                Transform secondWayPoint = transform.GetChild(i + 1);

                // Рисуем линию, соединяющую текущую и следующую точки пути
                Gizmos.DrawLine(firstWaypoint.position, secondWayPoint.position);
            }
        }
    }
}
