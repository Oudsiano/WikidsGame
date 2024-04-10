using RPG.Combat;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

namespace RPG.Core
{
    public class Health : MonoBehaviour
    {
        public float maxHealth; // Максимальное здоровье существа
        public float currentHealth; // Текущее здоровье существа
        private bool isDead = false; // Флаг, указывающий, что существо мертво
        private bool removed = false; // Флаг, указывающий, что существо было удалено
        public Slider healthBar; // Ссылка на полосу здоровья в пользовательском интерфейсе

        // Примечание: в настоящее время значение здоровья при загрузке новой сцены перезаписывается этим методом
        // из-за порядка выполнения сценариев
        void Start()
        {
            currentHealth = maxHealth; // Устанавливаем текущее здоровье в максимальное значение при старте
            healthBar.value = currentHealth; // Устанавливаем значение полосы здоровья в текущее здоровье
        }

        // Метод для нанесения урона существу
        public void TakeDamage(float damage)
        {
            currentHealth = Mathf.Max(currentHealth - damage, 0); // Уменьшаем текущее здоровье на урон, но не меньше 0
            print("Health of " + currentHealth); // Выводим текущее здоровье в консоль
            healthBar.value = currentHealth; // Обновляем значение полосы здоровья
            if (currentHealth == 0) // Если здоровье достигло нуля, вызываем метод смерти
            {
                Die();
            }
        }


        // Метод для обработки смерти существа
        private void Die()
        {
            GetComponent<Animator>().SetTrigger("dead"); // Устанавливаем триггер смерти для аниматора
            isDead = true; // Устанавливаем флаг "мертв"
            GetComponent<ActionScheduler>().CancelAction(); // Отменяем действие, выполняемое действенным планировщиком
            RemoveProjectiles(); // Удаляем снаряды
            if (gameObject.GetComponent("MainPlayer")){}
            else
            {
                // Разрушаем/деактивируем объект
                if (!removed) // Если объект еще не был удален
                {
                    GetComponent<NavMeshAgent>().enabled = false; // Отключаем компонент навигации
                    removed = true; // Устанавливаем флаг "удален"
                }

                Destroy(this.gameObject, 5f); // Уничтожаем объект через 5 секунд после смерти
            }
        }

        public void Restore()
        {
            currentHealth = maxHealth;
            isDead = false;
        }

        // Метод для проверки, мертво ли существо
        public bool IsDead()
        {
            return isDead; // Возвращаем значение флага "мертв"
        }

        // Метод для захвата состояния существа для сохранения
        public object CaptureState()
        {
            return currentHealth; // Возвращаем текущее здоровье
        }

        // Примечание: в настоящее время значение здоровья при загрузке новой сцены перезаписывается этим методом
        // из-за порядка выполнения сценариев. Измените start на awake, чтобы исправить проблему
        public void RestoreState(object state)
        {
            currentHealth = (float)state; // Восстанавливаем текущее здоровье из сохраненного состояния
            if (currentHealth <= 0) // Если здоровье меньше или равно нулю, вызываем метод смерти
                Die();
        }

        // Метод для удаления снарядов, принадлежащих существу
        public void RemoveProjectiles()
        {
            Projectile[] projectiles = GetComponentsInChildren<Projectile>(); // Получаем все снаряды, находящиеся в дочерних объектах
            foreach (Projectile projectile in projectiles) // Перебираем все снаряды
            {
                Destroy(projectile.gameObject); // Уничтожаем снаряд
                Debug.Log("hello"); // Выводим сообщение в консоль
            }
        }
    }
}
