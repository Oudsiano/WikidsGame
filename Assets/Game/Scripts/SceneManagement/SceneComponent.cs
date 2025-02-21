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

        private void Awake()
        {
            if (InfectGroup == null)
            {
                InfectGroup = GameObject.Find("infection"); // TODO find change
                
                if (InfectGroup == null)
                {
                    InfectGroup = GameObject.Find("infested"); // TODO find change
                }
            }

            if (IGame.Instance.LevelChangeObserver.DictForInfected.ContainsKey(IdScene))
            {
                SetInfectGroupActive(IGame.Instance.LevelChangeObserver.DictForInfected[IdScene]);
            }
        }

        private void SetInfectGroupActive(bool isActive)
        {
            if (InfectGroup != null)
                InfectGroup.SetActive(isActive);
        }
    }
}