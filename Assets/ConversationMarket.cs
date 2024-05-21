using DialogueEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConversationMarket : MonoBehaviour
{
    public bool marketAvaliable = true;

    public Button MarketBtn;
    public int MinPriceMarket;
    public int MaxPriceMarket;


    public void Awake()
    {
        MarketBtn.gameObject.SetActive(false);
        ConversationManager.OnConversationStarted += onStartConversation;
        ConversationManager.OnConversationEnded += OnConversationEnded;

        MarketBtn.onClick.AddListener(onClickBtn);
    }

    private void OnConversationEnded() => MarketBtn.gameObject.SetActive(false);

    private void onStartConversation()
    {
        if (!marketAvaliable)
            MarketBtn.gameObject.SetActive(false);
        else
            MarketBtn.gameObject.SetActive(true);
    }

    private void onClickBtn()
    {
        if (IGame.Instance.UIManager.UiMarketPanel != null)
        {
            IGame.Instance.UIManager.OpenMarket(MinPriceMarket, MaxPriceMarket);
        }
        else
        {
            Debug.LogError("Нет маркета в инстансе");
        }
    }

    private void OnDestroy()
    {
        ConversationManager.OnConversationStarted -= onStartConversation;
        MarketBtn.onClick.RemoveAllListeners();
    }
}
