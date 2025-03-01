using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Serialization;

namespace SceneManagement
{
    public class SceneLoaderService : MonoBehaviour
    {
        [Scene] [SerializeField] private int _openScene;
        [Scene] [SerializeField] private int _mapScene;
        [Scene] [SerializeField] private int _firstBattleScene;
        [Scene] [SerializeField] private int _secondBattleScene;
        [Scene] [SerializeField] private int _thirdBattleScene;
        [Scene] [SerializeField] private int _librarySceneScene;
        [Scene] [SerializeField] private int _hollSceneScene;
        [Scene] [SerializeField] private int _firstTownSceneScene;


        public int OpenScene => _openScene;
        public int MapScene => _mapScene;
        public int FirstBattleScene => _firstBattleScene;
        public int SecondBattleScene => _secondBattleScene;
        public int ThirdBattleScene => _thirdBattleScene;
        public int LibraryScene => _librarySceneScene;
        public int HollScene => _hollSceneScene;
        public int FirstTownScene => _firstTownSceneScene;
    }
}