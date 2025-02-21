using UnityEngine;

namespace UI
{
    public class SetStudy4Step : MonoBehaviour // TODO Duplicate
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
}
