using UnityEngine;

namespace SceneManagement
{
    public class MultiLineText : MonoBehaviour // TODO check
    {
        [TextArea(3, 10)] public string[] hoverTexts = { "Scene1", "Scene2", "Scene3", "Scene4", "Scene5", "Scene6" };
    }
}