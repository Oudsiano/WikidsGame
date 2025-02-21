using UnityEngine;

public class SceneMusic : MonoBehaviour
{
    private void Start() // TODO construct
    {
        AudioManager.instance.PlaySound("SceneFirstMusic");
    }
}
