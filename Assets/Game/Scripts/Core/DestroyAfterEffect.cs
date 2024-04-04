using UnityEngine;

namespace RPG.Core
{
    public class DestroyAfterEffect : MonoBehaviour
    {
        // Метод Update вызывается один раз за кадр
        void Update()
        {
            // Проверяем, завершилось ли воспроизведение эффекта частиц
            if (!GetComponent<ParticleSystem>().IsAlive())
            {
                // Если эффект завершился, уничтожаем объект, к которому прикреплен скрипт
                Destroy(gameObject);
            }
        }
    }
}
