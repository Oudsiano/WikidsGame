
using UnityEngine;
using UnityEngine.Serialization;

namespace Core.Camera
{
    public class CameraTester : MonoBehaviour // TODO not used class
    {
        [SerializeField] private Transform _target;
        [SerializeField] private float _smoothSpeed;
        [FormerlySerializedAs("_offSet")] [SerializeField] private Vector3 _offset;

        private void LateUpdate()
        {
            transform.position = _target.position + _offset;
        }
    }
}