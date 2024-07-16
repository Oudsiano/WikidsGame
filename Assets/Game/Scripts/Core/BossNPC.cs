using UnityEngine;
using System.Collections.Generic;
using RPG.Core;

public class BossNPC : MonoBehaviour
{
    public List<GameObject> Enemies;
    public Color lineColor = Color.red;
    private Vector3 startPosition;
    private List<LineRenderer> lines = new List<LineRenderer>();
    private NPCInteractable _NPCInteractable;

    private void Awake()
    {
        _NPCInteractable = GetComponent<NPCInteractable>();
        if (_NPCInteractable == null)
        {
            Debug.LogError("У текущего GameObject нет компонента NPCInteractable");
        }

        if (Enemies == null)
        {
            Debug.LogError("В BossNPC нет врагов");
            return;
        }

        foreach (GameObject enemy in Enemies)
        {
            if (enemy.GetComponent<Health>() == null)
            {
                Debug.LogError($"У врага {enemy.name} нет компонента Health");
            }
        }

        startPosition = transform.position;

        foreach (GameObject enemy in Enemies)
        {
            GameObject lineObject = new GameObject("LineRenderer");
            lineObject.transform.SetParent(enemy.transform); 
            LineRenderer line = lineObject.AddComponent<LineRenderer>();

            line.startColor = lineColor;
            line.endColor = lineColor;
            line.startWidth = 0.1f;
            line.endWidth = 0.1f;
            line.positionCount = 0;
            line.material = new Material(Shader.Find("Sprites/Default"));
            lines.Add(line);
        }

        

    }

    private void Update()
    {
        UpdateEnemyStatus();
        for (int i = Enemies.Count - 1; i >= 0; i--)
        {
            if (Enemies[i] != null)
            {
                // Проверяем наличие LineRenderer
                LineRenderer line = lines[i];
                if (line == null || line.gameObject == null)
                {
                    // Если линии нет, удаляем врага из списка
                    Enemies.RemoveAt(i);
                    lines.RemoveAt(i);
                    continue;
                }

                DrawParabolicLine(Enemies[i].transform.position + Vector3.up, startPosition + Vector3.up, line);
            }
            else
            {
                // Если враг был уничтожен, также удаляем из списков
                Enemies.RemoveAt(i);
                lines.RemoveAt(i);
            }
        }
    }
    private void UpdateEnemyStatus()
    {
        bool hasEnemies = false;
        foreach (GameObject enemy in Enemies)
        {
            if (enemy != null)
            {
                hasEnemies = true;
                break;
            }
        }

        if (_NPCInteractable != null)
        {
            _NPCInteractable.NeedKillEnemies = hasEnemies;
        }
    }


    private void DrawParabolicLine(Vector3 enemyPosition, Vector3 bossPosition, LineRenderer line)
    {
        int segments = 20;
        Vector3 controlPoint = (enemyPosition + bossPosition) / 2;
        controlPoint.y += 5;

        line.positionCount = segments + 1;
        for (int i = 0; i <= segments; i++)
        {
            float t = (float)i / segments;
            Vector3 point = CalculateParabolaPoint(enemyPosition, controlPoint, bossPosition, t);
            line.SetPosition(i, point);
        }
    }

    private Vector3 CalculateParabolaPoint(Vector3 start, Vector3 control, Vector3 end, float t)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;
        Vector3 point = uu * start;
        point += 2 * u * t * control;
        point += tt * end;
        return point;
    }
}
