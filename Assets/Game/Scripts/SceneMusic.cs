using UnityEngine;

public class SceneMusic : MonoBehaviour
{
    private void Start() // TODO construct
    {
         AudioManager.Instance.PlayMusic("SceneFirstMusic");
        // SoundManager.PlayMusic("SceneFirstMusic");
    }
}
