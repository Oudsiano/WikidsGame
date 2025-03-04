
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
            public const string MapScene = "ChangeRegion";
            public const string FirstBattleScene = "BattleScene";
            public const string SecondBattleScene = "BattleScene 1";
            public const string LibraryScene = "Library";
            public const string HollScene = "Holl";
            public const string EndScene = "EndScene";
            public const string FirstTownScene = "FirstTownScene";
        }
    }
}