using UnityEngine;

namespace UI
{
    public class SetStudy4Step : MonoBehaviour // TODO Duplicate
    {
        private void OnTriggerEnter(Collider other)
        {
            IGame.Instance._uiManager.HelpInFirstScene.Study4();
        }

        private void OnTriggerExit(Collider other)
        {
            IGame.Instance._uiManager.HelpInFirstScene.EndStudy4();
        }
    }
}
