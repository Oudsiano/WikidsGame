using System.Collections.Generic;
using UnityEngine;

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
            _player.Agent.Warp(newPosition.position);
            
            await _timer.SetupCooldown();
        }
    }
}