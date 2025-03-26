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
    public class EnemyMover : Mover
    {
        private void Update()
        {
            if (_disableInput)
            {
                return;
            }
            
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }
            
            if (_agent.isActiveAndEnabled && _agent.isOnNavMesh)
            {
                if (PauseClass.GetPauseState())
                {
                    _agent.isStopped = true;
                }
                else if (_agent.isStopped)
                {
                    _agent.isStopped = false;
                }
            }
            

            UpdateAnimator();
        }
        
        public override void MoveTo(Vector3 position)
        {
            if (_agent == null || !_agent.isActiveAndEnabled || !_agent.isOnNavMesh)
            {
                return;
            }
            
            _agent.destination = position;
            _agent.isStopped = false;
        }
        
    }
}
