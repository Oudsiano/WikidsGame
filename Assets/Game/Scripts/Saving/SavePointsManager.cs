using System.Collections.Generic;
using Data;

namespace Saving
{
    public class SavePointsManager // TODO rename
    {
        private static Dictionary<int, SavePoint> allSavePoints;

        public static Dictionary<int, SavePoint> AllSavePoints
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

        public static void UpdateStateSpawnPointsAfterLoad(DataPlayer dataPlayer, bool reset = false)
        {
            if (allSavePoints == null)
            {
                return;
            }

            for (int i = 0; i < dataPlayer.PlayerData.stateSpawnPoints.Count; i++)
            {
                bool thisLast = i == dataPlayer.PlayerData.spawnPoint;

                if (allSavePoints.Count > i)
                    if (allSavePoints[i] != null)
                    {
                        allSavePoints[i].SetAlreadyEnabled(dataPlayer.PlayerData.stateSpawnPoints[i], thisLast);
                    }
            }
        }

        public static void ResetDict()
        {
            allSavePoints = new Dictionary<int, SavePoint>();
            IGame.Instance.dataPlayer.PlayerData.stateSpawnPoints = new List<bool> { false };
        }
    }
}