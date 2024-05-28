using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetStudy4Step : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        IGame.Instance.UIManager.HelpInFirstScene.Study4();
    }

    private void OnTriggerExit(Collider other)
    {
        IGame.Instance.UIManager.HelpInFirstScene.EndStudy4();
    }
}
