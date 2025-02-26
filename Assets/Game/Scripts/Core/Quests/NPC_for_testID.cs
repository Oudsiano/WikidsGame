using Data;
using DialogueEditor;
using Saving;
using SceneManagement;
using UI;
using UnityEngine;

namespace Core.Quests
{
    public class NPC_for_testID : MonoBehaviour // TODO Rename
    {
        [Header("TestID")] [SerializeField] public int TestID;

        [Header("SuccessCoins")] [SerializeField]
        public int coins = 100;

        [Header("NPC mesh object")] public GameObject MeshGameObject; // TODO GO
        [Header("Icon")] public string IconText;

        private IconForFarCamera _icon;
        private OpenURL _thisOpenURL;
        private Transform _splashOrangeTransform;

        private GameObject _parentGO;
        private GameAPI _gameAPI;
        private DataPlayer _dataPlayer;
        private FastTestsManager _fastTestsManager;
        private SaveGame _saveGame;
        
        public void Construct(GameAPI gameAPI, DataPlayer dataPlayer, FastTestsManager fastTestsManager, SaveGame saveGame) // TODO construct
        {
            _gameAPI = gameAPI;
            _dataPlayer = dataPlayer;
            _fastTestsManager = fastTestsManager;
            _saveGame = saveGame;
            _thisOpenURL = GetComponent<OpenURL>();
            
            var _oldOpenUrl = transform.parent.GetComponent<OpenURL>();

            if (_oldOpenUrl != null)
            {
                if (_oldOpenUrl.urlToOpen.Length > 2) // TODO magic number
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

        public void SuccessAnswer()
        {
            AddCoinsToPlayer();
            DeactivateInteract();
        }

        public void IsTestCompleted()
        {
            if (TestID == 0)
            {
                Debug.LogError("Not have TestID in inspector");
            }
            
            _gameAPI.IsTestCompleted(TestID, (isCompleted) => 
            {
                ConversationManager.Instance.SetBool("ThisTestCompleted", isCompleted); // TODO can be cached

                if (isCompleted)
                {
                    if (_dataPlayer.PlayerData.wasSuccessTests.Contains(TestID) == false)
                    {
                        _dataPlayer.PlayerData.wasSuccessTests.Add(TestID);
                        _fastTestsManager.GenAvaliableTests();
                    }
                }
            });
        }

        private void AddCoinsToPlayer()
        {
            _saveGame.Coins += coins;
        }

        private void DeactivateInteract()
        {
            NPCInteractable interact = _parentGO.GetComponent<NPCInteractable>(); // TODO can be TRYGETCOMP
            interact.posibleInteract = false;

            Transform splashOrangeTransform = transform.Find("Splash_orange"); // TODO find change
            // ��������� �������� ������ � ������ "Splash_orange" // TODO UTF-8 code error
            if (splashOrangeTransform != null)
            {
                splashOrangeTransform.gameObject.SetActive(false);
            }
            else
            {
                Debug.LogWarning("Child object 'Splash_orange' not found.");
            }
        }
    }
}