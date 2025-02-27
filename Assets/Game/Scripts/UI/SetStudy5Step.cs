using UnityEngine;

namespace UI
{
    public class SetStudy5Step : SetStudyStep // TODO Duplicate
    {
        private void OnTriggerEnter(Collider other)
        {
            UiManager.HelpInFirstScene.Study5();
        }

    }
}
