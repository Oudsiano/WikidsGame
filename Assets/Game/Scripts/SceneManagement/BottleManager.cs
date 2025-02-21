using Core.PickableItems;
using UnityEngine;
using UnityEngine.Serialization;

namespace SceneManagement
{
    public class BottleManager : MonoBehaviour // TODO Restruct 
    {
        [FormerlySerializedAs("_Bottle")] [SerializeField] private GameObject _prefab;

        public void MakeBottleOnSceneWithCount(float count, Vector3 pos)  // TODO move to factory Bottles mb OBJPOOL
        {
            double countCoins = count;
            Instantiate(_prefab, pos, Quaternion.Euler(0, 0, 0))
                .GetComponent<PickableHPBottle>()
                .Init(count);
        }
    }
}
