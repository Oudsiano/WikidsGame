using System.Collections;
using System.Collections.Generic;
using TMPro;
using DG.Tweening;
using UnityEngine;

public class DeathUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _deathText;

    public void ShowDeathScreen()
    {

        gameObject.SetActive(true);
        _deathText.fontSize = 42;
        _deathText.transform.DOKill();
        _deathText.transform.localScale = new Vector3(0.7f, 0.7f, 0.7f);
        _deathText.transform.DOScale(new Vector3(1, 1, 1), 3)
            .OnComplete(() =>
            {
                gameObject.SetActive(false);
                IGame.Instance.UIManager.ShowAgainUi();
            });
    }

}
