#if UNITY_EDITOR 
using UnityEditor;
using UnityEngine;

// Помечаем, что это кастомный инспектор для OrientationController
[CustomEditor(typeof(OrientationController))]
// Помечаем, что этот компонент поддерживает редактирование сразу нескольких объектов
[CanEditMultipleObjects]
public class OrientationControllerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
				
        // Выводим текущую ориентацию в самом верху ...
        GUILayout.Label("Current orientation: " +
                        (OrientationController.isVertical ? "Vertical" : "Horizontal"));
				
        // ... рисуем стандартный испектор ...
        base.DrawDefaultInspector();

        var controllers = targets;
				
        // ... после отрисовываем кнопку Save ...
        if (GUILayout.Button("Save values"))
            // этот цикл тут для поддержки редактирования нескольких объектов
            foreach(var controller in controllers)
                ((OrientationController)controller).SaveCurrentState();
				
        // ... и после кнопку Put
        if (GUILayout.Button("Put values"))
            // этот цикл тут для поддержки редактирования нескольких объектов
            foreach (var controller in controllers)
                ((OrientationController)controller).PutCurrentState();
				
        // сохраняем изменения
        serializedObject.ApplyModifiedProperties();
    }
}
#endif