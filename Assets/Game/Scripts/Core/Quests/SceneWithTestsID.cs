using System;
using System.Collections.Generic;
using Core.Quests.Data;
using UnityEngine;
using UnityEngine.Serialization;

namespace Core.Quests
{
    [Serializable]
    public class SceneWithTestsID : MonoBehaviour
    {
        [FormerlySerializedAs("sceneDataList")] [SerializeField]
        private List<SceneData> _sceneDataList = new();

        public IReadOnlyList<SceneData> SceneDataList => _sceneDataList;
    }
}