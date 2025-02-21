using UnityEngine;

namespace UI
{
    public class SetStudy5Step : MonoBehaviour // TODO Duplicate
    {
        private void OnTriggerEnter(Collider other)
        {
            IGame.Instance.UIManager.HelpInFirstScene.Study5();
        }

    }
}
