using UnityEngine;
using UnityEngine.SceneManagement;
using RPG.Core; // Пространство имен, содержащее класс Health

public class SpecificCharacterDeathHandler : MonoBehaviour
{
    public GameObject specificCharacter; // Укажите объект нужного персонажа

    void Start()
    {
        if (specificCharacter != null)
        {
            Health health = specificCharacter.GetComponent<Health>();
            if (health != null)
            {
                health.OnDeath += HandleCharacterDeath;
            }
        }
    }

    void HandleCharacterDeath()
    {
        // Здесь вы можете выполнить дополнительные действия перед загрузкой сцены
        SceneManager.LoadScene(8); // Загрузка сцены номер 8
    }
}
