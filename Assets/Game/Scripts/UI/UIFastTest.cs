using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIFastTest : MonoBehaviour
{

    [SerializeField] Button btnClose;

    [SerializeField] Button btn1;
    [SerializeField] Button btn2;
    [SerializeField] Button btn3;
    [SerializeField] Button btn4;

    [SerializeField] TMP_Text testText;


    private void Awake()
    {
        btnClose.onClick.AddListener(OnClickClose);
        SetTexts("Main Question Text", "Answer 1", "Answer 2", "Answer 3", "Answer 4");

    }

    public void SetTexts(string mainText, string btnText1, string btnText2, string btnText3, string btnText4)
    {
        // Устанавливаем текст для основного текстового поля
        testText.text = mainText;

        // Устанавливаем текст для каждой кнопки
        btn1.GetComponentInChildren<TMP_Text>().text = btnText1;
        btn2.GetComponentInChildren<TMP_Text>().text = btnText2;
        btn3.GetComponentInChildren<TMP_Text>().text = btnText3;
        btn4.GetComponentInChildren<TMP_Text>().text = btnText4;
    }

    private void OnClickClose()
    {
        gameObject.SetActive(false);
        IGame.Instance.SavePlayerPosLikeaPause(false);
        pauseClass.IsOpenUI = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
