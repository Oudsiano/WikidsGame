using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum QuestType
{
    killEnemy,
    VisitPoints,

}

public class QuestManager : MonoBehaviour
{
    public List<OneQuest> thisQuestsScene;

    private QuestsForThisScene _QuestsForThisScene;
    
    public void Init()
    {
        RPG.Core.SceneLoader.LevelChanged += SceneLoader_LevelChanged;
    }

    private void SceneLoader_LevelChanged(LevelChangeObserver.allScenes obj)
    {
        _QuestsForThisScene = FindObjectOfType<QuestsForThisScene>();
        if (_QuestsForThisScene!=null)
        {
            thisQuestsScene = new List<OneQuest>(_QuestsForThisScene.QuestsThisScene);
        }
    }
}
