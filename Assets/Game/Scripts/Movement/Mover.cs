using Core;
using Core.Interfaces;
using Core.Player;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

namespace Movement
{
    // Класс, отвечающий за перемещение персонажа и взаимодействие с ним
    public class Mover : MonoBehaviour, IAction
    {
        private Animator _animator;
        private NavMeshAgent _agent;
        private ActionScheduler _actionScheduler; // Ссылка на планировщик действий
        private bool _isPlayer;

        //TODO Public
        [FormerlySerializedAs("clickEffect")] // TODO Remove
        public ClickEffect ClickEffect; // Ссылка на скрипт для создания эффекта при нажатии на точку
        public float StrafeDistance = 3f;
        public NPCInteractable Target;

        private void Start() // TODO Construct
        {
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
            if (_isPlayer == false && _agent.isActiveAndEnabled)
            {
                if (PauseClass.GetPauseState()) // TODO Update
                {
                    _agent.isStopped = true;
                }
                else if (_agent.isStopped)
                {
                    _agent.isStopped = false;
                }
            }

            if (Input.GetKeyDown(KeyCode.Space)) // TODO Update
            {
                Strafe(Vector3.back);
            }

            UpdateAnimator(); // Обновляем состояние аниматора

            // Проверяем, была ли нажата кнопка мыши            
            if (_isPlayer)
                if (Input.GetMouseButtonDown(0))
                {
                    CreateEffectAtMousePosition(); // Создаем эффект в позиции указателя мыши // TODO Expensive 
                }


            //Вынес вызов диалогового окна в сам скрипт NPCInteractable // TODO Expensive Code
            /*
            // Если есть цель для взаимодействия и мы достигли ее, выполняем взаимодействие
            if (target != null)
            {
                if (!ConversationManager.Instance.IsConversationActive)
                    if ((transform.position - target.transform.position).magnitude < 2f)
                    {
                        target.InteractWithNPC(); // Вызываем метод взаимодействия с NPC
                    }
            }*/
        }

        private void Strafe(Vector3 direction)
        {
            // Выполняем уклонение, например, перемещая персонажа в сторону от направления снаряда
            Vector3 strafeDirection = Vector3.Cross(Vector3.up, direction).normalized;
            Vector3 targetPosition =
                transform.position +
                strafeDirection * StrafeDistance; // strafeDistance - расстояние, на которое нужно уклониться

            MoveTo(targetPosition);
        }

        // Метод для начала действия перемещения к определенной точке
        public void StartMoveAction(Vector3 newPosition)
        {
            _actionScheduler.Setup(this); // Устанавливаем текущее действие как перемещение
            MoveTo(newPosition);
        }

        // Метод для перемещения к указанной точке
        public void MoveTo(Vector3 position)
        {
            if (_agent.isActiveAndEnabled == false)
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

            _agent.isStopped = false; // Возобновляем движение
        }

        // Метод для обновления аниматора на основе скорости движения
        private void UpdateAnimator()
        {
            if (_agent == false)
            {
                return;
            }

            Vector3 localVelocity = transform.InverseTransformDirection(_agent.velocity);
            // Устанавливаем значение параметра анимации "forwardSpeed" на основе скорости движения по оси Z
            _animator.SetFloat("forwardSpeed", localVelocity.z); // TODO HardCode
        }

        // Метод для отмены текущего действия
        public void Cancel() // TODO Rename
        {
            if (_agent.isActiveAndEnabled == false)
            {
                return;
            }

            _agent.isStopped = true; // Останавливаем движение
        }

        // Метод для проверки, находимся ли мы в заданной точке с некоторым допуском
        public bool IsAtLocation(float tolerance) // TODO Extensions vector3
        {
            return Vector3.Distance(_agent.destination, transform.position) < tolerance;
        }

        // Метод для восстановления состояния персонажа после загрузки сохранения
        public void RestoreState(object state) // TODO not used
        {
            _actionScheduler = GetComponent<ActionScheduler>(); // Получаем планировщик действий
            _agent = GetComponent<NavMeshAgent>(); // Получаем навигационного агента
            _agent.enabled = false; // Отключаем навигационный агент
            _agent.enabled = true; // Включаем навигационный агент обратно
        }

        // Метод для создания эффекта на поверхности в позиции указателя мыши
        private void CreateEffectAtMousePosition() // TODO Rename
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); // TODO Camera
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                ClickEffect.CreateEffect(hit.point +
                                         new Vector3(0, 0.2f, 0)); // Создаем эффект в точке столкновения луча

                // Если объект, в который попал луч, имеет тег "Interactable", устанавливаем его как цель взаимодействия
                if (hit.transform.CompareTag("Interactable")) // TODO Tag
                {
                    Target = hit.transform.GetComponent<NPCInteractable>();
                }
                else
                {
                    Target = null;
                }

                if (EventSystem.current.IsPointerOverGameObject()) // TODO not used
                {
                    return; // Если да, то выходим из метода
                }
                else
                {
                    AudioManager.instance.PlaySound("Walk");
                }
            }
        }
    }
}