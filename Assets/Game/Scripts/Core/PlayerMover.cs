using Core;
using Core.Interfaces;
using Core.Player;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using Utils;

namespace Movement
{
public class PlayerMover : Mover
{
        private Camera _camera;
        private SocketManager _socketManager;

        [FormerlySerializedAs("clickEffect")] [SerializeField]
        public ClickEffect ClickEffect; // Ссылка на скрипт для создания эффекта при нажатии на точку

        public float StrafeDistance = 3f;
        
        private string _playerId;
        
        public void Construct(SocketManager socketManager)
        {
            base.Construct();
            _camera = Camera.main;
            _socketManager =  socketManager;
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
            
            if (Input.GetMouseButtonDown(0))
            {
                CreateEffectAtMousePosition(); // Создаем эффект в позиции указателя мыши // TODO Expensive 
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
            if (_agent == null || !_agent.isActiveAndEnabled || !_agent.isOnNavMesh)
            {
                return;
            }
                NavMeshPath path = new NavMeshPath();
                _agent.CalculatePath(position, path);

                if (path.status == NavMeshPathStatus.PathComplete)
                {
                    _agent.SetPath(path);
                    SendPlayerPosition();
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
        
        private void SendPlayerPosition()
        {
            if (_socketManager != null && !string.IsNullOrEmpty(_playerId))
            {
                var data = new PlayerNetworkPositionData
                {
                    id = _playerId,
                    x = transform.position.x,
                    y = transform.position.y,
                    z = transform.position.z
                };

                string json = JsonUtility.ToJson(data);
                Debug.Log($"📤 Отправка позиции: {json}");
                _socketManager.SendData(json);
            }
        }
        
        public void SetPlayerId(string id)
        {
            _playerId = id;
        }
    }
}

