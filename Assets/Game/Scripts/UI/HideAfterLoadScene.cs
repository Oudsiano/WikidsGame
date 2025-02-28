using Cysharp.Threading.Tasks;
using UnityEngine;

namespace UI
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
            await UniTask.Delay((int)1 * 1000);
            
            gameObject.SetActive(false);
        }
    }
}
