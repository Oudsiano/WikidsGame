using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Core.Quests
{
    public class AllQuestsInGame : MonoBehaviour
    {
        [FormerlySerializedAs("AllQuests")] [SerializeField] private List<OneSceneListQuests> _allQuests;

        public IReadOnlyList<OneSceneListQuests> Quests => _allQuests;
    }
}