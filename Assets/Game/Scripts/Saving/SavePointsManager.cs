using System.Collections.Generic;
using Data;

namespace Saving
{
    public class SavePointsManager // TODO rename
    {
        private static Dictionary<int, SavePoint> allSavePoints;
        private DataPlayer _dataPlayer;

        public Dictionary<int, SavePoint> AllSavePoints
        {
            get
            {
                if (allSavePoints == null)
                {
                    allSavePoints = new Dictionary<int, SavePoint>();
                }

                return allSavePoints;
            }
            set => allSavePoints = value;
        }

        public void Construct(DataPlayer dataPlayer)
        {
            _dataPlayer = dataPlayer;
        }

        public void UpdateStateSpawnPointsAfterLoad(bool reset = false) // TODO change
        {
            if (allSavePoints == null)
            {
                return;
            }

            for (int i = 0; i < _dataPlayer.PlayerData.stateSpawnPoints.Count; i++)
            {
                bool thisLast = i == _dataPlayer.PlayerData.spawnPoint;

                if (allSavePoints.Count > i)
                {
                    if (allSavePoints[i] != null)
                    {
                        allSavePoints[i].SetAlreadyEnabled(_dataPlayer.PlayerData.stateSpawnPoints[i], thisLast);
                    }
                }
            }
        }

        public void ResetDict() // TODO change
        {
            allSavePoints = new Dictionary<int, SavePoint>();
            _dataPlayer.PlayerData.stateSpawnPoints = new List<bool> { false };
        }
    }
}