using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class NPCController : MonoBehaviour
{

    private readonly Destinations destinations;

    private NavMeshAgent navMeshAgent;

    void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.updateRotation = false;
        navMeshAgent.updateUpAxis = false;
    }

    void Update()
    {
        Move();
    }

    private void Move()
    {
        if (ReadyToMove())
        {
            MoveToNextDestination();
        }
    }

    private void MoveToNextDestination()
    {
        navMeshAgent.SetDestination(destinations.Pop());
    }

    private bool ReadyToMove()
    {
        return !navMeshAgent.hasPath || navMeshAgent.isStopped;
    }

    private class Destinations
    {
        private readonly Queue<Vector3> positions = new();

        public Vector3 Pop()
        {
            return positions.Dequeue();
        }

        public void Add(Vector3 position)
        {
            positions.Enqueue(position);
        }
    }
}
