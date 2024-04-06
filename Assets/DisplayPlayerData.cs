using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DisplayPlayerData : MonoBehaviour
{
    public TMP_Text dataText; // Ссылка на текстовый объект
    public DataPlayer dataPlayer; // Ссылка на объект DataPlayer

    // Update is called once per frame
    void Update()
    {
        // Проверяем, что ссылка на объект DataPlayer установлена
        if (dataPlayer != null)
        {
            // Получаем доступ к экземпляру PlayerData из DataPlayer и отображаем данные в текстовом поле
            dataText.text = "Scene to load: " + dataPlayer.playerData.sceneToLoad.ToString() + dataPlayer.playerData.isAlive.ToString() + dataPlayer.playerData.testSuccess.ToString()
               + dataPlayer.playerData.id.ToString()+ dataPlayer.playerData.health.ToString();
        }
        else
        {
            Debug.LogWarning("DataPlayer reference is not set in DisplayPlayerData script!");
        }
    }
}
