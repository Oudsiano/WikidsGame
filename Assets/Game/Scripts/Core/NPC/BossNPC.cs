using System.Collections.Generic;
using Healths;
using UnityEngine;
using UnityEngine.Serialization;

namespace Core.NPC
{
    public class BossNPC : MonoBehaviour
    {
        private const string SPRITES_DEFAULT_PATH = "Sprites/Default";
        private const string LINE_RENDERER_NAME = "LineRenderer";

        private readonly List<LineRenderer> _lines = new();
        private readonly Color _lineColor = Color.red;
        
        [FormerlySerializedAs("Enemies")][SerializeField] private List<GameObject> _enemies;
        
        private Vector3 _startPosition;
        private NPCInteractable _interactable;

        public void Construct()
        {
            _interactable = GetComponent<NPCInteractable>();

            if (_interactable == null)
            {
                Debug.LogError("У текущего GameObject нет компонента NPCInteractable");
            }

            if (_enemies == null)
            {
                Debug.LogError("В BossNPC нет врагов");
                return;
            }

            foreach (GameObject enemy in _enemies)
            {
                if (enemy.TryGetComponent<Health>(out _) == false) // TODO TryGetComp
                {
                    Debug.LogError($"У врага {enemy.name} нет компонента Healths");
                }
            }

            _startPosition = transform.position;

            foreach (GameObject enemy in _enemies)
            {
                if (enemy.TryGetComponent(out Health health))
                {
                    health.BossNPC = this;

                    GameObject lineObject = new GameObject(LINE_RENDERER_NAME);
                    lineObject.transform.SetParent(enemy.transform);
                    LineRenderer line = lineObject.AddComponent<LineRenderer>();

                    line.startColor = _lineColor;
                    line.endColor = _lineColor;
                    line.startWidth = 0.1f; // TODO magic numbers
                    line.endWidth = 0.1f; // TODO magic numbers
                    line.positionCount = 0; // TODO magic numbers
                    line.material = new Material(Shader.Find(SPRITES_DEFAULT_PATH));
                    _lines.Add(line);
                }
            }
        }

        private void Update()
        {
            UpdateEnemyStatus();

            for (int i = _enemies.Count - 1; i >= 0; i--)
            {
                if (_enemies[i] != null)
                {
                    LineRenderer line = _lines[i];

                    if (line == null || line.gameObject == null)
                    {
                        _enemies.RemoveAt(i);
                        _lines.RemoveAt(i);

                        continue;
                    }

                    DrawParabolicLine(_enemies[i].transform.position + Vector3.up, _startPosition + Vector3.up, line);
                }
                else
                {
                    _enemies.RemoveAt(i);
                    _lines.RemoveAt(i);
                }
            }
        }

        private void UpdateEnemyStatus()
        {
            bool hasEnemies = false;

            foreach (GameObject enemy in _enemies) // TODO GO
            {
                if (enemy != null)
                {
                    hasEnemies = true;

                    break;
                }
            }

            if (_interactable != null)
            {
                _interactable.NeedKillEnemies = hasEnemies;
            }
        }

        #region Line

        private void
            DrawParabolicLine(Vector3 enemyPosition, Vector3 bossPosition, LineRenderer line) // TODO magic numbers
        {
            int segments = 20;
            Vector3 controlPoint = (enemyPosition + bossPosition) / 2;
            controlPoint.y += 5;

            line.positionCount = segments + 1;

            for (int i = 0; i <= segments; i++)
            {
                float t = (float)i / segments; // TODO expensive unboxing
                Vector3 point = CalculateParabolaPoint(enemyPosition, controlPoint, bossPosition, t);
                line.SetPosition(i, point);
            }
        }

        private Vector3
            CalculateParabolaPoint(Vector3 start, Vector3 control, Vector3 end, float t) // TODO magic numbers
        {
            float u = 1 - t;
            float tt = t * t;
            float uu = u * u;
            Vector3 point = uu * start;
            point += 2 * u * t * control;
            point += tt * end;

            return point;
        }

        #endregion
    }
}