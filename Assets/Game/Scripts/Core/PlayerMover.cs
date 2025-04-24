using Core;
using Core.Interfaces;
using Core.Player;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using Utils;
using System.Collections.Generic;

namespace Movement
{
public class PlayerMover : Mover
{
        private Camera _camera;
        private SocketManager _socketManager;

        [FormerlySerializedAs("clickEffect")] [SerializeField]
        public ClickEffect ClickEffect; // Ссылка на скрипт для создания эффекта при нажатии на точку
        

        public float StrafeDistance = 3f;
        [SerializeField] private LineRenderer lineRenderer; // Для отображения траектории
        [SerializeField] private float trajectoryPointDistance = 1.0f; // Минимальное расстояние между точками траектории
        [SerializeField] private float heightOffset = 0.2f; // Смещение траектории по высоте
        
        private string _playerId;
        private List<Vector3> trajectoryPoints = new List<Vector3>();
        private bool isDrawingTrajectory = false; // Флаг рисования траектории
        private Vector3 lastPoint; // Последняя добавленная точка
        private int currentWaypointIndex; // Индекс текущей точки пути
        
        public bool IsDrawingTrajectory => isDrawingTrajectory;
        
        public void Construct(SocketManager socketManager)
        {
            base.Construct();
            _camera = Camera.main;
            _socketManager =  socketManager;
            
            if (lineRenderer == null)
            {
                GameObject lineObject = new GameObject("TrajectoryLine");
                lineObject.transform.SetParent(transform);
                lineRenderer = lineObject.AddComponent<LineRenderer>();
                lineRenderer.startWidth = 0.1f;
                lineRenderer.endWidth = 0.1f;
                lineRenderer.positionCount = 0;
                lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
                lineRenderer.startColor = Color.green;
                lineRenderer.endColor = Color.green;
            }
        }
        
        private void Update()
        {
            if (_disableInput)
            {
                return;
            }
            
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }
            

            if (Input.GetKeyDown(KeyCode.Space))
            {
                Strafe(Vector3.back);
            }

            UpdateAnimator();
            
            // if (Input.GetMouseButtonDown(0))
            // {
            //     CreateEffectAtMousePosition(); // Создаем эффект в позиции указателя мыши // TODO Expensive 
            // }
            
            if (Input.GetMouseButtonDown(0))
            {
                StartDrawingTrajectory();
            }

            // Продолжение рисования траектории
            if (Input.GetMouseButton(0))
            {
                ContinueDrawingTrajectory();
            }

            // Завершение рисования и начало движения
            if (Input.GetMouseButtonUp(0))
            {
                StopDrawingTrajectoryAndMove();
            }

            // Движение по траектории
            if (trajectoryPoints.Count > 0 && currentWaypointIndex < trajectoryPoints.Count)
            {
                MoveAlongTrajectory();
            }

        }
        
        private void StartDrawingTrajectory()
        {
            Debug.Log("StartDrawingTrajectory");
            Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                // Очищаем предыдущую траекторию
                trajectoryPoints.Clear();
                currentWaypointIndex = 0;
                lineRenderer.positionCount = 0;

                // Начинаем рисовать траекторию
                isDrawingTrajectory = true;
                Vector3 startPoint = hit.point + new Vector3(0, heightOffset, 0);
                trajectoryPoints.Add(startPoint);
                lastPoint = startPoint;

                // Отображаем начальную точку
                lineRenderer.positionCount = 1;
                lineRenderer.SetPosition(0, startPoint);
            }
        }

        private void ContinueDrawingTrajectory()
        {
            if (!isDrawingTrajectory)
            {
                return;
            }

            Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                Vector3 currentPoint = hit.point + new Vector3(0, heightOffset, 0);

                // Добавляем новую точку только если расстояние достаточно большое
                if (Vector3.Distance(lastPoint, currentPoint) >= trajectoryPointDistance)
                {
                    trajectoryPoints.Add(currentPoint);
                    lastPoint = currentPoint;

                    // Обновляем LineRenderer
                    lineRenderer.positionCount = trajectoryPoints.Count;
                    lineRenderer.SetPosition(trajectoryPoints.Count - 1, currentPoint);
                }
            }
        }

        private void StopDrawingTrajectoryAndMove()
        {
            isDrawingTrajectory = false;
            Debug.Log("StopDrawingTrajectory");
        }
        
        private void MoveAlongTrajectory()
        {
            if (_agent == null || !_agent.isActiveAndEnabled || !_agent.isOnNavMesh)
            {
                return;
            }

            // Проверяем, достиг ли герой текущей точки пути
            if (!_agent.pathPending && _agent.remainingDistance <= _agent.stoppingDistance)
            {
                currentWaypointIndex++;
                if (currentWaypointIndex < trajectoryPoints.Count)
                {
                    // Переходим к следующей точке
                    MoveTo(trajectoryPoints[currentWaypointIndex]);
                }
                else
                {
                    // Достигли конца траектории
                    _agent.isStopped = true;
                    _agent.ResetPath();
                    trajectoryPoints.Clear();
                    lineRenderer.positionCount = 0;
                    Target = null; // Очищаем цель
                }
            }
        }
        
        private void Strafe(Vector3 direction)
        {
            Vector3 strafeDirection = Vector3.Cross(Vector3.up, direction).normalized;
            Vector3 targetPosition =
                transform.position +
                strafeDirection * StrafeDistance;

            MoveTo(targetPosition);
        }
        
        public override void MoveTo(Vector3 position)
        {
            if (isDrawingTrajectory)
            {
                Debug.Log("Trajectory is drawing, can't move");
                return;
            }
            
            if (_agent == null || !_agent.isActiveAndEnabled || !_agent.isOnNavMesh)
            {
                return;
            }
                NavMeshPath path = new NavMeshPath();
                _agent.CalculatePath(position, path);
            
                if (path.status == NavMeshPathStatus.PathComplete)
                {
                    _agent.SetPath(path);
                     SendPlayerPosition(position);
                }
                
            _agent.isStopped = false;
        }
        
        private void CreateEffectAtMousePosition() // TODO Rename
        {
            Ray ray = _camera.ScreenPointToRay(Input.mousePosition); // TODO Camera
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                ClickEffect.CreateEffect(hit.point +
                                          new Vector3(0, 0.2f, 0)); // Создаем эффект в точке столкновения луча

                // Если объект, в который попал луч, имеет тег "Interactable", устанавливаем его как цель взаимодействия
                Target = hit.transform.CompareTag("Interactable")
                    ? hit.transform.GetComponent<NPCInteractable>()
                    : null; // TODO Tag

                if (EventSystem.current.IsPointerOverGameObject()) // TODO not used
                {
                    return; // Если да, то выходим из метода
                }
                else
                {
                    AudioManager.Instance.PlaySound("Walk");
                }
            }
        }
        
        private void SendPlayerPosition(Vector3 targetPosition)
        {
            Debug.Log("SendPlayerPosition works");
            if (_socketManager != null && !string.IsNullOrEmpty(_playerId))
            {
                var data = new PlayerNetworkPositionData
                {
                    id = _playerId,
                    x = targetPosition.x,
                    y = targetPosition.y,
                    z = targetPosition.z
                };

                string json = JsonUtility.ToJson(data);
                Debug.Log($"📤 Отправка позиции: {json}");
                _socketManager.SendData(json);
            }
        }
        
        public void SetPlayerId(string id)
        {
            Debug.Log("_playerId="+ _playerId);
            _playerId = id;
            // SendPlayerPosition();
        }
    }
}

