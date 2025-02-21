using System;
using System.Collections.Generic;
using SceneManagement;

namespace Core.Quests.Data
{
    [Serializable]
    public class SceneData
    {
        public LevelChangeObserver.allScenes scene;
        public List<int> numbers = new List<int>();
    }
}