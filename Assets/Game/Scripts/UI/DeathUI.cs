using DG.Tweening;
using TMPro;
using UnityEngine;

namespace UI
{
    public class DeathUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _deathText;

        public void ShowDeathScreen()
        {
            gameObject.SetActive(true);
            _deathText.fontSize = 42;
            _deathText.transform.DOKill();
            _deathText.transform.localScale = new Vector3(0.7f, 0.7f, 0.7f); // TODO magic numbers
            _deathText.transform.DOScale(new Vector3(1, 1, 1), 3)
                .OnComplete(() =>
                {
                    gameObject.SetActive(false);
                    IGame.Instance._uiManager.ShowAgainUi();
                });
        }
    }
}