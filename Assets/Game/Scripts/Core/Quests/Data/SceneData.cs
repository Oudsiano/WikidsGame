using System;
using System.Collections.Generic;
using SceneManagement;
using SceneManagement.Enums;

namespace Core.Quests.Data
{
    [Serializable]
    public class SceneData
    {
        public allScenes scene;
        public List<int> numbers = new List<int>();
    }
}