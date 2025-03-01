using SceneManagement.Enums;
using UnityEngine;

namespace SceneManagement
{
    public class SceneComponent : MonoBehaviour
    {
         [Header("Scene Settings")]
        [SerializeField] public int SceneID; // Идентификатор сцены // TODO DELETE

        [Header("Camera New Params")]
        [SerializeField] public int newMinZoomCamera;
        [SerializeField] public int newMaxZoomCamera;

        [Header("Infection Group")]
        [SerializeField] private GameObject infectGroup;

        private LevelChangeObserver _levelChangeObserver;
        
        public void Construct(LevelChangeObserver levelChangeObserver)
        {
            _levelChangeObserver = levelChangeObserver;

            SceneID = _levelChangeObserver.IndexSceneToLoad;
            // // Если объект не установлен, пробуем найти его в сцене
            // if (infectGroup == null)
            // {
            //     infectGroup = GameObject.Find("infection") ?? GameObject.Find("infested");
            // }
            // // Если в LevelChangeObserver имеется информация для данного идентификатора, устанавливаем состояние infectGroup
            // if (_levelChangeObserver.DictForInfected != null && _levelChangeObserver.DictForInfected.ContainsKey(sceneId))
            // {
            //     SetInfectGroupActive(_levelChangeObserver.DictForInfected[sceneId]);
            // } // TODO Infected group refactor
        }

        private void SetInfectGroupActive(bool isActive)
        {
            if (infectGroup != null)
                infectGroup.SetActive(isActive);
        }
    }
}