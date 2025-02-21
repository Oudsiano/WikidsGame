using UnityEngine;

namespace Core
{
    public class DestroyAfterEffect : MonoBehaviour
    {
        private void Update() // TODO best practice getcomp in update
        {
            if (GetComponent<ParticleSystem>().IsAlive() == false) // TODO trygetcomp
            {
                Destroy(gameObject);
            }
        }
    }
}
