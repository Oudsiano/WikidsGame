using UnityEngine;

namespace RPG.Core
{
    // Класс, отвечающий за планирование и управление действиями сущности
    public class ActionScheduler : MonoBehaviour
    {
        public IAction currentAction; // Текущее выполняемое действие

        // Метод для начала выполнения нового действия
        public void StartAction(IAction action)
        {
            // Если новое действие такое же, как текущее, то ничего не делаем
            if (currentAction == action)
                return;

            // Если текущее действие существует, отменяем его выполнение
            if (currentAction != null)
            {
                currentAction.Cancel();
            }
            // Устанавливаем новое действие как текущее
            currentAction = action;
        }

        // Метод для отмены текущего действия
        public void CancelAction()
        {
            // Вызываем StartAction с аргументом null для отмены текущего действия
            StartAction(null);
        }
    }
}
