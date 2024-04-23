using UnityEngine;

public class DestroyOtherObject : MonoBehaviour
{
    public GameObject objectToDestroy; // ссылка на объект, который нужно уничтожить

    private void OnDestroy()
    {
        if (objectToDestroy != null)
        {
            objectToDestroy.SetActive(false);
        }
    }
}