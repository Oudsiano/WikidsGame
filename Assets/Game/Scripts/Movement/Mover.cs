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
    // Класс, отвечающий за перемещение персонажа и взаимодействие с ним
    public class Mover : MonoBehaviour, IAction
    {
        private Camera _camera;
        private Animator _animator;
        private NavMeshAgent _agent;
        private ActionScheduler _actionScheduler; // Ссылка на планировщик действий
        private bool _isPlayer;

        [FormerlySerializedAs("clickEffect")] [SerializeField]
        public ClickEffect ClickEffect; // Ссылка на скрипт для создания эффекта при нажатии на точку

        public float StrafeDistance = 3f;
        public NPCInteractable Target;
        private bool _disableInput = false;
        
        public bool DisableInput =>_disableInput;
        
        private void Start() // TODO Construct
        {
            _camera = UnityEngine.Camera.main;
            _isPlayer = gameObject.GetComponent<MainPlayer>()
                ? true
                : false; //Мувер должен знать на игроке он или нет // TODO MoverPlayer
            _animator = GetComponent<Animator>();

            if (_agent == false)
            {
                _agent = GetComponent<NavMeshAgent>();
            }

            if (_actionScheduler == false)
            {
                _actionScheduler = GetComponent<ActionScheduler>();
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
            
            if (_isPlayer == false && _agent.isActiveAndEnabled && _agent.isOnNavMesh)
            {
                if (PauseClass.GetPauseState())
                {
                    _agent.isStopped = true;
                }
                else if (_agent.isStopped)
                {
                    _agent.isStopped = false;
                }
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                Strafe(Vector3.back);
            }

            UpdateAnimator();

            if (_isPlayer)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    CreateEffectAtMousePosition(); // Создаем эффект в позиции указателя мыши // TODO Expensive 
                }
            }
        }

        public void DeactivateInput() => _disableInput = true;
        public void ActivateInput() => _disableInput = false;

        private void Strafe(Vector3 direction)
        {
            Vector3 strafeDirection = Vector3.Cross(Vector3.up, direction).normalized;
            Vector3 targetPosition =
                transform.position +
                strafeDirection * StrafeDistance;

            MoveTo(targetPosition);
        }

        public void SetupMove(Vector3 newPosition)
        {
            _actionScheduler.Setup(this);
            MoveTo(newPosition);
        }

        public void MoveTo(Vector3 position)
        {
            if (_agent == null || !_agent.isActiveAndEnabled || !_agent.isOnNavMesh)
            {
                return;
            }

            if (_isPlayer)
            {
                NavMeshPath path = new NavMeshPath();
                _agent.CalculatePath(position, path);

                if (path.status == NavMeshPathStatus.PathComplete)
                {
                    _agent.SetPath(path);
                }
            }
            else
            {
                _agent.destination = position;
            }

            _agent.isStopped = false;
            Debug.Log($"[Mover] MoveTo вызван на позицию: {position}");
        }

        private void UpdateAnimator()
        {
            if (_agent == false)
            {
                return;
            }

            Vector3 localVelocity = transform.InverseTransformDirection(_agent.velocity);
            _animator.SetFloat(Constants.Animator.ForwardSpeed, localVelocity.z);
        }

        public void Cancel()
        {
            if (_agent.isActiveAndEnabled == false)
            {
                return;
            }

            if (_agent.isOnNavMesh)
            {
                _agent.isStopped = true;
            }
        }

        public bool IsAtLocation(float tolerance) // TODO Extensions vector3
        {
            return Vector3.Distance(_agent.destination, transform.position) < tolerance;
        }

        public void RestoreState(object state) // TODO not used
        {
            _actionScheduler = GetComponent<ActionScheduler>(); // Получаем планировщик действий
            _agent = GetComponent<NavMeshAgent>(); // Получаем навигационного агента
            _agent.enabled = false; // Отключаем навигационный агент
            _agent.enabled = true; // Включаем навигационный агент обратно
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
    }
}