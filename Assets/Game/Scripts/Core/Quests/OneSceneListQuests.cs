using System.Collections.Generic;
using Core.Quests.Data;
using NaughtyAttributes;
using SceneManagement;
using SceneManagement.Enums;
using UnityEngine;

namespace Core.Quests
{
    [System.Serializable]
    public class OneSceneListQuests
    {
        [SerializeField] public string SceneId; // TODO using error
        [SerializeField] public List<OneQuest> QuestsThisScene; // TODO O/C error
    }
}