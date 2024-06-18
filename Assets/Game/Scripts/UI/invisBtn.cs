using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class invisBtn : MonoBehaviour
{
    [SerializeField] private Image BG;
    [SerializeField] private TMPro.TMP_Text text;
    [SerializeField] private Image sector;
    [SerializeField] public string textBtn;
    private Button thisBtn;

    private float timeBtn;
    private bool timeAnimate;

    private void Awake()
    {
        thisBtn = GetComponent<Button>();
        thisBtn.onClick.AddListener(onClick);

        timeAnimate = false;
        BG.color = new Color(0.6036846f, 0.9622642f, 0.606497f, 0.509804f);
        sector.fillAmount = 0;

        // ?????????, ??? ????? ???????????? ?????????
        TMPro.TMP_FontAsset font = Resources.Load<TMPro.TMP_FontAsset>("Fonts & Materials/YourCyrillicSupportingFont");
        text.font = font;
    }

    private void onClick()
    {
        if (timeAnimate) return;
        timeBtn = 0;
        timeAnimate = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (!timeAnimate) return;

        timeBtn += Time.deltaTime;

        if (timeBtn > 30)
        {
            timeAnimate = false;
            BG.color = new Color(0.6036846f, 0.9622642f, 0.606497f, 0.509804f);
            text.text = $"{textBtn}";
            ;
            sector.fillAmount = 0;
            IGame.Instance.playerController.SetInvisByBtn(false);
        }
        else if (timeBtn > 15)
        {
            BG.color = new Color(0.6509434f, 0.6509434f, 0.6509434f, 0.509804f);
            text.text = ((int)(30 - timeBtn)).ToString();
            sector.fillAmount = (30 - timeBtn) / 30f;
            IGame.Instance.playerController.SetInvisByBtn(false);
        }
        else
        {
            BG.color = new Color(1, 0.7599125f, 0, 0.7019608f);
            text.text = ((int)(30 - timeBtn)).ToString();
            sector.fillAmount = (30 - timeBtn) / 30f;
            IGame.Instance.playerController.SetInvisByBtn(true);
        }
    }
}
