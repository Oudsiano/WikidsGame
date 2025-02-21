using System;
using System.Collections.Generic;
using SceneManagement;
using UnityEngine;

namespace Core.Quests
{
    [Serializable]
    public class SceneData
    {
        public LevelChangeObserver.allScenes scene;
        public List<int> numbers = new List<int>();
    }

    [Serializable]
    public class SceneWithTestsID : MonoBehaviour
    {
        [SerializeField]
        public List<SceneData> sceneDataList = new List<SceneData>();
    }
}