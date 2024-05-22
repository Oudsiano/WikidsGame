using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetMusicForThisScene : MonoBehaviour
{
    public string musicFileName;


    // Start is called before the first frame update
    void Start()
    {
        if (musicFileName .Length>0)
        {
            SoundManager.PlayMusic(musicFileName);
        }
    }

}
