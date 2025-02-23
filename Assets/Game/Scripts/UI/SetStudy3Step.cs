using UnityEngine;

namespace UI
{
    public class SetStudy3Step : MonoBehaviour // TODO Duplicate
    {
        private void OnTriggerEnter(Collider other)
        {
            IGame.Instance._uiManager.HelpInFirstScene.Study3();
        }

        private void OnTriggerExit(Collider other)
        {
            IGame.Instance._uiManager.HelpInFirstScene.EndStudy3();
        }
    }
}
