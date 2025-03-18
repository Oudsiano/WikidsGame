using UnityEngine;
using UnityEngine.UI;

namespace Core.Player.MovingBetweenPoints
{
    public class Map : MonoBehaviour
    {
        [SerializeField] private Point[] _points;
        [SerializeField] private PointView[] _pointsView;
        [SerializeField] private Image _image;
        
        private PointClickHandler _handler;
        
        public void Construct(PointClickHandler handler)
        {
            _handler = handler;
            
            if (_points.Length != _pointsView.Length)
            {
                Debug.LogError("Points and PointViews count mismatch!");
                return;
            }

            for (int i = 0; i < _points.Length; i++)
            {
                _pointsView[i].Construct(_handler, _points[i].transform);
            }
        }

        public PointView[] GetPointsView => _pointsView;
        public Image Image => _image;
    }
}