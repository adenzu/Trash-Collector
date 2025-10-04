using System;
using System.Collections.Generic;
using System.Linq;
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

    void Start()
    {
        destinations.Initialize();
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
        navMeshAgent.SetDestination(destinations.Next());
    }

    private bool ReadyToMove()
    {
        return !destinations.Finished() && (!navMeshAgent.hasPath || navMeshAgent.isStopped);
    }

    [Serializable]
    private class Destinations
    {
        [SerializeField]
        private Transform[] initialPositions;

        private Queue<Vector3> positions;

        public void Initialize()
        {
            positions = new Queue<Vector3>(initialPositions.Select(transform => transform.position));
        }

        public Vector3 Next()
        {
            return positions.Dequeue();
        }

        public void Add(Vector3 position)
        {
            positions.Enqueue(position);
        }

        public bool Finished()
        {
            return 0 == positions.Count;
        }
    }
}
