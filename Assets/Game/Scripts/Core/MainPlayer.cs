using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Core
{
    // Класс, представляющий главного игрока
    public class MainPlayer : MonoBehaviour
    {
        private static MainPlayer _instance; // Статическая переменная для хранения единственного экземпляра главного игрока

        // Статический метод, который можно использовать для получения текущего экземпляра главного игрока
        public static MainPlayer Instance
        {
            get { return _instance; } // Возвращает текущий экземпляр
        }

        // Метод, вызываемый при активации объекта
        private void Awake()
        {
            // Если уже есть экземпляр главного игрока и он не равен текущему объекту
            if (_instance != null && _instance != this)
            {
                // Уничтожаем этот объект
                Destroy(this.gameObject);
            }
            else
            {
                // Иначе, если экземпляр еще не создан
                // Устанавливаем текущим экземпляром этот объект
                _instance = this;
                // Не уничтожаем этот объект при загрузке новой сцены
                DontDestroyOnLoad(this.gameObject);
            }
        }
    }
}
