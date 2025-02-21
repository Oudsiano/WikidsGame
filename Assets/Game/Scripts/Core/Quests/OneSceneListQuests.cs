using System.Collections.Generic;
using Core.Quests.Data;
using SceneManagement;
using UnityEngine;

namespace Core.Quests
{
    [System.Serializable]
    public class OneSceneListQuests
    {
        [SerializeField] public LevelChangeObserver.allScenes SceneId; // TODO using error
        [SerializeField] public List<OneQuest> QuestsThisScene; // TODO O/C error
    }
}