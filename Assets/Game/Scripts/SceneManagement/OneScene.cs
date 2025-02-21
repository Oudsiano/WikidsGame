using System;
using SceneManagement.Enums;
using UnityEngine;

namespace SceneManagement
{
    [Serializable]
    public class OneScene // TODO rename
    {
        [SerializeField] public allScenes IdScene;
        [SerializeField] public string fileScene;
    }
}