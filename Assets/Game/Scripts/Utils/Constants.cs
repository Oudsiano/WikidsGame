using System.Collections.Generic;

namespace Utils
{
    public sealed class Constants
    {
        public sealed class Animator
        {
            public static readonly int ForwardSpeed = UnityEngine.Animator.StringToHash("forwardSpeed");
        }

        public sealed class Assets
        {
            public const string LoadingScreen = "LoadingScreen";
        }

        public sealed class Scenes
        {
            public const string BootstrapScene = "BootstrapScene";
            public const string OpenScene = "OpenScene";
            public const string MapScene = "MapScene";
            public const string GamePlayScene = "GamePlayScene";

            public const string FirstBattleScene = "FirstBattleScene";
            public const string SecondBattleScene = "SecondBattleScene";
            public const string ThirdBattleScene = "ThirdBattleScene";
            public const string FourthBattleSceneDark = "FourthBattleSceneDark";
            public const string FifthBattleSceneKingdom = "FifthBattleSceneKingdom";
            public const string SixthBattleSceneKingdom = "SixthBattleSceneKingdom";
            public const string SeventhBattleSceneViking = "SeventhBattleSceneViking";

            public const string FirstTownScene = "FirstTownScene";
            public const string BossFightDarkScene = "BossFightDarkScene";
            public const string BossFightKingdom1Scene = "BossFightKingdom1Scene";
            public const string BossFightKingdom2Scene = "BossFightKingdom2Scene";
            public const string BossFightViking1Scene = "BossFightViking1Scene";
            public const string LibraryScene = "LibraryScene";
            public const string HollScene = "HollScene";

            public const string EndScene = "EndScene";
        }
    }
}