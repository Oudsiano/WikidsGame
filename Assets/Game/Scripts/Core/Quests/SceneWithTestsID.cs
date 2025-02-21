using System;
using System.Collections.Generic;
using Core.Quests.Data;
using UnityEngine;
using UnityEngine.Serialization;

namespace Core.Quests
{
    [Serializable]
    public class SceneWithTestsID : MonoBehaviour // TODO ?
    {
        [FormerlySerializedAs("sceneDataList")] [SerializeField] public List<SceneData> SceneDataList = new();
    }
}