using UnityEngine;

public class OpenURL : MonoBehaviour
{
    public string urlToOpen = "https://wikids.ru";

    public void Open()
    {
        Application.OpenURL(urlToOpen);
        Debug.Log("сайт должен был открыться");
    }
}