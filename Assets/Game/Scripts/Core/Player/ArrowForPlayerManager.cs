using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;

namespace Core.Player
{
    public class ArrowForPlayerManager
    {
        private SortedDictionary<int, ArrowForPlayer> _allArrowForPlayers; // TODO need to use sortedDictionary?

        public SortedDictionary<int, ArrowForPlayer> AllArrowForPlayers
        {
            get => _allArrowForPlayers;
            set => _allArrowForPlayers = value;
        }

        public void Construct() // TODO construct 
        {
            _allArrowForPlayers = new SortedDictionary<int, ArrowForPlayer>();
            SceneManager.sceneLoaded += SceneLoader_LevelChanged;
        }
        
        public void StartArrow()
        {
            List<ArrowForPlayer> sorted = AllArrowForPlayers.Values.ToList(); 
            
            if (sorted.Count > 0)
            {
                sorted[0].ArrowSprite.SetActive(true);
            }
        }
        
        private void SceneLoader_LevelChanged(Scene scene, LoadSceneMode mode)
        {
            _allArrowForPlayers = new SortedDictionary<int, ArrowForPlayer>();
        }
    }
}