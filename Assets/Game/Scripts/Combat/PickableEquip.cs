using System.Collections;
using FarrokhGames.Inventory.Examples;
using RPG.Combat;
using RPG.Controller;
using UnityEngine;

public class PickableEquip : MonoBehaviour
{
    [SerializeField] private ItemDefinition item;
    //[SerializeField] private float respawnTime = 20f; // Время до появления оружия снова

    // Метод, вызываемый при взаимодействии с коллайдером
    private void OnTriggerEnter(Collider other)
    {
        /*Fighter fighter = other.GetComponent<Fighter>();

        // Проверяем, является ли столкнувшийся объект игроком
        // (PlayerController используется для управления игроком)
        var PlayerController = other.GetComponent<PlayerController>();
        if (fighter && PlayerController)
        {
            //IGame.Instance.dataPLayer.
            PickUpIt();
            // Запускаем корутину для скрытия пикапа на некоторое время
        }*/
    }

    public void SetItem(ItemDefinition _item)
    {
        item = (ItemDefinition)_item.CreateInstance();
    }

    private void PickUpIt()
    {
        if (IGame.Instance.WeaponArmorManager.IsWeaponInGame(item.name))
        {

            if (item != null)
            {
                item.CreateInstance();
                IGame.Instance.UIManager.uIBug.TryAddEquipToBug(item);
            }
            else
                Debug.LogError("mistake item");
        }
        else
        {
            Debug.LogWarning("Этого предмета нет в списке предметов в WeaponArmorManager");
        }
        // Получаем компонент Fighter у объекта, который столкнулся с пикапом
        //IGame.Instance.playerController.GetFighter().EquipItem(item);

        //StartCoroutine(HideForSeconds(respawnTime));

        Destroy(gameObject);
    }

    private void Update()
    {
        // Получаем луч из мыши
        var ray = GetMouseRay();

        // Проводим луч в мире и получаем результаты
        RaycastHit[] hits;
        hits = Physics.RaycastAll(ray, Mathf.Infinity);

        foreach (var Hit in hits)
        {
            PickableEquip target = Hit.transform.gameObject.GetComponent<PickableEquip>();

            // Если цель не существует или игрок не может атаковать выбранного противника, продолжаем цикл
            if (!target)
                continue;

            // Если игрок кликнул мышью, атакуем цель
            if (Input.GetMouseButtonDown(0))
            {
                PickUpIt();
            }
        }
    }
    private Ray GetMouseRay()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        return ray;
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
        GetComponent<SphereCollider>().enabled = shouldShow;

        // Для каждого дочернего объекта пикапа включаем или выключаем его активность
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(shouldShow);
        }
    }
}
