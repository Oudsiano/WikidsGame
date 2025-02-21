using Data;
using UnityEngine;
using UnityEngine.UI;

namespace Health
{
    [RequireComponent(typeof(Health))]
    public class HealthSliderUpdater : MonoBehaviour
    {
        [SerializeField] public DataPlayer data; // TODO rename
        public Slider healthSlider; // TODO rename
        private Health _health;

        private void Start()
        {
            _health = GetComponent<Health>();

            if (healthSlider == null)
            {
                Debug.LogError("Health Slider is not assigned to " + gameObject.name);
            }
            else if (_health == null)
            {
                Debug.LogError("Health component is not assigned to " + gameObject.name);
            }

            data = FindObjectOfType<DataPlayer>(); // TODO find change
        }

        private void Update()
        {
            UpdateHealthSlider();
        }

        private void UpdateHealthSlider()
        {
            if (_health != null && healthSlider != null)
            {
                healthSlider.value = _health.currentHealth / _health.maxHealth;
                data.PlayerData.health = (int)_health.currentHealth;
            }
        }
    }
}