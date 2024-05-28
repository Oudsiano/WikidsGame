using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnswerHandler : MonoBehaviour
{
    public void AddCoinsToPlayer()
    {
        IGame.Instance.saveGame.Coins += 100;
    }

    public void DeactivateCollider(GameObject targetObject)
    {
        Collider collider = targetObject.GetComponent<Collider>();
        if (collider != null)
        {
            collider.enabled = false;
        }
        else
        {
            Debug.LogWarning("Collider not found on the target object.");
        }
    }
}
