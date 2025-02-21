using DialogueEditor;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class ConversationMarket : MonoBehaviour
{
    [FormerlySerializedAs("marketAvaliable")] [SerializeField]
    private bool _marketAvailiable = true;

    [FormerlySerializedAs("MarketBtn")] [SerializeField]
    private Button _marketButton;

    [FormerlySerializedAs("MinPriceMarket")] [SerializeField]
    private int _minPriceMarket;

    [FormerlySerializedAs("MaxPriceMarket")] [SerializeField]
    private int _maxPriceMarket;

    private void Awake()  // TODO Construct
    {
        _marketButton.gameObject.SetActive(false);
        ConversationManager.OnConversationStarted += OnStartConversation;
        ConversationManager.OnConversationEnded += OnConversationEnded;

        _marketButton.onClick.AddListener(OnClickButton);
    }

    private void OnDestroy()
    {
        ConversationManager.OnConversationStarted -= OnStartConversation;
        ConversationManager.OnConversationEnded -= OnConversationEnded;
        _marketButton.onClick.RemoveAllListeners();
    }

    private void OnConversationEnded()
    {
        if (_marketButton != null)
        {
            _marketButton.gameObject.SetActive(false);
        }
    }

    private void OnStartConversation()
    {
        if (_marketAvailiable == false)
        {
            _marketButton.gameObject.SetActive(false);
        }
        else
        {
            _marketButton.gameObject.SetActive(true);
        }
    }

    private void OnClickButton()
    {
        if (IGame.Instance.UIManager.UiMarketPanel != null)
        {
            IGame.Instance.UIManager.OpenMarket(_minPriceMarket, _maxPriceMarket);
        }
    }
}