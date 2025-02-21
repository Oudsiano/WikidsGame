using Core.Interfaces;
using UnityEngine;

namespace Core
{
    // Класс, отвечающий за планирование и управление действиями сущности
    public class ActionScheduler : MonoBehaviour
    {
        private IAction _currentAction;

        // Метод для начала выполнения нового действия
        public void Setup(IAction action)
        {
            // Если новое действие такое же, как текущее, то ничего не делаем
            if (_currentAction == action)
            {
                return;
            }

            // Если текущее действие существует, отменяем его выполнение
            if (_currentAction != null)
            {
                _currentAction.Cancel();
            }

            _currentAction = action;
        }
        
        public void Cancel()
        {
            Setup(null);
        }
    }
}