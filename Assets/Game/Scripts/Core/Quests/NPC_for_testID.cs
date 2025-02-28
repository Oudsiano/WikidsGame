using Data;
using DialogueEditor;
using Saving;
using SceneManagement;
using UI;
using UnityEngine;
using UnityEngine.Serialization;


namespace Core.Quests
{
    public class NPC_for_testID : MonoBehaviour // TODO Rename
    {
        private const string TEST_COMPLETED = "ThisTestCompleted";
        private const string SPLASH_ORANGE_NAME = "Splash_orange";

        [FormerlySerializedAs("TestID")] [SerializeField]
        private int _testID;

        [FormerlySerializedAs("coins")] [SerializeField]
        private int _coins = 100;

        [FormerlySerializedAs("MeshGameObject")] [SerializeField]
        private GameObject _meshNPC; // TODO GO

        [FormerlySerializedAs("IconText")] [SerializeField]
        private string _iconText;

        private GameObject _parent; // TODO GO
        private IconForFarCamera _icon;
        private OpenURL _thisOpenURL;
        private Transform _splashOrangeTransform;

        private GameAPI _gameAPI;
        private DataPlayer _dataPlayer;
        private FastTestsManager _fastTestsManager;
        private SaveGame _saveGame;

        public int TestID => _testID;

        public void Construct(GameAPI gameAPI, DataPlayer dataPlayer, FastTestsManager fastTestsManager,
            SaveGame saveGame)
        {
            _gameAPI = gameAPI;
            _dataPlayer = dataPlayer;
            _fastTestsManager = fastTestsManager;
            _saveGame = saveGame;

            _thisOpenURL = GetComponent<OpenURL>();
            var oldOpenURL = transform.parent.GetComponent<OpenURL>();

            if (oldOpenURL != null)
            {
                int maxCountURL = 2;

                if (oldOpenURL.urlToOpen.Length > maxCountURL) // TODO magic number
                {
                    _thisOpenURL.urlToOpen = oldOpenURL.urlToOpen;
                }
            }

            _icon = GetComponentInChildren<IconForFarCamera>();
            _icon.description = _iconText;
        }

        public void SetParent(GameObject parent)
        {
            _parent = parent;

            Debug.Log(_parent.name);
        }

        public void SuccessAnswer()
        {
            AddCoinsToPlayer();
            DeactivateInteract();
        }

        public void IsTestCompleted()
        {
            if (_testID == 0)
            {
                Debug.LogError("Not have TestID in inspector");
            }

            _gameAPI.IsTestCompleted(_testID, (isCompleted) =>
            {
                ConversationManager.Instance.SetBool(TEST_COMPLETED, isCompleted);

                if (isCompleted)
                {
                    if (_dataPlayer.PlayerData.wasSuccessTests.Contains(_testID) == false)
                    {
                        _dataPlayer.PlayerData.wasSuccessTests.Add(_testID);
                        _fastTestsManager.GenAvaliableTests();
                    }
                }
            });
        }

        private void AddCoinsToPlayer()
        {
            _saveGame.Coins += _coins;
        }

        private void DeactivateInteract()
        {
            NPCInteractable interact = _parent.GetComponent<NPCInteractable>();
            interact.posibleInteract = false;
            Transform splashOrangeTransform = transform.Find(SPLASH_ORANGE_NAME); // TODO find change

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