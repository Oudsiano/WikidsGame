using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace Core.PickableItems
{
    public class PickableCoin : MonoBehaviour
    {
        [FormerlySerializedAs("count")] [SerializeField] private float _count;
        [FormerlySerializedAs("textCount")] [SerializeField] private TextMeshPro _textCount; // TODO change to VIEW

        public void Init(float count) // TODO Construct
        {
            _count = count;

            if (_textCount != null)
            {
                _textCount.text = count.ToString();
            }
        }

        // Метод, вызываемый при взаимодействии с коллайдером
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
            IGame.Instance.CursorManager.SetCursorPickUp();
        }

        private void OnMouseExit()
        {
            IGame.Instance.CursorManager.SetCursorDefault();
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0)) 
            {
                Ray ray = UnityEngine.Camera.main.ScreenPointToRay(Input.mousePosition); // TODO can be cached
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.collider.gameObject == gameObject || hit.collider.gameObject.name == "SM_Item_Coins_01") // TODO can be cached 
                    {
                        IGame.Instance.saveGame.Coins += _count;
                        Destroy(gameObject);
                        IGame.Instance.CursorManager.SetCursorDefault();
                    }
                }
            }
        }
    }
}