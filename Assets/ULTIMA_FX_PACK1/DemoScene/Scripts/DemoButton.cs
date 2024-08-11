using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DemoButton : MonoBehaviour
{
    public Button Button;
    public Image Image;
    public Text Text;

    [SerializeField] private bool _enabledState = true;

    public Color EnabledTextColor;
    public Color DisabledTextColor;

    private void Start()
    {
        Button.onClick.AddListener(OnClickHandler);
        UpdateButton();
    }

    private void OnClickHandler()
    {
        _enabledState = !_enabledState;
        UpdateButton();
    }

    void UpdateButton()
    {
        Text.color = _enabledState ? EnabledTextColor : DisabledTextColor;
        Image.color = _enabledState ? Color.white : new Color(1, 1, 1, 0.4f);
    }
	
	public void SetState(bool state)
	{
		_enabledState = state;
		UpdateButton();
	}
}
