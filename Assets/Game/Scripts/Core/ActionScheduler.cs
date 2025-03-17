using Core.Interfaces;
using UnityEngine;

namespace Core
{
    public class ActionScheduler : MonoBehaviour
    {
        private IAction _action;

        public IAction Action => _action;
        
        // Метод для начала выполнения нового действия
        public void Setup(IAction action)
        {
            if (_action == action)
            {
                return;
            }

            _action?.Cancel();

            _action = action;
        }
        
        public void Cancel()
        {
            Setup(null);
        }
    }
}