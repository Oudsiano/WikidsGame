using UnityEngine;
using UnityEngine.Serialization;

namespace Core.Quests
{
    public class QuestSpecialEnemyName : MonoBehaviour
    {
        [FormerlySerializedAs("specialEnemyName")] [SerializeField] public string SpecialEnemyName; // TODO OC error
    }
}
