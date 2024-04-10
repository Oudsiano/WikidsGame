using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DownloadTestData : MonoBehaviour
{
    [SerializeField] private GameAPI gameAPI;

        public void DownloadData()
    {
        gameAPI = FindObjectOfType<GameAPI>();

        gameAPI.UpdataDataTest();

    }
}
