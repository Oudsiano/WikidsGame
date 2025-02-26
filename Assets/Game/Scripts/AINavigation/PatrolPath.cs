using UnityEngine;

namespace AINavigation
{
    public class PatrolPath : MonoBehaviour
    {
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.white;

            for (int i = 0; i <= transform.childCount - 1; i++)
            {
                Transform firstWaypoint = transform.GetChild(i);
                
                if (i + 1 >= transform.childCount) // TODO magic number
                {
                    Gizmos.DrawLine(firstWaypoint.position, transform.GetChild(0).position);
                    
                    break;
                }
                
                Transform secondWayPoint = transform.GetChild(i + 1); // TODO magic number
                Gizmos.DrawLine(firstWaypoint.position, secondWayPoint.position);
            }
        }
    }
}
