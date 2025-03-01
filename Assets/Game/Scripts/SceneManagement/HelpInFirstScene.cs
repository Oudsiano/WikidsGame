using Core.Camera;
using Data;
using SceneManagement.Enums;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

namespace SceneManagement
{
    public class HelpInFirstScene : MonoBehaviour
    {
        [FormerlySerializedAs("Panel")] [SerializeField]
        private GameObject _panel;

        [FormerlySerializedAs("text1")] [SerializeField]
        private GameObject _text1;

        [FormerlySerializedAs("text2")] [SerializeField]
        private GameObject _text2;

        [FormerlySerializedAs("text3")] [SerializeField]
        private GameObject _text3;

        [FormerlySerializedAs("text4")] [SerializeField]
        private GameObject _text4;

        [FormerlySerializedAs("text5")] [SerializeField]
        private GameObject _text5;

        private DataPlayer _dataPlayer;

        private SceneLoaderService _sceneLoader;
        
        public void Construct(DataPlayer dataPlayer, SceneLoaderService sceneLoader)
        {
            _sceneLoader = sceneLoader;
            _dataPlayer = dataPlayer;
                
            SceneManager.sceneLoaded += OnSceneLoaded;
            
            FollowCamera.OnCameraRotation += FollowCamera_OnCameraRotation;
            FollowCamera.OnCameraScale += FollowCamera_OnCameraScale;

            restTexts();
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode _)
        {
            Study1Show(scene.buildIndex);
        }
        
        private void OnDestroy()
        {
            FollowCamera.OnCameraRotation -= FollowCamera_OnCameraRotation;
            FollowCamera.OnCameraScale -= FollowCamera_OnCameraScale;
        }

        public void Study3() // TODO duplicate
        {
            if (_dataPlayer.PlayerData.helpIndex != 2)
            {
                return;
            }

            restTexts();
            _text3.SetActive(true);
            _panel.SetActive(true);
        }

        public void EndStudy3() // TODO duplicate
        {
            _panel.SetActive(false);
            _text3.SetActive(false);
            
            if (_dataPlayer.PlayerData.helpIndex != 2) return;
            {
                _dataPlayer.PlayerData.helpIndex = 3;
            }
        }

        public void Study4() // TODO duplicate
        {
            if (_dataPlayer.PlayerData.helpIndex != 3)
            {
                return;
            }

            restTexts();
            _text4.SetActive(true);
            _panel.SetActive(true);
        }

        public void EndStudy4() // TODO duplicate
        {
            _panel.SetActive(false);
            _text4.SetActive(false);
            if (_dataPlayer.PlayerData.helpIndex != 3) return;
            _dataPlayer.PlayerData.helpIndex = 4;
        }

        public void Study5() // TODO duplicate
        {
            restTexts();
            _text5.SetActive(true);
            _panel.SetActive(true);
            _dataPlayer.PlayerData.helpIndex = 5;
        }

        public void EndStudy5() // TODO duplicate
        {
            _panel.SetActive(false);
            _text5.SetActive(false);
        }

        private void FollowCamera_OnCameraRotation() => EndStudy1();
        private void FollowCamera_OnCameraScale() => EndStudy2();

        private void restTexts() // TODO duplicate
        {
            _panel.SetActive(false);
            _text1.SetActive(false);
            _text2.SetActive(false);
            _text3.SetActive(false);
            _text4.SetActive(false);
            _text5.SetActive(false);
            _panel.SetActive(false);
        }

        private void Study1Show(int scene) // TODO duplicate
        {
            if (scene != _sceneLoader.OpenScene)
            {
                return;
            }

            if (_dataPlayer.PlayerData.helpIndex != 0)
            {
                return;
            }

            if (_panel == null)
            {
                //Debug.LogError("panel null");
                return;
            }

            restTexts();
            _panel.SetActive(true);
            _text1.SetActive(true);
        }

        private void EndStudy1() // TODO duplicate
        {
            _text1.SetActive(false);
            _panel.SetActive(false);
            if (_dataPlayer.PlayerData.helpIndex != 0) return;
            _dataPlayer.PlayerData.helpIndex = 1;
            Study2();
        }


        private void Study2() // TODO duplicate
        {
            if (_dataPlayer.PlayerData.sceneToLoad != _sceneLoader.SecondBattleScene) return;
            if (_dataPlayer.PlayerData.helpIndex != 1) return;
            restTexts();
            _text2.SetActive(true);
            _panel.SetActive(true);
        }

        private void EndStudy2() // TODO duplicate
        {
            _panel.SetActive(false);
            _text2.SetActive(false);
            if (_dataPlayer.PlayerData.helpIndex != 1) return;
            _dataPlayer.PlayerData.helpIndex = 2;
        }
    }
}