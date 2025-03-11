using Core;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace UI
{
    public class InvisibilityButton : MonoBehaviour // TODO UI Panel Hierarchy
    {
        [FormerlySerializedAs("BG")] [SerializeField]
        private Image _background;

        [FormerlySerializedAs("text")] [SerializeField]
        private TMPro.TMP_Text _text;

        [FormerlySerializedAs("sector")] [SerializeField]
        private Image _sectorImage;

        [SerializeField] public string textBtn;
        private Button _button;

        private float _time;
        private bool _timeAnimate; // TODO why bool

        private void Awake() // TODO construct
        {
            _button = GetComponent<Button>();
            _button.onClick.AddListener(OnClick);

            _timeAnimate = false;
            // _background.color = new Color(0.6036846f, 0.9622642f, 0.606497f, 0.509804f); // TODO magic numbers
            // _sectorImage.fillAmount = 0;

            // Load a font that supports Cyrillic characters
            TMPro.TMP_FontAsset
                font = Resources.Load<TMPro.TMP_FontAsset>(
                    "Fonts & Materials/YourCyrillicSupportingFont"); // TODO why load here
            _text.font = font;
        }
    
        private void Update()
        {
            if (_timeAnimate == false)
            {
                return;
            }

            if (PauseClass.GetPauseState() == false)
            {
                _time += Time.deltaTime;
            }

            if (_time > 30)
            {
                _timeAnimate = false;
                _background.color = new Color(0.6036846f, 0.9622642f, 0.606497f, 0.509804f); // TODO magic numbers
                _text.text = $"{textBtn}";
                _sectorImage.fillAmount = 0;
                IGame.Instance.playerController.SetInvisByBtn(false);
            }
            else if (_time > 15)
            {
                _background.color = new Color(0.6509434f, 0.6509434f, 0.6509434f, 0.509804f); // TODO magic numbers
                _text.text = ((int)(30 - _time)).ToString();
                _sectorImage.fillAmount = (30 - _time) / 30f;
                IGame.Instance.playerController.SetInvisByBtn(false);
            }
            else
            {
                _background.color = new Color(1, 0.7599125f, 0, 0.7019608f); // TODO magic numbers
                _text.text = ((int)(30 - _time)).ToString(); // TODO magic numbers
                _sectorImage.fillAmount = (30 - _time) / 30f; // TODO magic numbers
                IGame.Instance.playerController.SetInvisByBtn(true);
            }
        }
    
        private void OnClick()
        {
            if (_timeAnimate)
            {
                return;
            }

            if (IGame.Instance.saveGame.Coins >= 15) // TODO magic numbers
            {
                IGame.Instance.saveGame.Coins -= 15; // TODO magic numbers
                _time = 0;
                Debug.Log("Нажали на кнопку инвиза");
                _timeAnimate = true;
            }
            else
            {
                Debug.Log("Недостаточно монет для инвиза");
            }
        }
    }
}