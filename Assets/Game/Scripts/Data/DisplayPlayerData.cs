using TMPro;
using UnityEngine;

namespace Data
{
    public class DisplayPlayerData : MonoBehaviour
    {
        public TMP_Text dataText; // TODO rename
        public DataPlayer dataPlayer; // TODO rename

        private void Update()
        {
            if (dataPlayer != null)
            {
                string displayString =
                    $"ID: {dataPlayer.PlayerData.id}\nHealth: {dataPlayer.PlayerData.health}\nIsAlive: {dataPlayer.PlayerData.health}\nSceneToLoad: {dataPlayer.PlayerData.sceneToLoad} \ntestSuccess: {dataPlayer.PlayerData.testSuccess} \nspawnpoint: {dataPlayer.PlayerData.spawnPoint}";
                dataText.text = displayString;
            }
            else
            {
                Debug.LogWarning("DataPlayer reference is not set in DisplayPlayerData script!");
            }
        }
    }
}