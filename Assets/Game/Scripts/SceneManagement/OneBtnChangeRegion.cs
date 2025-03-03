using System;
using NaughtyAttributes;
using SceneManagement.Enums;
using UnityEngine;
using UnityEngine.UI;

namespace SceneManagement
{
    [Serializable]
    public class OneBtnChangeRegion // TODO rename
    {
        public Button Button;
        public string loadedScene;

        public void SetRed()
        {
            Button.GetComponent<Image>().color = new Color32(0xFF, 0x73, 0x5F, 0xFF); // TODO can be cached
        }

        internal void SetGreen()
        {
            Button.GetComponent<Image>().color = new Color32(0x94, 0xFF, 0x5F, 0xFF); // TODO can be cached
        }

        internal void SetNormal()
        {
            Button.GetComponent<Image>().color = new Color32(0xff, 0xFF, 0xfF, 0xFF); // TODO can be cached
        }
    }
}