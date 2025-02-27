using SceneManagement.Enums;
using UnityEngine;

namespace SceneManagement
{
    public class SceneComponent : MonoBehaviour
    {
        [SerializeField] public allScenes IdScene; // TODO rename

        //[SerializeField] public GameObject TheCore; // TODO not used code

        [Header("Camera New Params")] [SerializeField]
        public int newMinZoomCamera; // TODO rename

        [SerializeField] public int newMaxZoomCamera; // TODO rename

        [Header("Infection Group")] [SerializeField]
        public GameObject InfectGroup; // TODO rename

        private LevelChangeObserver _levelChangeObserver;
        
        public void Construct(LevelChangeObserver levelChangeObserver)
        {
            _levelChangeObserver = levelChangeObserver;
            
            if (InfectGroup == null)
            {
                InfectGroup = GameObject.Find("infection"); // TODO find change
                
                if (InfectGroup == null)
                {
                    InfectGroup = GameObject.Find("infested"); // TODO find change
                }
            }

            if (_levelChangeObserver.DictForInfected.ContainsKey(IdScene))
            {
                SetInfectGroupActive(_levelChangeObserver.DictForInfected[IdScene]);
            }
        }

        private void SetInfectGroupActive(bool isActive)
        {
            if (InfectGroup != null)
                InfectGroup.SetActive(isActive);
        }
    }
}