using System;
using System.Collections.Generic;
using NaughtyAttributes;
using SceneManagement.Enums;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace SceneManagement
{
    [Serializable]
    public class OneBtnChangeRegion // TODO rename
    {
        public Button Button;
        [Dropdown("GetSceneNames")] public string loadedScene;

        public void SetRed()
        {
            Button.GetComponent<Image>().color = new Color32(0xFF, 0x73, 0x5F, 0xFF); // TODO can be cached
        }

        internal void SetGreen()
        {
            Button.GetComponent<Image>().color = new Color32(0x94, 0xFF, 0x5F, 0xFF); // TODO can be cached
        }

        internal void SetNormal()
        {
            Button.GetComponent<Image>().color = new Color32(0xff, 0xFF, 0xfF, 0xFF); // TODO can be cached
        }

        private List<string> GetSceneNames()
        {
            return new List<string>
            {
                Constants.Scenes.BootstrapScene,
                Constants.Scenes.OpenScene,
                Constants.Scenes.MapScene,
                Constants.Scenes.FirstTownScene,
                Constants.Scenes.FirstBattleScene,
                Constants.Scenes.SecondBattleScene,
                Constants.Scenes.ThirdBattleScene,
                Constants.Scenes.FourthBattleSceneDark,
                Constants.Scenes.FifthBattleSceneKingdom,
                Constants.Scenes.SixthBattleSceneKingdom,
                Constants.Scenes.SeventhBattleSceneViking,
                Constants.Scenes.BossFightDarkScene,
                Constants.Scenes.BossFightKingdom1Scene,
                Constants.Scenes.BossFightKingdom2Scene,
                Constants.Scenes.BossFightViking1Scene,
                Constants.Scenes.LibraryScene,
                Constants.Scenes.HollScene,
                Constants.Scenes.EndScene
            };
        }
    }
}