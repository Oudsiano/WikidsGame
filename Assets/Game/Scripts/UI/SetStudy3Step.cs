using UnityEngine;

namespace UI
{
    public class SetStudy3Step : SetStudyStep // TODO Duplicate
    {
        private void OnTriggerEnter(Collider other)
        {
            UiManager.HelpInFirstScene.Study3();
        }

        private void OnTriggerExit(Collider other)
        {
            UiManager.HelpInFirstScene.EndStudy3();
        }
    }
}
