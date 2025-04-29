using UnityEngine;
using UnityEngine.Serialization;

public class SetMusicForThisScene : MonoBehaviour
{
    [FormerlySerializedAs("musicFileName")][SerializeField]  private string _musicFileName;

    private void Start() // TODO construct
    {
        Debug.Log("SetMusicForThisScene Start");
        
        AudioListener[] listeners = FindObjectsOfType<AudioListener>();
        Debug.Log($"Found {listeners.Length} AudioListeners in scene:");
        foreach (var listener in listeners)
        {
            Debug.Log($"AudioListener on {listener.gameObject.name}, Active: {listener.gameObject.activeInHierarchy}");
        }
        
        if (_musicFileName.Length > 0)
        {
            Debug.Log($"AudioListener.paused: {AudioListener.pause}, Time.timeScale: {Time.timeScale}");
            
            SoundManager.PlayMusic(_musicFileName);
            // AudioManager.Instance.PlaySound("_musicFileName");
            Debug.Log("music is playing "+_musicFileName);
        }
    }
}