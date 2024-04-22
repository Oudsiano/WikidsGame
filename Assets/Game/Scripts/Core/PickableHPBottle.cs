using System.Collections;
using RPG.Combat;
using RPG.Controller;
using UnityEngine;

public class PickableHPBottle : MonoBehaviour
{
    [SerializeField] private float countHPRestore; 
    //[SerializeField] private TMPro.TextMeshPro textCount;

    public void Init(float c)
    {
        countHPRestore = c;
        //textCount.text = count.ToString();
    }

    // Метод, вызываемый при взаимодействии с коллайдером
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerController>())
        {
            IGame.Instance.playerController.GetHealth().Heal(countHPRestore);
            Destroy(gameObject);
        }
    }

}
