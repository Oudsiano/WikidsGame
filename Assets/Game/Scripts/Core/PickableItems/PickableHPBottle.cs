using AINavigation;
using SceneManagement;
using UnityEngine;
using UnityEngine.Serialization;

namespace Core.PickableItems
{
    public class PickableHPBottle : MonoBehaviour // TODO Duplicate calss witch PickableCoin
    {
        [FormerlySerializedAs("countHPRestore")] [SerializeField]
        private float _countHPRestore;

        [FormerlySerializedAs("pickUpVFX")] [SerializeField]
        private GameObject _pickUpVFX; // Add this line to include a VFX prefab

        private UnityEngine.Camera _camera;
        private CursorManager _cursorManager;
        private PlayerController _playerController;

        public void Construct(float count, CursorManager cursorManager, PlayerController playerController)
        {
            _cursorManager = cursorManager;
            _playerController = playerController;
            _camera = UnityEngine.Camera.main;
            _countHPRestore = count;
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = _camera.ScreenPointToRay(Input.mousePosition); // TODO can be cached

                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    if (hit.collider.gameObject.TryGetComponent(out PickableHPBottle _))
                    {
                        HandleClick();
                    }
                }
            }
        }

        private void OnMouseEnter()
        {
            _cursorManager.SetCursorPickUp();
        }

        private void OnMouseExit()
        {
            _cursorManager.SetCursorDefault();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.GetComponent<PlayerController>())
            {
                //HandleClick(); // TODO not used code
            }
        }

        private void HandleClick()
        {
            _playerController.GetHealth().Heal(_countHPRestore);

            if (_pickUpVFX != null)
            {
                Instantiate(_pickUpVFX, transform.position, Quaternion.identity);
            }

            Destroy(gameObject);
            _cursorManager.SetCursorDefault();
        }
    }
}