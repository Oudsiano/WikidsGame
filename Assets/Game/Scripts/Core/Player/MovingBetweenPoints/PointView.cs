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
            _button.onClick.AddListener(ClickPoint);
        }

        private void OnDestroy()
        {
            _button.onClick.RemoveListener(ClickPoint);
        }
        
        private void ClickPoint()
        {
            if (transform.parent != null)
            {
                transform.parent.gameObject.SetActive(false);
            }
            
            _handler.HandleClick(_transform);
        }
    }
}