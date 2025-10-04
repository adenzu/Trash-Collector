using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class NPCController : MonoBehaviour
{

    [SerializeField]
    private Destinations destinations;

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

    [Serializable]
    private class Destinations
    {
        [SerializeField]
        private Vector3[] initialPositions;

        private readonly Queue<Vector3> positions;

        public Destinations()
        {
            positions = new Queue<Vector3>(initialPositions);
        }

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
