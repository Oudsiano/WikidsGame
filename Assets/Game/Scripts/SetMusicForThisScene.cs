using UnityEngine;
using UnityEngine.Serialization;

public class SetMusicForThisScene : MonoBehaviour
{
    [FormerlySerializedAs("musicFileName")][SerializeField]  private string _musicFileName;

    private void Start() // TODO construct
    {
        if (_musicFileName.Length > 0)
        {
            SoundManager.PlayMusic(_musicFileName);
            Debug.Log("music is playing "+_musicFileName);
        }
    }
}