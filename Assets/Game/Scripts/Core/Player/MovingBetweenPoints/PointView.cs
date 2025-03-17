using UnityEngine;
using UnityEngine.UI;

namespace Core.Player.MovingBetweenPoints
{
    public class PointView : MonoBehaviour
    {
        [SerializeField] private Button _button;

        private PointClickHandler _handler;
        private Transform _transform;
        
        public void Construct(PointClickHandler handler, Transform transform)
        {
            _handler = handler;
            _transform = transform;
        }

        public Transform TransformPoint => _transform;
        
        private void OnDestroy()
        {
            _button.onClick.RemoveListener(ClickPoint);
        }
        
        private void ClickPoint()
        {
            _handler.HandleClick(_transform);
        }
    }
}