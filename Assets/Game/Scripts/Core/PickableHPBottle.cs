using System.Collections;
using AINavigation;
using UnityEngine;

public class PickableHPBottle : MonoBehaviour
{
    [SerializeField] private float countHPRestore;
    [SerializeField] private GameObject pickUpVFX; // Add this line to include a VFX prefab

    public void Init(float c)
    {
        countHPRestore = c;
        //textCount.text = count.ToString();
    }

    private void Update()
    {
        // Проверяем, был ли клик по объекту
        if (Input.GetMouseButtonDown(0)) // 0 - левая кнопка мыши
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
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
    void OnMouseEnter()
    {
        IGame.Instance.CursorManager.SetCursorPickUp();
    }
    private void OnMouseExit()
    {
        IGame.Instance.CursorManager.SetCursorDefault();
    }

    // Method called when interacting with the collider
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerController>())
        {
            //HandleClick();
        }
    }

    private void HandleClick()
    {
        IGame.Instance.playerController.GetHealth().Heal(countHPRestore);

        // Instantiate the VFX at the bottle's position
        if (pickUpVFX != null)
        {
            Instantiate(pickUpVFX, transform.position, Quaternion.identity);
        }

        Destroy(gameObject);
        IGame.Instance.CursorManager.SetCursorDefault();
    }

}
