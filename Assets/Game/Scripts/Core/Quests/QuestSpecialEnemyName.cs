using UnityEngine;
using UnityEngine.Serialization;

namespace Core.Quests
{
    public class QuestSpecialEnemyName : MonoBehaviour
    {
        [FormerlySerializedAs("specialEnemyName")] [SerializeField] private string _specialEnemyName; // TODO OC error

        public string SpecialEnemyName => _specialEnemyName;
    }
}
