using DialogueEditor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC_for_testID : MonoBehaviour
{

    [Header("NPC mesh object")]
    public GameObject MeshGameObject;


    [Header("Icon")]
    public string IconText;


    [Header("TestID")]
    [SerializeField] public int TestID;



    [Header("SuccessCoins")]
    [SerializeField] public int coins=100;

    private IconForFarCamera _icon;
    private OpenURL _thisOpenURL;
    private Transform splashOrangeTransform;

    private GameObject parentGO;

    public void setParentGO(GameObject _go)
    {
        parentGO = _go;
        Debug.Log(parentGO.name);
    }


    // Start is called before the first frame update
    void Start()
    {
        _thisOpenURL = GetComponent<OpenURL>();

        var _oldOpenUrl = transform.parent.GetComponent<OpenURL>();
        if (_oldOpenUrl != null)
            if (_oldOpenUrl.urlToOpen.Length>2)
            _thisOpenURL.urlToOpen = _oldOpenUrl.urlToOpen;

        _icon = transform.Find("Icon").GetComponent<IconForFarCamera>();
        _icon.description = IconText;


        Transform splashOrangeTransform = transform.Find("Splash_orange");


    }

    public void SuccessAnsver()
    {
        AddCoinsToPlayer();
        DeactivateCollider();
    }


    public void AddCoinsToPlayer()
    {
        IGame.Instance.saveGame.Coins += coins;
    }

    public void DeactivateCollider()
    {

        // Отключаем коллайдер
        Collider collider = parentGO.GetComponent<Collider>();
        if (collider != null)
        {
            collider.enabled = false;
        }
        else
        {
            Debug.LogWarning("Collider not found on the target object.");
        }

        // Отключаем дочерний объект с именем "Splash_orange"
        if (splashOrangeTransform != null)
        {
            splashOrangeTransform.gameObject.SetActive(false);
        }
        else
        {
            Debug.LogWarning("Child object 'Splash_orange' not found.");
        }
    }


    public void IsTestCompleted()
    {
        if (TestID == 0)
        {
            Debug.LogError("Not have TestID in inspector");
        }
        FindObjectOfType<GameAPI>().IsTestCompleted(TestID, (isCompleted) =>
        {
            if (isCompleted)
            {
                Debug.Log("test completed znachenie update");
                ConversationManager.Instance.SetBool("ThisTestCompleted", true);
            }
            else
            {
                Debug.Log("test not completed znachenie update");
                ConversationManager.Instance.SetBool("ThisTestCompleted", false);
            }
        });
    }
}
