using UnityEngine;

namespace UI
{
    public class SetStudy4Step : SetStudyStep // TODO Duplicate
    {
        private void OnTriggerEnter(Collider other)
        {
            UiManager.HelpInFirstScene.Study4();
        }

        private void OnTriggerExit(Collider other)
        {
            UiManager.HelpInFirstScene.EndStudy4();
        }
    }
}
