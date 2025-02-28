using System.Collections.Generic;
using Core.Quests.Data;
using UnityEngine;
using UnityEngine.Serialization;

namespace Core.Quests
{
    public class QuestsForThisScene : MonoBehaviour
    {
        [FormerlySerializedAs("QuestsThisScene")] [SerializeField]
        private List<OneQuest> _questsThisScene; // TODO not used code

        public IReadOnlyList<OneQuest> QuestsThisScene => _questsThisScene;
    }
}