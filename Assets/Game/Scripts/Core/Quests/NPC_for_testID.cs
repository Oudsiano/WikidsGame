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
        [FormerlySerializedAs("TestID")] [SerializeField]
        private int _testID;

        [FormerlySerializedAs("coins")] [SerializeField]
        private int _coins = 100;

        [FormerlySerializedAs("MeshGameObject")] [SerializeField]
        private GameObject _meshNPC; // TODO GO

        [FormerlySerializedAs("IconText")] [SerializeField]
        private string _iconText;

        private IconForFarCamera _icon;
        private OpenURL _thisOpenURL;
        private Transform _splashOrangeTransform;

        private GameObject _parent;
        private GameAPI _gameAPI;
        private DataPlayer _dataPlayer;
        private FastTestsManager _fastTestsManager;
        private SaveGame _saveGame;

        public void Construct(GameAPI gameAPI, DataPlayer dataPlayer, FastTestsManager fastTestsManager,
            SaveGame saveGame) // TODO construct
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

            _icon = GetComponentInChildren<IconForFarCamera>();
            _icon.description = _iconText;
        }

        public int TestID => _testID;

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
                ConversationManager.Instance.SetBool("ThisTestCompleted", isCompleted); // TODO can be cached

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
            NPCInteractable interact = _parent.GetComponent<NPCInteractable>(); // TODO can be TRYGETCOMP
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