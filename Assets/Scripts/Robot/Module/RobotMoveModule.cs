using System;
using UnityEngine;
using UnityEngine.AI;

public class RobotMoveModule : RobotBaseModule
{
    [SerializeField] private NavMeshAgent agent;

    private Action onReachCallback;

    private bool isMoving;

    public override void Init(Robot robot)
    {
        base.Init(robot);

        if (agent == null)
            agent = GetComponent<NavMeshAgent>();
    }

    public override void Tick()
    {
        base.Tick();

        if (!isMoving)
            return;

        if (!agent.pathPending &&
            agent.remainingDistance <= agent.stoppingDistance &&
            (!agent.hasPath || agent.velocity.sqrMagnitude < 0.01f))
        {
            isMoving = false;
            onReachCallback?.Invoke();
            onReachCallback = null;
        }
    }

    #region Move

    public bool MoveTo(Vector3 destination, Action onReach = null)
    {
        if (!agent.isOnNavMesh)
            return false;

        agent.isStopped = false;

        onReachCallback = onReach;

        bool success = agent.SetDestination(destination);

        if (success)
            isMoving = true;
        else
            onReachCallback = null;

        return success;
    }

    /// <summary>
    /// Hủy
    /// </summary>
    public void Stop()
    {
        isMoving = false;
        agent.isStopped = true;
        agent.ResetPath();
        onReachCallback = null;
    }

    public void Pause()
    {
        if (!agent.hasPath)
            return;

        isMoving = false;
        agent.isStopped = true;
    }

    public void Resume()
    {
        if (!agent.isOnNavMesh)
            return;

        if (!agent.hasPath || agent.pathPending)
            return;

        isMoving = true;
        agent.isStopped = false;
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