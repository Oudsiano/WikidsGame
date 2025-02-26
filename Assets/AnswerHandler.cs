using System.Collections;
using System.Collections.Generic;
using Saving;
using UnityEngine;

public class AnswerHandler : MonoBehaviour
{
    private SaveGame _saveGame;
    
    public void Construct(SaveGame saveGame)
    {
        _saveGame = saveGame;
    }
    
    public void AddCoinsToPlayer()
    {
        _saveGame.Coins += 100; // TODO Change
    }

    public void DeactivateCollider(GameObject targetObject)
    {
        // Отключаем коллайдер
        Collider collider = targetObject.GetComponent<Collider>();
        
        if (collider != null)
        {
            collider.enabled = false;
        }
        else
        {
            Debug.LogWarning("Collider not found on the target object.");
        }

        // Отключаем дочерний объект с именем "Splash_orange"
        Transform splashOrangeTransform = targetObject.transform.Find("Splash_orange");
        if (splashOrangeTransform != null)
        {
            splashOrangeTransform.gameObject.SetActive(false);
        }
        else
        {
            Debug.LogWarning("Child object 'Splash_orange' not found.");
        }
    }
}
