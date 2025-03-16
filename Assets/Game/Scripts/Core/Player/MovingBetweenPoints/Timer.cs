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

        public event Action Started;
        public event Action Ended;

        public void Construct(float duration)
        {
            _duration = duration;
        }

        public async UniTask SetupCooldown()
        {
            // Если таймер уже запущен — отменяем его
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource = new CancellationTokenSource();

            Started?.Invoke();
            Debug.Log("Таймер запущен!");

            try
            {
                // Ждем в течение _duration секунд
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
                _cancellationTokenSource.Dispose();
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