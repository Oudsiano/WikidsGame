using System.Collections;
using RPG.Combat;
using RPG.Controller;
using UnityEngine;

public class PickableArmor : MonoBehaviour
{


    [SerializeField] private Armor armor;
    [SerializeField] private float respawnTime = 5f; // Время до появления оружия снова

    // Метод, вызываемый при взаимодействии с коллайдером
    private void OnTriggerEnter(Collider other)
    {
        // Получаем компонент Fighter у объекта, который столкнулся с пикапом
        Fighter fighter = other.GetComponent<Fighter>();

        // Проверяем, является ли столкнувшийся объект игроком
        // (PlayerController используется для управления игроком)
        var PlayerController = other.GetComponent<PlayerController>();
        if (fighter && PlayerController)
        {
            //IGame.Instance.dataPLayer.
            fighter.EquipArmor(armor);

            // Запускаем корутину для скрытия пикапа на некоторое время
            StartCoroutine(HideForSeconds(respawnTime));
        }
    }

    // Корутина для временного скрытия пикапа
    private IEnumerator HideForSeconds(float seconds)
    {
        // Скрываем пикап
        ShowPickup(false);

        // Ждем указанное количество секунд
        yield return new WaitForSeconds(seconds);

        // Показываем пикап снова
        ShowPickup(true);
    }

    // Метод для скрытия или показа пикапа
    private void ShowPickup(bool shouldShow)
    {
        // Включаем или выключаем коллайдер сферы пикапа
        GetComponent<MeshCollider>().enabled = shouldShow;

        // Для каждого дочернего объекта пикапа включаем или выключаем его активность
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(shouldShow);
        }
    }
}
