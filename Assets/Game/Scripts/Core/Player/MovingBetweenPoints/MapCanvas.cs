using UnityEngine;
using UnityEngine.UI;

namespace Core.Player.MovingBetweenPoints
{
    public class MapCanvas : MonoBehaviour
    {
        [SerializeField] private Image _image;

        public void Construct(Image image)
        {
            _image.sprite = image.sprite;
        }
    }
}