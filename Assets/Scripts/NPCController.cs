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

    [SerializeField]
    private StateMachine stateMachine;

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
        stateMachine.Initialize();
        stateMachine.NextState();
    }

    public void SetDestination(string destinationName)
    {
        navMeshAgent.SetDestination(destinations.GetDestination(destinationName));
    }

    [Serializable]
    private class Destinations
    {
        [SerializeField]
        private Transform[] initialDestinations;

        private Dictionary<string, Vector3> destinations;

        public void Initialize()
        {
            destinations = initialDestinations.ToDictionary(transform => transform.name, transform => transform.position);
        }

        public Vector3 GetDestination(string destinationName)
        {
            return destinations[destinationName];
        }
    }
}
