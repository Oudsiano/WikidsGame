using System.Collections;
using RPG.Combat;
using RPG.Controller;
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

    // Method called when interacting with the collider
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerController>())
        {
            IGame.Instance.playerController.GetHealth().Heal(countHPRestore);

            // Instantiate the VFX at the bottle's position
            if (pickUpVFX != null)
            {
                Instantiate(pickUpVFX, transform.position, Quaternion.identity);
            }

            Destroy(gameObject);
        }
    }
}
