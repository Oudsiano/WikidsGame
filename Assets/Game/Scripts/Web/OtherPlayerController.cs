using UnityEngine;
using UnityEngine.AI;
using Utils;
using System.Collections.Generic;

public class OtherPlayerController : MonoBehaviour
{
    private NavMeshAgent _agent;
    private Animator _animator;
    private bool _ifModularCharacterCreated;
    
    private Queue<Vector3> pathPoints = new Queue<Vector3>();
    private Vector3? currentTarget;

    [SerializeField] private float tolerance = 0.1f;
    
    public bool IfModularCharacterCreated=>_ifModularCharacterCreated;

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();
    }

    /// <summary>
    /// Получает траекторию от сервера и начинает двигаться
    /// </summary>
    public void SetTrajectory(List<Vector3> trajectory)
    {
        pathPoints.Clear();
        foreach (var point in trajectory)
            pathPoints.Enqueue(point);

        SetNextPoint();
    }
    
    public void IsCreatedModularCharacter()
    {
        _ifModularCharacterCreated = true;
    }

    private void SetNextPoint()
    {
        if (pathPoints.Count > 0)
        {
            currentTarget = pathPoints.Dequeue();
            if (_agent != null && _agent.isOnNavMesh)
                _agent.SetDestination(currentTarget.Value);
        }
        else
        {
            currentTarget = null;
        }
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