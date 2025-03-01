using System;
using System.Collections.Generic;
using SceneManagement;
using SceneManagement.Enums;
using UnityEngine.Serialization;

namespace Core.Quests.Data
{
    [Serializable]
    public class SceneData
    {
        [FormerlySerializedAs("scene")] public int indexScene;
        public List<int> numbers = new List<int>();
    }
}