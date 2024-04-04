using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Core;

namespace RPG.Combat
{
    // Класс, представляющий цель для атаки
    [RequireComponent(typeof(Health))] // Обязательное присутствие компонента здоровья
    public class CombatTarget : MonoBehaviour
    {
        // Метод Start вызывается перед первым обновлением кадра
        void Start()
        {
            // На данный момент метод не содержит дополнительной логики
        }

        // Метод Update вызывается один раз за кадр
        void Update()
        {
            // На данный момент метод не содержит дополнительной логики
        }
    }
}
