using UnityEngine;
using UnityEngine.Serialization;

namespace Core
{
    public class PersistentObjectSpawner : MonoBehaviour
    {
        [SerializeField] private string _saveFileLink; // TODO not used code

        [FormerlySerializedAs("persistentObjectPrefab")] [SerializeField]
        private GameObject _persistentObjectPrefab;

        private bool _isSpawned = false;

        private void Awake() // TODO Construct
        {
            if (_isSpawned)
            {
                return;
            }

            _isSpawned = true;
            SpawnPersistentObjects();
        }

        private void SpawnPersistentObjects()
        {
            GameObject persistentObject = Instantiate(_persistentObjectPrefab);
            DontDestroyOnLoad(persistentObject);
        }
    }
}