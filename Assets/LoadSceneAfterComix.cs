using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneAfterComix : MonoBehaviour
{

    public void LoadSceneNext()
    {
        SceneManager.LoadScene("ChangeRegion");
    }
    
}
