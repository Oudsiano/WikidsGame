using Saving;
using SceneManagement;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace Core.PickableItems
{
    public class PickableCoin : MonoBehaviour
    {
        [FormerlySerializedAs("count")] [SerializeField]
        private float _value;

        [FormerlySerializedAs("textCount")] [SerializeField]
        private TextMeshPro _textCount; // TODO change to VIEW

        private CursorManager _cursorManager;
        private SaveGame _saveGame;
        private UnityEngine.Camera _camera;

        public void Construct(float value, CursorManager cursorManager, SaveGame saveGame)
        {
            _camera = UnityEngine.Camera.main;
            _saveGame = saveGame;
            _cursorManager = cursorManager;
            _value = value;

            if (_textCount != null)
            {
                _textCount.text = value.ToString();
            }
        }

        // Метод, вызываемый при взаимодействии с коллайдером // TODO not used code
        /*private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerController>())
        {
            IGame.Instance.saveGame.Coins += count;
            Destroy(gameObject);
        }
    }*/
        private void OnMouseEnter()
        {
            _cursorManager.SetCursorPickUp();
        }

        private void OnMouseExit()
        {
            _cursorManager.SetCursorDefault();
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = _camera.ScreenPointToRay(Input.mousePosition); // TODO can be cached
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.collider.gameObject.TryGetComponent(out PickableCoin _)) // TODO can be cached 
                    {
                        _saveGame.Coins += _value;
                        Destroy(gameObject);
                        _cursorManager.SetCursorDefault();
                    }
                }
            }
        }
    }
}