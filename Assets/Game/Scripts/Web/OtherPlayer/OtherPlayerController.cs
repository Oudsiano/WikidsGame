using UnityEngine;
using UnityEngine.AI;
using Utils;
using System.Collections.Generic;

public class OtherPlayerController : MonoBehaviour
{
    private NavMeshAgent _agent;
    private Animator _animator;

    
    private Queue<Vector3> pathPoints = new Queue<Vector3>();
    private Vector3? currentTarget;

    [SerializeField] private float tolerance = 0.1f;
    


    public void Construct()
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
    
    public void Move(Vector3 position)
    {
        if (_agent != null && _agent.isOnNavMesh)
        {
            _agent.SetDestination(position);
        }
    }
    
    
    private void SetNextPoint()
    {
        if (pathPoints.Count > 0)
        {
            currentTarget = pathPoints.Dequeue();
            Debug.Log($"[OtherPlayerController] SetNextPoint: Moving to {currentTarget.Value}, remaining points = {pathPoints.Count}");
            
            if (_agent != null && _agent.isOnNavMesh)
                _agent.SetDestination(currentTarget.Value);
        }
        else
        {
            Debug.Log("[OtherPlayerController] Trajectory finished.");
            currentTarget = null;
        }
    }

    private void Update()
    {
        // if (currentTarget.HasValue && _agent != null && _agent.isOnNavMesh)
        // {
        //     float distance = Vector3.Distance(transform.position, currentTarget.Value);
        //     Debug.Log($"[OtherPlayerController] Moving to target {currentTarget.Value}, distance = {distance}, remainingDistance = {_agent.remainingDistance}, pathPending = {_agent.pathPending}");
        //     
        //     if (distance <= tolerance)
        //     {
        //         Debug.Log("[OtherPlayerController] Target reached, moving to next point.");
        //         SetNextPoint();
        //     }
        // }
        
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