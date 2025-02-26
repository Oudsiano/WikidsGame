using AINavigation;
using SceneManagement;
using UnityEngine;
using UnityEngine.Serialization;

namespace Core.PickableItems
{
    public class PickableHPBottle : MonoBehaviour // TODO Duplicate calss witch PickableCoin
    {
        [FormerlySerializedAs("countHPRestore")] [SerializeField] private float _countHPRestore;
        [FormerlySerializedAs("pickUpVFX")] [SerializeField] private GameObject _pickUpVFX; // Add this line to include a VFX prefab

        private UnityEngine.Camera _camera;
        private CursorManager _cursorManager;
        private PlayerController _playerController;
        
        public void Construct(float count, CursorManager cursorManager, PlayerController playerController)
        {
            _cursorManager = cursorManager;
            _playerController = playerController;
            _camera = UnityEngine.Camera.main;
            _countHPRestore = count;
            //textCount.text = count.ToString(); // TODO not used code
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0)) 
            {
                Ray ray = _camera.ScreenPointToRay(Input.mousePosition);// TODO can be cached
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.collider.gameObject == gameObject)
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
