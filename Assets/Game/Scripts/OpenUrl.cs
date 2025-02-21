using UnityEngine;

public class OpenURL : MonoBehaviour
{
    public string urlToOpen; // TODO OC

    public void Open()
    {
        Application.OpenURL(urlToOpen);
        Debug.Log("сайт должен был открыться");
    }
}