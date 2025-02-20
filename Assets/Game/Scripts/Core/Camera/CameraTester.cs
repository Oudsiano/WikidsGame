
using UnityEngine;

namespace Core.Camera
{
    public class CameraTester : MonoBehaviour // TODO not used class
    {
        [SerializeField] private Transform _target;
        [SerializeField] private float _smoothSpeed;
        [SerializeField] private Vector3 _offSet;

        private void LateUpdate()
        {
            transform.position = _target.position + _offSet;
        }
    }
}