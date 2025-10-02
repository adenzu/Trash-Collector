using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class NPCController : MonoBehaviour
{

    [SerializeField, Min(0.1f)]
    private float destinationSelectionRadius = 1.0f;

    [SerializeField, Min(0.1f)]
    private float moveEverySeconds = 5.0f;

    private NavMeshAgent navMeshAgent;
    private float sinceLastMove = 0.0f;

    void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.updateRotation = false;
        navMeshAgent.updateUpAxis = false;
    }

    void Update()
    {
        if (sinceLastMove < 0.0f)
        {
            Vector3 randomOffset = Random.insideUnitCircle.normalized * destinationSelectionRadius;
            navMeshAgent.SetDestination(transform.position + randomOffset);
            sinceLastMove = moveEverySeconds;
        }

        sinceLastMove -= Time.deltaTime;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, destinationSelectionRadius);
    }
}
