using UnityEngine;

namespace AINavigation
{
    public class PatrolPath : MonoBehaviour
    {
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
                if (i + 1 >= transform.childCount) // TODO magic number
                {
                    Gizmos.DrawLine(firstWaypoint.position, transform.GetChild(0).position);
                    break;
                }

                // В противном случае получаем Transform следующей точки пути
                Transform secondWayPoint = transform.GetChild(i + 1); // TODO magic number

                // Рисуем линию, соединяющую текущую и следующую точки пути
                Gizmos.DrawLine(firstWaypoint.position, secondWayPoint.position);
            }
        }
    }
}
