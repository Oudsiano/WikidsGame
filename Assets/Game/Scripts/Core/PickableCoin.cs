using System.Collections;
using RPG.Combat;
using RPG.Controller;
using UnityEngine;

public class PickableCoin : MonoBehaviour
{
    [SerializeField] private float count; 
    [SerializeField] private TMPro.TextMeshPro textCount;

    public void Init(float c)
    {
        count = c;
        if (textCount!=null)
        textCount.text = count.ToString();
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
    void OnMouseEnter()
    {
        IGame.Instance.CursorManager.SetCursorPickUp();
    }
    private void OnMouseExit()
    {
        IGame.Instance.CursorManager.SetCursorDefault();
    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(0)) // 0 - левая кнопка мыши
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if ((hit.collider.gameObject == gameObject) || (hit.collider.gameObject.name == "SM_Item_Coins_01"))
                {
                    IGame.Instance.saveGame.Coins += count;
                    Destroy(gameObject);
                }
            }
        }
    }

}
