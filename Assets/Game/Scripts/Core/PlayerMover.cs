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
        [SerializeField] private float moveSpeed = 20f;
        [SerializeField] private LineRenderer lineRenderer; // Для отображения траектории
        [SerializeField] private float trajectoryPointDistance = 1.0f; // Минимальное расстояние между точками траектории
        [SerializeField] private float heightOffset = 0.5f; // Смещение траектории по высоте
        [SerializeField] private float maxStepHeight = 3.5f;
        [SerializeField] private LayerMask obstacleLayer; // Слой для препятствий
        
        private string _playerId;
        private List<Vector3> trajectoryPoints = new List<Vector3>();
        private bool isDrawingTrajectory = false; // Флаг рисования траектории
        private Vector3 lastPoint; // Последняя добавленная точка
        private int currentWaypointIndex; // Индекс текущей точки пути
        private bool isMoving = false;
        private Vector3 lastPosition;
        
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
                Debug.Log("Я двигаюсь по траектории");
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
                isMoving = false;
                // _agent.isStopped = true;
                // _agent.ResetPath();
                

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
                
                NavMeshHit navMeshHit;
                
                if (NavMesh.SamplePosition(currentPoint, out navMeshHit, 1.0f, NavMesh.AllAreas))
                {
                    currentPoint = navMeshHit.position; // Корректируем точку на ближайшую позицию на NavMesh
                }
                else
                {
                    Debug.LogWarning($"Point {currentPoint} is not on NavMesh, skipping");
                    return;
                }

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

            OnDrawGizmos();
        }
        
        
        public void MoveAlongTrajectory()
        {
            Debug.Log("MoveAlongTrajectory");
            
            if (isDrawingTrajectory)
            {
                return;
            }
            
            if (trajectoryPoints.Count == 0 || currentWaypointIndex >= trajectoryPoints.Count)
            {
                Debug.Log("No trajectory points or reached end");
                trajectoryPoints.Clear();
                lineRenderer.positionCount = 0;
                isMoving = false;
                Target = null;
                return;
            }
            
            Vector3 targetPosition = trajectoryPoints[currentWaypointIndex];
            
            // Проверяем, есть ли препятствие между текущей позицией и целевой точкой
            Vector3 direction = (targetPosition - transform.position).normalized;
            float distance = Vector3.Distance(transform.position, targetPosition);
            
            if (Physics.Raycast(transform.position, direction, distance, obstacleLayer))
            {
                Debug.Log("Obstacle detected, stopping movement");
                trajectoryPoints.Clear();
                lineRenderer.positionCount = 0;
                isMoving = false;
                Target = null;
                return;
            }
            
            // Корректируем высоту, чтобы персонаж мог подниматься по неровностям
            Vector3 adjustedTargetPosition = targetPosition;
            RaycastHit heightHit;
            if (Physics.Raycast(targetPosition + Vector3.up * 1f, Vector3.down, out heightHit, maxStepHeight + 1f))
            {
                adjustedTargetPosition.y = heightHit.point.y;
            }
            
            float step = moveSpeed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, adjustedTargetPosition, step);
            
            // Поворачиваем персонажа в сторону движения
            direction = (adjustedTargetPosition - transform.position).normalized;
            if (direction != Vector3.zero)
            {
                Quaternion lookRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
            }
            
            if (Vector3.Distance(transform.position, adjustedTargetPosition) < 0.1f)
            {
                currentWaypointIndex++;
                if (currentWaypointIndex >= trajectoryPoints.Count)
                {
                    Debug.Log("Reached end of trajectory");
                    trajectoryPoints.Clear();
                    lineRenderer.positionCount = 0;
                    isMoving = false;
                    Target = null;
                }
                else
                {
                    Debug.Log($"Moving to waypoint {currentWaypointIndex}/{trajectoryPoints.Count}: {trajectoryPoints[currentWaypointIndex]}");
                }
            }
            
            AudioManager.Instance.PlaySound("Walk");
            // SoundManager.PlaySound("Walk");
            
            // if (isDrawingTrajectory)
            // {
            //     return;
            // }
            //
            // Debug.Log("MoveAlongTrajectory");
            //
            //
            // if(!_agent.pathPending && _agent.remainingDistance < 0.4f)
            // {
            //     currentWaypointIndex++;
            //
            //     Debug.Log("currentWaypointIndex=" + currentWaypointIndex);
            //
            //     if (currentWaypointIndex < trajectoryPoints.Count)
            //     {
            //         _agent.SetDestination(trajectoryPoints[currentWaypointIndex]);
            //         Debug.Log("Player Moves to: " + trajectoryPoints[currentWaypointIndex]);
            //     }
            //     else
            //     {
            //         Debug.Log("Reached end of trajectory, clearing LineRenderer");
            //         trajectoryPoints.Clear();
            //         lineRenderer.positionCount = 0; // Очищаем LineRenderer, чтобы линия исчезла
            //         isMoving = false;
            //         _agent.isStopped = true;
            //         _agent.ResetPath();
            //     }
            // }
            // else
            // {
            //     Debug.Log($"Agent position: {transform.position}, Destination: {_agent.destination}, Velocity: {_agent.velocity}");
            //
            //     // Если агент остановился, пытаемся перезапустить движение
            //     if (_agent.isStopped && currentWaypointIndex < trajectoryPoints.Count)
            //     {
            //         Debug.LogWarning("Agent is stopped, restarting movement");
            //         MoveTo(trajectoryPoints[currentWaypointIndex]);
            //     }
            // }
        }
        
        private void UpdateAnimator()
        {
            Vector3 velocity = (transform.position - lastPosition) / Time.deltaTime;
            lastPosition = transform.position;
            Vector3 localVelocity = transform.InverseTransformDirection(velocity);
            _animator.SetFloat(Constants.Animator.ForwardSpeed, localVelocity.z);
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
            // if (isDrawingTrajectory)
            // {
            //     Debug.Log("Trajectory is drawing, can't move");
            //     return;
            // }
            //
            // if (_agent == null || !_agent.isActiveAndEnabled || !_agent.isOnNavMesh)
            // {
            //     return;
            // }
            //     NavMeshPath path = new NavMeshPath();
            //     _agent.CalculatePath(position, path);
            //
            //     if (path.status == NavMeshPathStatus.PathComplete)
            //     {
            //         _agent.SetPath(path);
            //          SendPlayerPosition(position);
            //     }
            //     
            // // _agent.isStopped = false;
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
                    // SoundManager.PlaySound("Walk");
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
        
        private void OnDrawGizmos()
        {
            foreach (Vector3 point in trajectoryPoints)
            {
                if (trajectoryPoints.Count > 0 && currentWaypointIndex < trajectoryPoints.Count)
                {
                    // Рисуем красную сферу в текущей точке
                    Gizmos.color = Color.red;
                    Gizmos.DrawWireSphere(point, 0.3f); // Радиус сферы 0.3
                }
            }
            
            // Gizmos.color = Color.black;
            // Gizmos.DrawWireSphere(trajectoryPoints[currentWaypointIndex], 0.3f); // Радиус сферы 0.3
        }
    }
}

