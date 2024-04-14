using System.Collections;
using RPG.Combat;
using RPG.Controller;
using UnityEngine;

public class PickableCoin : MonoBehaviour
{
    [SerializeField] private float count; // Оружие, которое можно подобрать
    [SerializeField] private TMPro.TextMeshPro textCount; // Оружие, которое можно подобрать

    public void Init(float c)
    {
        count = c;
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
