using System.Threading;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace Core.Player.MovingBetweenPoints
{
    public class TimerView : MonoBehaviour
    {
        [SerializeField] private TMP_Text _text;
        
        private Timer _timer;
        private CancellationTokenSource _cancellationTokenSource;
        
        public void Construct(Timer timer)
        {
            _timer = timer;
            
            _timer.Started += OnTimerStarted;
            _timer.Ended += OnTimerEnded;
        }
        
        private void OnDestroy()
        {
            if (_timer != null)
            {
                _timer.Started -= OnTimerStarted;
                _timer.Ended -= OnTimerEnded;
            }
            _cancellationTokenSource?.Cancel();
        }
        
        private void OnTimerStarted()
        {
            gameObject.SetActive(true);
            StartUpdatingText().Forget();
        }

        private async UniTask StartUpdatingText()
        {
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource = new CancellationTokenSource();

            float remainingTime = _timer.Duration;

            while (remainingTime > 0)
            {
                _text.text = Mathf.CeilToInt(remainingTime).ToString();
                await UniTask.Delay(1000, cancellationToken: _cancellationTokenSource.Token);
                remainingTime -= 1f;
            }
        }

        private void OnTimerEnded()
        {
            gameObject.SetActive(false); // Скрываем текст по завершению таймера
        }
    }
}