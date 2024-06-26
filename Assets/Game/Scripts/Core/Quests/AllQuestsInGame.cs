using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static LevelChangeObserver;
[System.Serializable]
public class OneSceneListQuests
{
    [SerializeField]
    public allScenes SceneId;
    [SerializeField]
    public List<OneQuest> QuestsThisScene;
}


public class AllQuestsInGame : MonoBehaviour
{
    [SerializeField]
    public List<OneSceneListQuests> AllQuests;
}
