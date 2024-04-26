using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUIManager : MonoBehaviour
{

    [SerializeField] public Button ButtonShowMap;
    [SerializeField] public GameObject MapCanvas;

    // Start is called before the first frame update
    public void Init()
    {
        ButtonShowMap.onClick.AddListener(OnClickButtonMap);
    }

    private void OnClickButtonMap()
    {
        if (!MapCanvas.gameObject.activeSelf) MapCanvas.gameObject.SetActive(true);
    }

}
