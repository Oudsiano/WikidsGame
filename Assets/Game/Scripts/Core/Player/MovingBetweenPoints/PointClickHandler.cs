using System.Collections.Generic;
using Movement;
using UnityEngine;
using UnityEngine.AI;

namespace Core.Player.MovingBetweenPoints
{
    public class PointClickHandler
    {
        private float _duration = 5f;
        
        private MainPlayer _player;
        private Timer _timer;
        
        public void Construct(MainPlayer player, Timer timer)
        {
            _player = player;
            _timer = timer;
            
            _timer.Construct(_duration);
        }

        public async void HandleClick(Transform newPosition)
        {
            var mover = _player.GetComponent<Mover>();
            mover?.Cancel(); 

            if (NavMesh.SamplePosition(newPosition.position, out var hit, 2f, NavMesh.AllAreas))
            {
                Debug.Log($"[Teleport] До Warp: {_player.transform.position}");
                
                var agent = _player.Agent;
                agent.ResetPath();
                agent.isStopped = true;
                
                bool success = agent.Warp(hit.position);
                
                if (!success)
                {
                    Debug.LogWarning("Warp НЕ сработал!");
                    return;
                }
                
                agent.ResetPath();
                agent.isStopped = false;

                Debug.Log($"[Teleport] После Warp: {_player.transform.position}");

                await _timer.SetupCooldown();
            }
            else
            {
                Debug.LogWarning("Целевая точка вне NavMesh!");
            }
        }
    }
}