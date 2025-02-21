using UnityEngine;

namespace UI
{
    public class SetStudy3Step : MonoBehaviour // TODO Duplicate
    {
        private void OnTriggerEnter(Collider other)
        {
            IGame.Instance.UIManager.HelpInFirstScene.Study3();
        }

        private void OnTriggerExit(Collider other)
        {
            IGame.Instance.UIManager.HelpInFirstScene.EndStudy3();
        }
    }
}
