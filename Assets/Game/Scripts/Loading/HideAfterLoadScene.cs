using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Loading
{
    public class HideAfterLoadScene : MonoBehaviour
    {
        private float _duration;
        
        private void Start()
        {
            CallFunctionWithDelay(_duration); // TODO magic numbers
        }

        private async UniTask CallFunctionWithDelay(float delay)
        {
            await UniTask.Delay(1 * 1000);
            
            gameObject.SetActive(false);
        }
    }
}
