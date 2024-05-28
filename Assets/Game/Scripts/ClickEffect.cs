using UnityEngine;

public class ClickEffect : MonoBehaviour
{
    public ParticleSystem effectPrefab; // Префаб эффекта частиц

    // Метод для создания эффекта в указанной позиции
    public void CreateEffect(Vector3 position)
    {
        Instantiate(effectPrefab, position, Quaternion.identity); // Создаем эффект в указанной позиции
    }
}
