using UnityEngine;
using UnityEngine.AI;
using RPG.Core;

namespace RPG.Movement
{
    // Класс, отвечающий за перемещение персонажа и взаимодействие с ним
    public class Mover : MonoBehaviour, IAction
    {
        public ClickEffect clickEffect; // Ссылка на скрипт для создания эффекта при нажатии на точку
        private Animator animator; // Ссылка на компонент аниматора
        private NavMeshAgent thisNavAgent; // Ссылка на компонент навигации
        private ActionScheduler actionScheduler; // Ссылка на планировщик действий

        NPCInteractable target; // Цель для взаимодействия

        // Метод, вызываемый при старте
        void Start()
        {
            // Получаем компоненты, если они не были получены ранее
            if (!thisNavAgent)
                thisNavAgent = GetComponent<NavMeshAgent>();
            if (!actionScheduler)
                actionScheduler = GetComponent<ActionScheduler>();

            animator = GetComponent<Animator>(); // Получаем компонент аниматора
        }

        // Метод, вызываемый каждый кадр
        void Update()
        {
            UpdateAnimator(); // Обновляем состояние аниматора

            // Проверяем, была ли нажата кнопка мыши
            if (Input.GetMouseButtonDown(0))
            {
                CreateEffectAtMousePosition(); // Создаем эффект в позиции указателя мыши
            }

            // Если есть цель для взаимодействия и мы достигли ее, выполняем взаимодействие
            if (target != null)
            {
                if ((transform.position - target.transform.position).magnitude < 1f)
                {
                    target.InteractWithNPC(); // Вызываем метод взаимодействия с NPC
                }
            }
        }

        // Метод для начала действия перемещения к определенной точке
        public void StartMoveAction(Vector3 pos)
        {
            actionScheduler.StartAction(this); // Устанавливаем текущее действие как перемещение
            MoveTo(pos); // Вызываем метод перемещения к заданной точке
             
        }

        // Метод для перемещения к указанной точке
        public void MoveTo(Vector3 pos)
        {
            thisNavAgent.destination = pos; // Устанавливаем пункт назначения для навигационного агента
            thisNavAgent.isStopped = false; // Возобновляем движение
        }

        // Метод для обновления аниматора на основе скорости движения
        private void UpdateAnimator()
        {
            if (!thisNavAgent) return; // Если навигационный агент не установлен, выходим из метода

            // Получаем локальную скорость движения
            Vector3 localVelocity = transform.InverseTransformDirection(thisNavAgent.velocity);
            // Устанавливаем значение параметра анимации "forwardSpeed" на основе скорости движения по оси Z
            animator.SetFloat("forwardSpeed", localVelocity.z);
        }

        // Метод для отмены текущего действия
        public void Cancel()
        {
            thisNavAgent.isStopped = true; // Останавливаем движение
        }

        // Метод для проверки, находимся ли мы в заданной точке с некоторым допуском
        public bool IsAtLocation(float tolerance)
        {
            // Возвращаем true, если расстояние между текущей позицией и пунктом назначения меньше указанного допуска
            return Vector3.Distance(thisNavAgent.destination, transform.position) < tolerance;
        }

        // Метод для восстановления состояния персонажа после загрузки сохранения
        public void RestoreState(object state)
        {
            actionScheduler = GetComponent<ActionScheduler>(); // Получаем планировщик действий
            thisNavAgent = GetComponent<NavMeshAgent>(); // Получаем навигационного агента
            thisNavAgent.enabled = false; // Отключаем навигационный агент
            thisNavAgent.enabled = true; // Включаем навигационный агент обратно
        }

        // Метод для создания эффекта на поверхности в позиции указателя мыши
        private void CreateEffectAtMousePosition()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); // Создаем луч из точки клика мыши
            RaycastHit hit;
            // Проверяем, попал ли луч в какой-либо объект
            if (Physics.Raycast(ray, out hit))
            {
                clickEffect.CreateEffect(hit.point + new Vector3(0, 0.2f, 0)); // Создаем эффект в точке столкновения луча

                // Если объект, в который попал луч, имеет тег "Interactable", устанавливаем его как цель взаимодействия
                if (hit.transform.CompareTag("Interactable"))
                {
                    target = hit.transform.GetComponent<NPCInteractable>(); // Получаем компонент NPCInteractable
                }
                else // Если объект не имеет тег "Interactable", устанавливаем цель взаимодействия как null
                {
                    target = null;
                }
            }
        }
    }
}
