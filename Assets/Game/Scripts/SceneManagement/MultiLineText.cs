using UnityEngine;

public class MultiLineText : MonoBehaviour
{
    [TextArea(3, 10)] // Добавляет многострочное текстовое поле в Inspector
    public string[] hoverTexts = new string[] { "Scene1", "Scene2", "Scene3", "Scene4", "Scene5", "Scene6" };
}
