using System;
using UnityEngine;
using UnityEngine.AI;

public class RobotMoveModule : RobotBaseModule
{
    [SerializeField] private NavMeshAgent agent;

    public event Action OnDestinationReached;

    private bool isMoving;

    public override void Init(Robot robot)
    {
        base.Init(robot);

        if (agent == null)
            agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        if (!isMoving)
            return;

        if (!agent.pathPending &&
            agent.remainingDistance <= agent.stoppingDistance &&
            (!agent.hasPath || agent.velocity.sqrMagnitude < 0.01f))
        {
            isMoving = false;
            OnDestinationReached?.Invoke();
        }
    }

    #region Move

    /// <summary>
    /// Di chuyển tới một vị trí.
    /// </summary>
    public bool MoveTo(Vector3 destination)
    {
        if (!agent.isOnNavMesh)
            return false;

        bool success = agent.SetDestination(destination);

        if (success)
            isMoving = true;

        return success;
    }

    /// <summary>
    /// Di chuyển tới Transform.
    /// </summary>
    public bool MoveTo(Transform target)
    {
        if (target == null)
            return false;

        return MoveTo(target.position);
    }

    /// <summary>
    /// Dừng tại chỗ.
    /// </summary>
    public void Stop()
    {
        isMoving = false;
        agent.isStopped = true;
    }

    /// <summary>
    /// Tiếp tục di chuyển.
    /// </summary>
    public void Resume()
    {
        agent.isStopped = false;
    }

    /// <summary>
    /// Hủy đường đi hiện tại.
    /// </summary>
    public void ResetPath()
    {
        isMoving = false;
        agent.ResetPath();
    }

    #endregion

    #region Speed

    public void SetSpeed(float speed)
    {
        agent.speed = speed;
    }

    public void SetAngularSpeed(float angularSpeed)
    {
        agent.angularSpeed = angularSpeed;
    }

    public void SetAcceleration(float acceleration)
    {
        agent.acceleration = acceleration;
    }

    #endregion

    #region State

    public bool IsMoving => isMoving;

    public bool HasPath => agent.hasPath;

    public bool IsStopped => agent.isStopped;

    public float RemainingDistance => agent.remainingDistance;

    public Vector3 Destination => agent.destination;

    public NavMeshPathStatus PathStatus => agent.pathStatus;

    #endregion

    #region Utility

    public bool ReachedDestination()
    {
        return !agent.pathPending &&
               agent.remainingDistance <= agent.stoppingDistance &&
               (!agent.hasPath || agent.velocity.sqrMagnitude < 0.01f);
    }

    public bool CalculatePath(Vector3 destination, out NavMeshPath path)
    {
        path = new NavMeshPath();
        return agent.CalculatePath(destination, path);
    }

    public void Warp(Vector3 position)
    {
        isMoving = false;
        agent.Warp(position);
    }

    #endregion
}