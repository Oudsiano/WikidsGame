using System;
using System.Collections.Generic;
using UnityEngine.Serialization;

namespace Core.Quests.Data
{
    [Serializable]
    public class SceneData
    {
        [FormerlySerializedAs("scene")] public string indexScene;
        public List<int> numbers = new List<int>();
    }
}