using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider2D))]
public class InteractiveObject : MonoBehaviour
{
    [SerializeField]
    private LayerMask interactorLayerFilter;

    [SerializeField]
    private string interactorTagFilter;

    [SerializeField]
    private UnityEvent<Collider2D> onOverLap;

    public void InvokeOnOverlap(Collider2D invoker)
    {
        if (IsTagAttached(invoker.gameObject) && InLayer(invoker.gameObject))
        {
            onOverLap.Invoke(invoker);
        }
    }

    private bool IsTagAttached(GameObject invoker)
    {
        return invoker.CompareTag(interactorTagFilter);
    }

    private bool InLayer(GameObject invoker)
    {
        return (invoker.layer & interactorLayerFilter.value) != 0;
    }
}
