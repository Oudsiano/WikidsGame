using UnityEngine;
using UnityEngine.AI;
using Utils;

public class OtherPlayerController : MonoBehaviour
{
    private NavMeshAgent _agent;
    private Animator _animator;
    private bool _ifModularCharacterCreated;
    
    public bool IfModularCharacterCreated=>_ifModularCharacterCreated;

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();
    }

    /// <summary>
    /// Обновление позиции, полученной с сервера
    /// </summary>
    public void SetPosition(Vector3 newPosition)
    {
        if (_agent == null || !_agent.isOnNavMesh) return;

        // Двигаем к позиции (можно заменить на Lerp или интерполяцию)
        _agent.SetDestination(newPosition);
        Debug.Log("Новая позиция: " + transform.position);
    }

    public void IsCreatedModularCharacter()
    {
        _ifModularCharacterCreated = true;
    }

    private void Update()
    {
        // Обновление анимации ходьбы
        if (_agent != null && _animator != null)
        {
            Vector3 localVelocity = transform.InverseTransformDirection(_agent.velocity);
            _animator.SetFloat(Constants.Animator.ForwardSpeed, localVelocity.z);// 👈 Имя параметра должно совпадать с аниматором
        }
    }
    
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * 2f);
    }
}