using SceneManagement.Enums;
using UnityEngine;
using UnityEngine.Serialization;

namespace Data
{
    public class DataPlayer : MonoBehaviour
    {
        [FormerlySerializedAs("playerData")] public PlayerData PlayerData = new();

        public void SetSceneToLoad(string sceneName)
        {
            PlayerData.sceneNameToLoad = sceneName;
        }

        public void SavePlayerPosition(int spawn)
        {
            PlayerData.spawnPoint = spawn;
        }

        public int LoadPlayerPosition()
        {
            return PlayerData.spawnPoint;
        }

        public bool IsTestComplete(int idTest)
        {
            if (PlayerData.progress == null || PlayerData.progress.Length == 0)
            {
                return false;
            }

            foreach (OneLeson lesson in PlayerData.progress)
            {
                if (lesson.tests != null)
                {
                    foreach (OneTestQuestion test in lesson.tests)
                    {
                        if (test.id == idTest)
                        {
                            return test.completed;
                        }
                    }
                }
            }

            return false;
        }

        private bool[] ResizeArray(bool[] array, int newSize)
        {
            bool[] newArray = new bool[newSize];
            for (int i = 0; i < Mathf.Min(array.Length, newSize); i++)
            {
                newArray[i] = array[i];
            }

            return newArray;
        }
    }
}