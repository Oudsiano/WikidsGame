using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class SpecificCharacterDeathHandler : MonoBehaviour
{
    [FormerlySerializedAs("specificCharacter")] [SerializeField] private GameObject _specificCharacter;

    private void Start() // TODO construct
    {
        if (_specificCharacter != null)
        {
            Health.Health health = _specificCharacter.GetComponent<Health.Health>();
            
            if (health != null)
            {
                health.OnDeath += HandleCharacterDeath;
            }
        }
    }

    private void HandleCharacterDeath()
    {
        SceneManager.LoadScene(8); // Загрузка сцены номер 8 // TODO can be cached
    }
}
