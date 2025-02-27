using UnityEngine;

namespace UI
{
    public class SetStudyStep : MonoBehaviour
    {
        private UIManager _uiManager;

        public void Construct(UIManager uiManager)
        {
            _uiManager = uiManager;
        }

        protected UIManager UiManager => _uiManager;
    }
}