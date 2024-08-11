using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static LevelChangeObserver;

[Serializable]
public class SceneData
{
    public allScenes scene;
    public List<int> numbers = new List<int>();
}

[Serializable]
public class SceneWithTestsID : MonoBehaviour
{
    [SerializeField]
    public List<SceneData> sceneDataList = new List<SceneData>();
}