using AINavigation;
using UnityEngine;
using UnityEngine.Serialization;

namespace Core.Player
{
    public class ArrowForPlayer : MonoBehaviour
    {
        [SerializeField] public GameObject ArrowSprite; // TODO GO
        public int Index;

        [FormerlySerializedAs("ArrowImage")] [SerializeField]
        private GameObject _arrowImage;

        private ArrowForPlayerManager _arrowManager;
        private PlayerController _playerController;
        private bool _isTriggered = false;

        public void Construct(ArrowForPlayerManager arrowManager, PlayerController playerController)
        {
            _arrowManager = arrowManager;
            _playerController = playerController;
            _arrowManager.AllArrowForPlayers[Index] = this;

            if (Index != 0)
            {
                ArrowSprite.SetActive(false);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            _isTriggered = true;

            for (int i = Index; i >= 0; i--)
            {
                if (_arrowManager.AllArrowForPlayers.ContainsKey(i))
                    if (_arrowManager.AllArrowForPlayers[i].Index <= Index)
                    {
                        //if (_arrowManager.AllArrowForPlayers.ContainsKey(_arrowManager.AllArrowForPlayers[i].Index))
                        {
                            // TODO not used code
                            _arrowManager.AllArrowForPlayers[i].gameObject.SetActive(false);
                            _arrowManager.AllArrowForPlayers.Remove(i);
                        }
                    }
            }

            _arrowManager.StartArrow();
        }

        private void Update()
        {
            if (_isTriggered == false)
            {
                Vector3 rotate = transform.eulerAngles;
                Vector3 position = (transform.position - _playerController.transform.position).normalized;
                float yAngle = Mathf.Acos(position.z) * Mathf.Rad2Deg;

                if (position.x < 0)
                {
                    yAngle = -yAngle;
                }

                rotate.x = 90; // TODO magic number
                rotate.y = yAngle;
                ArrowSprite.transform.rotation = Quaternion.Euler(rotate);

                ArrowSprite.transform.position = _playerController.transform.position +
                                                 new Vector3(0, 1, 0) + position * 3; // TODO magic number
            }
        }

        private void OnDestroy()
        {
            Destroy(ArrowSprite);
            Destroy(_arrowImage);
        }
    }
}