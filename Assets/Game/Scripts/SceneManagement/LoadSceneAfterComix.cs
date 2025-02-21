using UnityEngine;
using UnityEngine.SceneManagement;

namespace SceneManagement
{
    public class LoadSceneAfterComix : MonoBehaviour
    {
        public void LoadSceneNext()
        {
            SceneManager.LoadScene("ChangeRegion"); // TODO can be cached
        }
    }
}
