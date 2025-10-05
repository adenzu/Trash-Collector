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

    private float shopTime = 0;

    private bool finishedShopping = false;

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
    }

    void Update()
    {
        if (shopTime > 0)
        {
            shopTime -= Time.deltaTime;
            if (shopTime <= 0)
            {
                finishedShopping = true;
            }
        }
        stateMachine.NextState();
    }

    public void SetDestination(string destinationName)
    {
        navMeshAgent.SetDestination(destinations.GetDestination(destinationName));
    }

    public void ArrivedToDestination(bool[] bools)
    {
        bools[0] = !navMeshAgent.hasPath;
    }

    public void FinishedShopping(bool[] bools)
    {
        bools[0] = finishedShopping;
    }

    public void StartShopTime()
    {
        shopTime = 5.0f;
        finishedShopping = false;
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
