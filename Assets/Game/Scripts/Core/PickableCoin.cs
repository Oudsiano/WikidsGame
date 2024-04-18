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
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerController>())
        {
            IGame.Instance.dataPLayer.playerData.coins += count;
            IGame.Instance.UIManager.setCoinCount(IGame.Instance.dataPLayer.playerData.coins.ToString());
            Destroy(gameObject);
        }
    }

}
