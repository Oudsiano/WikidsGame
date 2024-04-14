using RPG.Combat;
using UnityEngine;
using RPG.Core;
using RPG.Movement;

namespace RPG.Controller
{
    // Класс, отвечающий за управление искусственным интеллектом (ИИ)
    public class AIController : MonoBehaviour
    {
        // Параметры ИИ
        [SerializeField] private float chaseDistance = 5f; // Дистанция преследования игрока
        [SerializeField] private float suspicionTimer = 10f; // Время подозрения
        [SerializeField] private PatrolPath patrolPath; // Путь патрулирования
        private int currentWayPointIndex = 0; // Индекс текущей точки пути
        [SerializeField] private float tolerance = 1f; // Допуск для определения достижения точки

        // Время пребывания в каждой точке пути
        [SerializeField] private float minDwellTime;
        [SerializeField] private float maxDwellTime;

        [SerializeField] private float currentDwellTime; // Время пребывания в текущей точке (для отладки)

        // Переменные ИИ
        private Vector3 lastKnownLocation; // Последнее известное местоположение игрока
        private Vector3 guardLocation; // Местоположение патруля
        private Quaternion guardRotation; // Начальное вращение патруля
        [SerializeField] private float timeSinceLastSawPlayer = Mathf.Infinity; // Время с момента последней видимости игрока

        // Кэшированные компоненты
        private Fighter fighter;
        private Mover mover;
        private Health health;

        void Awake()
        {
            fighter = GetComponent<Fighter>(); // Получаем компонент Fighter
            mover = GetComponent<Mover>(); // Получаем компонент Mover
            health = GetComponent<Health>(); // Получаем компонент Health

            guardLocation = transform.position; // Устанавливаем начальное местоположение патруля
            guardRotation = transform.rotation; // Устанавливаем начальное вращение патруля
        }

        void Update()
        {
            if (health.IsDead())
                return;

            if (DistanceToPlayer() < 40)
            {
                InteractWithCombat(); // Взаимодействие с боем (игроком)
            }

            timeSinceLastSawPlayer += Time.deltaTime; // Обновляем время с момента последней видимости игрока
        }

        // Расстояние до игрока
        private float DistanceToPlayer()
        {
            return Mathf.Abs(Vector3.Distance(MainPlayer.Instance.transform.position, transform.position));
        }

        // Взаимодействие с боем (игроком)
        private void InteractWithCombat()
        {
            // Если игрок в пределах дистанции преследования и возможно атаковать
            if (DistanceToPlayer() <= chaseDistance && fighter.CanAttack(MainPlayer.Instance.gameObject) || IsAttacked())
            {
                AttackBehavior(); // Атака игрока
            }
            else if (suspicionTimer > timeSinceLastSawPlayer)
            {
                SuspicionBehavior(); // Поведение при подозрении
            }
            else
            {
                PatrolBehavior(); // Патрулирование
            }
        }

        // Проверка на атаку со стороны игрока
        private bool IsAttacked()
        {
            var player = MainPlayer.Instance;
            bool isAttacked = player.GetComponent<Fighter>().target == this.gameObject.GetComponent<Health>();
            return (isAttacked);
        }

        // Отрисовка гизмоны для обозначения дистанции преследования
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, chaseDistance);
        }

        // Поведение при патрулировании
        private void PatrolBehavior()
        {
            Vector3 nextPos = guardLocation; // Следующая позиция - начальное местоположение патруля

            // Если есть путь патрулирования
            if (patrolPath)
            {
                // Если достигли текущей точки пути
                if (AtWayPoint())
                {
                    // Уменьшаем время пребывания в текущей точке
                    if (currentDwellTime > 0)
                        currentDwellTime -= Time.deltaTime;
                    else
                        GoToNextWayPoint(); // Переходим к следующей точке пути
                }

                nextPos = GetCurrentWayPoint(); // Получаем позицию текущей точки пути
            }

            mover.StartMoveAction(nextPos); // Начинаем движение к следующей позиции

            // Устанавливаем вращение патрулирующего объекта в начальное положение, если достигли точки
            if (!patrolPath && mover.IsAtLocation(tolerance))
            {
                transform.rotation = guardRotation;
            }
        }

        // Переход к следующей точке пути

        private void GoToNextWayPoint()
        {
            if (currentWayPointIndex < patrolPath.transform.childCount)
            {
                currentWayPointIndex++;
            }

            if (currentWayPointIndex == patrolPath.transform.childCount)
            {
                currentWayPointIndex = 0;
            }

            currentDwellTime = Random.Range(minDwellTime, maxDwellTime); // Случайное время пребывания в следующей точке
        }

        // Проверка на достижение текущей точки пути
        private bool AtWayPoint()
        {
            return Vector3.Distance(transform.position, patrolPath.transform.GetChild(currentWayPointIndex).position) < tolerance;
        }

        // Получение текущей точки пути
        private Vector3 GetCurrentWayPoint()
        {
            return patrolPath.transform.GetChild(currentWayPointIndex).position;
        }

        // Поведение при подозрении
        private void SuspicionBehavior()
        {
            mover.StartMoveAction(lastKnownLocation); // Начинаем движение к последнему местоположению игрока
        }

        // Поведение при атаке игрока
        private void AttackBehavior()
        {
            timeSinceLastSawPlayer = 0; // Обнуляем время с момента последней видимости игрока
            fighter.Attack(MainPlayer.Instance.gameObject); // Атакуем игрока
            lastKnownLocation = MainPlayer.Instance.transform.position; // Запоминаем последнее местоположение игрока
        }
    }
}