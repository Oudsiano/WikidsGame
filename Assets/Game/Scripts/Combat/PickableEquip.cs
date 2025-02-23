using System.Collections;
using UI.Inventory;
using UI.Inventory.Data;
using UnityEngine;
using UnityEngine.Serialization;

namespace Combat
{
    public class PickableEquip : MonoBehaviour
    {
        [FormerlySerializedAs("item")] [SerializeField]
        private ItemDefinition _item;
        //[SerializeField] private float respawnTime = 20f; // Время до появления оружия снова // TODO not used code

        // Метод, вызываемый при взаимодействии с коллайдером // TODO not used code
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

        private void Update()
        {
            var ray = GetMouseRay(); // TODO Duplicate code

            RaycastHit[] hits;
            hits = Physics.RaycastAll(ray, Mathf.Infinity);

            if (Input.GetMouseButtonDown(0))
            {
                foreach (var Hit in hits)
                {
                    PickableEquip target = Hit.transform.gameObject.GetComponent<PickableEquip>(); // TODO can be replaced to TRYGETCOMP

                    if (target == false)
                    {
                        continue;
                    }

                    if (target != this)
                    {
                        return;
                    }

                    PickUpIt();
                }
            }
        }

        public void SetItem(ItemDefinition item)
        {
            _item = item.CreateInstance();
        }

        private void PickUpIt()
        {
            if (IGame.Instance.WeaponArmorManager.IsWeaponInGame(_item.name))
            {
                if (_item != null)
                {
                    _item.CreateInstance();
                    IGame.Instance._uiManager.uIBug.TryAddEquipToBug(_item);
                }
                else
                {
                    Debug.LogError("mistake item");
                }
            }
            else
            {
                Debug.LogWarning("Этого предмета нет в списке предметов в WeaponArmorManager");
            }
            // Получаем компонент Fighter у объекта, который столкнулся с пикапом // TODO not used code
            //IGame.Instance.playerController.GetFighter().EquipItem(item);

            //StartCoroutine(HideForSeconds(respawnTime));

            Destroy(gameObject);
        }

        private Ray GetMouseRay()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); // TODO Camera.main 
            
            return ray;
        }
        
        private IEnumerator HideForSeconds(float seconds) // TODO not used code
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
            GetComponent<SphereCollider>().enabled = shouldShow; // TODO can be cached

            // Для каждого дочернего объекта пикапа включаем или выключаем его активность
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(shouldShow); 
            }
        }
    }
}