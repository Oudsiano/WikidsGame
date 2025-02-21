using DialogueEditor;
using UnityEngine;

namespace Core.Quests
{
    public class NPC_for_testID : MonoBehaviour // TODO Rename
    {
        [Header("TestID")] [SerializeField] public int TestID;
        [Header("SuccessCoins")] [SerializeField] public int coins = 100;
        [Header("NPC mesh object")] public GameObject MeshGameObject; // TODO GO
        [Header("Icon")] public string IconText;

        private IconForFarCamera _icon;
        private OpenURL _thisOpenURL;
        private Transform _splashOrangeTransform;

        private GameObject _parentGO;

        private void Start() // TODO construct
        {
            _thisOpenURL = GetComponent<OpenURL>();
            var _oldOpenUrl = transform.parent.GetComponent<OpenURL>();

            if (_oldOpenUrl != null)
            {
                if (_oldOpenUrl.urlToOpen.Length > 2)
                {
                    _thisOpenURL.urlToOpen = _oldOpenUrl.urlToOpen;
                }
            }

            _icon = transform.Find("Icon").GetComponent<IconForFarCamera>(); // TODO find change
            _icon.description = IconText;
        }

        public void SetParent(GameObject parent)
        {
            _parentGO = parent;
            
            Debug.Log(_parentGO.name);
        }

        public void SuccessAnsver()
        {
            AddCoinsToPlayer();
            DeactivateInteract();
        }


        public void AddCoinsToPlayer()
        {
            IGame.Instance.saveGame.Coins += coins;
        }

        public void DeactivateInteract()
        {
            NPCInteractable interract = _parentGO.GetComponent<NPCInteractable>();
            interract.posibleInteract = false;


            Transform splashOrangeTransform = transform.Find("Splash_orange");
            // ��������� �������� ������ � ������ "Splash_orange"
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
                ConversationManager.Instance.SetBool("ThisTestCompleted", isCompleted);

                if (isCompleted)
                    if (!IGame.Instance.dataPlayer.playerData.wasSuccessTests.Contains(TestID))
                    {
                        IGame.Instance.dataPlayer.playerData.wasSuccessTests.Add(TestID);
                        IGame.Instance.FastTestsManager.GenAvaliableTests();
                    }
            });
        }
    }
}