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

        private bool _isTriggered = false;

        private void Start() // TODO construct
        {
            if (IGame.Instance != null)
            {
                IGame.Instance.ArrowForPlayerManager.AllArrowForPlayers[Index] = this;
            }

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
                if (IGame.Instance.ArrowForPlayerManager.AllArrowForPlayers.ContainsKey(i))
                    if (IGame.Instance.ArrowForPlayerManager.AllArrowForPlayers[i].Index <= Index)
                    {
                        //if (IGame.Instance.ArrowForPlayerManager.AllArrowForPlayers.ContainsKey(IGame.Instance.ArrowForPlayerManager.AllArrowForPlayers[i].Index))
                        { // TODO not used code
                            IGame.Instance.ArrowForPlayerManager.AllArrowForPlayers[i].gameObject.SetActive(false);
                            IGame.Instance.ArrowForPlayerManager.AllArrowForPlayers.Remove(i);
                        }
                    }
            }

            IGame.Instance.ArrowForPlayerManager.StartArrow();
        }

        private void Update()
        {
            if (_isTriggered == false)
            {
                Vector3 rotate = transform.eulerAngles;
                Vector3 position = (transform.position - IGame.Instance.playerController.transform.position).normalized;
                float yAngle = Mathf.Acos(position.z) * Mathf.Rad2Deg;
                
                if (position.x < 0)
                {
                    yAngle = -yAngle;
                }

                rotate.x = 90; // TODO magic number
                rotate.y = yAngle;
                ArrowSprite.transform.rotation = Quaternion.Euler(rotate);

                ArrowSprite.transform.position = IGame.Instance.playerController.transform.position +
                                                 new Vector3(0, 1, 0) + position * 3;  // TODO magic number
            }
        }

        private void OnDestroy()
        {
            Destroy(ArrowSprite);
            Destroy(_arrowImage);
        }
    }
}