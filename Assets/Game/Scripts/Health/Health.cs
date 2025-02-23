using System;
using System.Collections;
using Combat;
using Core;
using Core.NPC;
using Core.Player;
using Core.Quests;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Health
{
    public class Health : MonoBehaviour
    {
        [FormerlySerializedAs("redHalfCircle")]
        public GameObject RedHalfCircle;

        public Slider healthBar; // Ссылка на полосу здоровья в пользовательском интерфейсе // TODO rename
        public float maxHealth; // Максимальное здоровье существа // TODO rename
        public float currentHealth; // Текущее здоровье существа // TODO rename

        private bool _isDead = false; // Флаг, указывающий, что существо мертво
        private bool _isRemoved = false; // Флаг, указывающий, что существо было удалено
        private int isAtackedInlast5sec = 0; // TODO why int

        private bool _isPlayer = false; // TODO rename
        private BossNPC _bossNPC; // TODO no need here

        public event Action OnDeath; // TODO rename

        public BossNPC BossNPC
        {
            get => _bossNPC;
            set => _bossNPC = value;
        } // TODO no need here

        private void Start() // TODO Construct
        {
            currentHealth = maxHealth; // Устанавливаем текущее здоровье в максимальное значение при старте
            healthBar.value = currentHealth; // Устанавливаем значение полосы здоровья в текущее здоровье

            _isPlayer = gameObject.GetComponent<MainPlayer>() != null; // TODO tryGetComp

            if (_isPlayer)
            {
                StartCoroutine(HeallUpPLayer());
            }
        }

        private IEnumerator HeallUpPLayer()
        {
            while (true) // TODO can be allocated memory
            {
                if (_isDead == false)
                {
                    if (isAtackedInlast5sec > 0)
                    {
                        isAtackedInlast5sec--;
                    }

                    if (isAtackedInlast5sec == 0)
                    {
                        Heal(3);
                    }
                    else
                    {
                        Heal(1);
                    }
                }

                yield return new WaitForSeconds(1); // TODO magic numbers
            }
        }

        public void Heal(float value)
        {
            currentHealth = Mathf.Min(currentHealth + value, maxHealth);
        }


        public void MissFastTest() // TODO rename
        {
            Fighter fighter = GetComponent<Fighter>(); // TODO tryGetComp

            if (fighter != null)
            {
                fighter.Target = IGame.Instance.playerController.GetHealth();
            }
        }


        public void AttackFromBehind(bool alreadyNeedKill)
        {
            if (alreadyNeedKill)
            {
                TakeDamage(GetCurrentHealth());

                return;
            }

            if (_isPlayer == false)
            {
                IGame.Instance.FastTestsManager.WasAttaked(this); // TODO rename
            }
        }

        public void TakeDamage(float value)
        {
            if (_isPlayer)
            {
                var tempRandom = UnityEngine.Random.Range(0, 9);

                if (tempRandom > 6) //30% // TODO magic numbers
                {
                    Dodge();

                    return;
                }
            }

            currentHealth = Mathf.Max(currentHealth - value, 0); // Уменьшаем текущее здоровье на урон, но не меньше 0 

            if (currentHealth == 0) // Если здоровье достигло нуля, вызываем метод смерти
            {
                Die();
            }

            if (_isPlayer)
            {
                isAtackedInlast5sec = 5; // TODO magic numbers
            }
            else
            {
                healthBar.value = currentHealth; // хил бар только у других. У пользователя свой отдельный скрипт
            }
        }

        public void Restore()
        {
            currentHealth = maxHealth;
            _isDead = false;
        }

        public bool IsDead() => _isDead;

        // Метод для захвата состояния существа для сохранения
        public object CaptureState() // TODO not used code
        {
            return currentHealth;
        }

        // Примечание: в настоящее время значение здоровья при загрузке новой сцены перезаписывается этим методом
        // из-за порядка выполнения сценариев. Измените start на awake, чтобы исправить проблему
        public void RestoreState(object state) // TODO not used code
        {
            currentHealth =
                (float)state; // Восстанавливаем текущее здоровье из сохраненного состояния // TODO expensive unboxing
            if (currentHealth <= 0) // Если здоровье меньше или равно нулю, вызываем метод смерти
            {
                Die();
            }
        }

        public float GetCurrentHealth() => currentHealth;

        private void Dodge()
        {
            GetComponent<Animator>().SetTrigger("dodge"); // TODO can be cached
        }

        private void Die() // TODO overload method
        {
            if (RedHalfCircle != null)
            {
                RedHalfCircle.SetActive(false);
            }

            GetComponent<Animator>()
                .SetTrigger(
                    "dead"); // Устанавливаем триггер смерти для аниматора // TODO can be cached // TODO tryGetComp
            _isDead = true;
            GetComponent<ActionScheduler>().Cancel(); // TODO can be cached // TODO tryGetComp
            RemoveProjectiles();

            if (OnDeath != null)
            {
                OnDeath();
            }

            if (_isPlayer == false)
            {
                if (_isRemoved == false)
                {
                    GetComponent<NavMeshAgent>().enabled =
                        false; // Отключаем компонент навигации // TODO can be cached // TODO tryGetComp
                    _isRemoved = true;
                }

                QuestSpecialEnemyName
                    SpecialEnemyName = GetComponent<QuestSpecialEnemyName>(); // TODO can be cached // TODO tryGetComp

                if (SpecialEnemyName != null)
                {
                    IGame.Instance.QuestManager.NewKill(SpecialEnemyName.SpecialEnemyName);
                }
                else
                {
                    IGame.Instance.QuestManager.NewKill();
                }

                if (healthBar != null)
                {
                    Destroy(healthBar.gameObject);
                }

                LineRenderer lineRenderer = GetComponentInChildren<LineRenderer>();

                if (lineRenderer != null)
                {
                    Destroy(lineRenderer.gameObject);
                }

                Destroy(gameObject, 5f); // TODO magic numbers
                IGame.Instance._coinManager.MakeGoldOnSceneWithCount(25,
                    this.gameObject.transform.position); // TODO magic numbers

                var tempRandom = UnityEngine.Random.Range(0, 9); // TODO magic numbers

                if (tempRandom > 6) //30%
                {
                    IGame.Instance._bottleManager.MakeBottleOnSceneWithCount(25,
                        this.gameObject.transform.position); // TODO magic numbers
                }
            }
            else
            {
                IGame.Instance._uiManager.DeathUI.ShowDeathScreen();
                // Деактивируем необходимые компоненты
                //GetComponent<NavMeshAgent>().enabled = false;
                //GetComponent<Collider>().enabled = false;
                //this.enabled = false; // Отключаем скрипт здоровья
            }
        }
        
        private void RemoveProjectiles()
        {
            Projectile[]
                projectiles =
                    GetComponentsInChildren<Projectile>(); // Получаем все снаряды, находящиеся в дочерних объектах // TODO can be cached
            
            foreach (Projectile projectile in projectiles) // Перебираем все снаряды
            {
                Destroy(projectile.gameObject); // Уничтожаем снаряд
                Debug.Log("hello"); // Выводим сообщение в консоль
            }
        }
    }
}