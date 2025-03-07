using System.Collections.Generic;
using Core.Quests.Data;
using NaughtyAttributes;
using SceneManagement;
using SceneManagement.Enums;
using UnityEngine;
using Utils;

namespace Core.Quests
{
    [System.Serializable]
    public class OneSceneListQuests
    {
        [Dropdown("GetSceneNames")][SerializeField] public string SceneId; // TODO using error
        [SerializeField] public List<OneQuest> QuestsThisScene; // TODO O/C error
        
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
                Constants.Scenes.LibraryScene,
                Constants.Scenes.HollScene,
                Constants.Scenes.EndScene
            };
        }
    }
}