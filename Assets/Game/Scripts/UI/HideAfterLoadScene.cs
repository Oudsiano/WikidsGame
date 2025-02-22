using System.Collections;
using UnityEngine;

namespace UI
{
    public class HideAfterLoadScene : MonoBehaviour
    {
        private void Start()
        {
            StartCoroutine(CallFunctionWithDelay(1.0f)); // TODO magic numbers
        }

        private IEnumerator CallFunctionWithDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            gameObject.SetActive(false);
        }
    }
}
