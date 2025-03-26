using Core;
using Core.Interfaces;
using Core.Player;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using Utils;

namespace Movement
{
    // Класс, отвечающий за перемещение персонажа и взаимодействие с ним
    public abstract class Mover : MonoBehaviour, IAction
    {
        protected Animator _animator;
        protected NavMeshAgent _agent;
        protected ActionScheduler _actionScheduler; // Ссылка на планировщик действий
        
        
        public NPCInteractable Target;
        protected bool _disableInput = false;
        
        public bool DisableInput =>_disableInput;

        public virtual void Construct()
        {
            _animator = GetComponent<Animator>();

            if (_agent == false)
            {
                _agent = GetComponent<NavMeshAgent>();
            }

            if (_actionScheduler == false)
            {
                _actionScheduler = GetComponent<ActionScheduler>();
            }
        }
        
        
        public void DeactivateInput() => _disableInput = true;
        public void ActivateInput() => _disableInput = false;
        

        public void SetupMove(Vector3 newPosition)
        {
            _actionScheduler.Setup(this);
            MoveTo(newPosition);
        }

        public abstract void MoveTo(Vector3 position);


        protected void UpdateAnimator()
        {
            if (_agent == false)
            {
                return;
            }

            Vector3 localVelocity = transform.InverseTransformDirection(_agent.velocity);
            _animator.SetFloat(Constants.Animator.ForwardSpeed, localVelocity.z);
        }

        public void Cancel()
        {
            if (_agent.isActiveAndEnabled == false)
            {
                return;
            }

            if (_agent.isOnNavMesh)
            {
                _agent.isStopped = true;
            }
        }

        public bool IsAtLocation(float tolerance) // TODO Extensions vector3
        {
            return Vector3.Distance(_agent.destination, transform.position) < tolerance;
        }

        public void RestoreState(object state) // TODO not used
        {
            _actionScheduler = GetComponent<ActionScheduler>(); // Получаем планировщик действий
            _agent = GetComponent<NavMeshAgent>(); // Получаем навигационного агента
            _agent.enabled = false; // Отключаем навигационный агент
            _agent.enabled = true; // Включаем навигационный агент обратно
        }
    }
}