using UnityEngine;
using UnityEngine.Serialization;

namespace Core
{
    public class PersistentObjectSpawner : MonoBehaviour
    {
        [SerializeField] private string _saveFileLink; // TODO not used code
        [FormerlySerializedAs("persistentObjectPrefab")] [SerializeField] private GameObject _persistentObjectPrefab;

        public static bool IsSpawned = false; // TODO static
        
        private void Awake() // TODO Construct
        {
            if (IsSpawned)
            {
                return;
            }

            IsSpawned = true;
            SpawnPersistentObjects();
        }

        private void SpawnPersistentObjects()
        {
            GameObject persistentObject = Instantiate(_persistentObjectPrefab);
            DontDestroyOnLoad(persistentObject);
        }
    }
}
