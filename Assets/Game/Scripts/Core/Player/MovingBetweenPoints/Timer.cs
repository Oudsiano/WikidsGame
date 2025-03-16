using System;
using System.Collections;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Core.Player.MovingBetweenPoints
{
    public class Timer
    {
        private float _duration = 5f;
        private CancellationTokenSource _cancellationTokenSource;
        private bool _isRunning = false;
        
        public event Action Started;
        public event Action Ended;

        public float Duration => _duration;
        
        public void Construct(float duration)
        {
            _duration = duration;
        }
        
        public async UniTask SetupCooldown()
        {
            if (_isRunning)
            {
                Debug.LogWarning("Таймер уже запущен! Повторный запуск невозможен.");
                return;
            }

            _isRunning = true;
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource = new CancellationTokenSource();

            Started?.Invoke();
            Debug.Log("Таймер запущен!");

            try
            {
                await UniTask.Delay(TimeSpan.FromSeconds(_duration), cancellationToken: _cancellationTokenSource.Token);
                Debug.Log("Таймер завершён!");
                Ended?.Invoke();
            }
            catch (OperationCanceledException)
            {
                Debug.Log("Таймер был отменён!");
            }
            finally
            {
                _isRunning = false;
                _cancellationTokenSource?.Dispose();
                _cancellationTokenSource = null;
            }
        }

        public void Cancel()
        {
            if (_cancellationTokenSource != null)
            {
                _cancellationTokenSource.Cancel();
            }
        }
    }
}