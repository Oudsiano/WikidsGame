using UnityEngine;
using UnityEngine.Serialization;

public class DestroyOtherObject : MonoBehaviour
{
    [FormerlySerializedAs("objectToDestroy")] [SerializeField]
    private GameObject _objectToDestroy;

    [FormerlySerializedAs("objectToUpdate")] [SerializeField]
    private GameObject _objectToUpdate;

    private void OnDestroy()
    {
        if (_objectToDestroy != null)
        {
            _objectToDestroy.SetActive(false);
        }

        if (_objectToUpdate != null)
        {
            _objectToUpdate.SetActive(true);
        }
    }
}