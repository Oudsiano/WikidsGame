using RPG.Combat;
using System;
using System.Collections;
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
        private int isAtackedInlast5sec = 0;
        private bool isPlayer = false;
        private BossNPC bossNPC; //if exist
        public Slider healthBar; // Ссылка на полосу здоровья в пользовательском интерфейсе

        // Событие, которое будет вызываться при смерти персонажа
        public event Action OnDeath;

        public GameObject redHalfCircle;

        public BossNPC BossNPC { get => bossNPC; set => bossNPC = value; }

        void Start()
        {
            currentHealth = maxHealth; // Устанавливаем текущее здоровье в максимальное значение при старте
            healthBar.value = currentHealth; // Устанавливаем значение полосы здоровья в текущее здоровье

            isPlayer = gameObject.GetComponent<MainPlayer>() != null;

            if (isPlayer)
                StartCoroutine(HeallUpPLayer());
        }

        private IEnumerator HeallUpPLayer()
        {
            while (true)
            {
                if (!isDead)
                {
                    if (isAtackedInlast5sec > 0) isAtackedInlast5sec--;

                    if (isAtackedInlast5sec == 0)
                        Heal(3);
                    else
                        Heal(1);
                }
                yield return new WaitForSeconds(1);
            }
        }

        public void Heal(float heal)
        {
            currentHealth = Mathf.Min(currentHealth + heal, maxHealth);
        }


        public void MissFastTest()
        {
            Fighter fighter = GetComponent<Fighter>();
            if (fighter != null)
            {
                fighter.target = IGame.Instance.playerController.GetHealth();
            }
        }


        public void AttackFromBehind(bool alreadyNeedKill)
        {
            if (!alreadyNeedKill)
            if ((bossNPC != null) && (bossNPC.ShowFastTestIfNeed(this)))
                return;

            TakeDamage(GetCurrentHealth());
        }

        

        // Метод для нанесения урона существу
        public void TakeDamage(float damage)
        {
            if (isPlayer)
            {
                var tempRandom = UnityEngine.Random.Range(0, 9);
                if (tempRandom > 6) //30%
                {
                    Dodge();
                    return;
                }
            }
            currentHealth = Mathf.Max(currentHealth - damage, 0); // Уменьшаем текущее здоровье на урон, но не меньше 0

            if (currentHealth == 0) // Если здоровье достигло нуля, вызываем метод смерти
                Die();

            if (isPlayer)
                isAtackedInlast5sec = 5;
            else
                healthBar.value = currentHealth; // хил бар только у других. У пользователя свой отдельный скрипт
        }

        private void Dodge()
        {
            GetComponent<Animator>().SetTrigger("dodge");
        }

        // Метод для обработки смерти существа
        private void Die()
        {
            if (redHalfCircle != null) redHalfCircle.SetActive(false);

            GetComponent<Animator>().SetTrigger("dead"); // Устанавливаем триггер смерти для аниматора
            isDead = true; // Устанавливаем флаг "мертв"
            GetComponent<ActionScheduler>().CancelAction(); // Отменяем действие, выполняемое действенным планировщиком
            RemoveProjectiles(); // Удаляем снаряды

            // Вызов события при смерти персонажа
            if (OnDeath != null)
            {
                OnDeath();
            }

            if (!isPlayer)
            {
                // Разрушаем/деактивируем объект
                if (!removed) // Если объект еще не был удален
                {
                    GetComponent<NavMeshAgent>().enabled = false; // Отключаем компонент навигации
                    removed = true; // Устанавливаем флаг "удален"
                }

                IGame.Instance.QuestManager.newKill();

                QuestSpecialEnemyName SpecialEnemyName = GetComponent<QuestSpecialEnemyName>();
                if (SpecialEnemyName != null)
                {
                    IGame.Instance.QuestManager.newKill(SpecialEnemyName.specialEnemyName);
                }

                Destroy(healthBar.gameObject);
                LineRenderer lineRenderer = GetComponentInChildren<LineRenderer>();
                if (lineRenderer != null)
                {
                    Destroy(lineRenderer.gameObject);
                }
                Destroy(this.gameObject, 5f); // Уничтожаем объект через 5 секунд после смерти
                IGame.Instance.CoinManager.MakeGoldOnSceneWithCount(25, this.gameObject.transform.position);

                var tempRandom = UnityEngine.Random.Range(0, 9);
                if (tempRandom > 6) //30%
                    IGame.Instance.BottleManager.MakeBottleOnSceneWithCount(25, this.gameObject.transform.position);
            }
            else
            {
                IGame.Instance.UIManager.DeathUI.ShowDeathScreen();
                // Деактивируем необходимые компоненты
                //GetComponent<NavMeshAgent>().enabled = false;
                //GetComponent<Collider>().enabled = false;
                //this.enabled = false; // Отключаем скрипт здоровья
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

        // Добавляем метод для получения текущего здоровья
        public float GetCurrentHealth()
        {
            return currentHealth;
        }
    }
}
