using UnityEngine;

public class DestroyOtherObject : MonoBehaviour
{
    public GameObject objectToDestroy; // ?????? ?? ??????, ??????? ????? ??????????
    public GameObject objectToUpdate;
    private void OnDestroy()
    {
        if (objectToDestroy != null)
        {
            objectToDestroy.SetActive(false);
        }
        if (objectToUpdate != null)
        {
            objectToUpdate.SetActive(true);

        }
    }
}