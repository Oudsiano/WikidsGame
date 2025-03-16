using UnityEngine;

namespace Core.Player.MovingBetweenPoints
{
    public class Map : MonoBehaviour
    {
        [SerializeField] private Point[] _points;
        [SerializeField] private PointView[] _pointsView;
        
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
    }
}