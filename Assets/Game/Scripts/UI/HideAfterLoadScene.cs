using System.Collections;
using UnityEngine;
public class HideAfterLoadScene : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(CallFunctionWithDelay(1.0f));
    }

    private IEnumerator CallFunctionWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        gameObject.SetActive(false);
    }


    
}
