using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetStudy5Step : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        IGame.Instance.UIManager.HelpInFirstScene.Study5();
    }

}
