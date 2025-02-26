using AINavigation;
using Core.PickableItems;
using UnityEngine;
using UnityEngine.Serialization;

namespace SceneManagement
{
    public class BottleManager : MonoBehaviour // TODO Restruct 
    {
        [FormerlySerializedAs("_Bottle")] [SerializeField] private GameObject _prefab;

        private CursorManager _cursorManager;
        private PlayerController _playerController;
        
        public void Construct(CursorManager cursorManager, PlayerController playerController)
        {
            _cursorManager = cursorManager;
            _playerController = playerController;
        }
        
        public void MakeBottleOnSceneWithCount(float count, Vector3 pos)  // TODO move to factory Bottles mb OBJPOOL
        {
            double countCoins = count;
            Instantiate(_prefab, pos, Quaternion.Euler(0, 0, 0))
                .GetComponent<PickableHPBottle>()
                .Construct(count, _cursorManager, _playerController);
        }
    }
}
