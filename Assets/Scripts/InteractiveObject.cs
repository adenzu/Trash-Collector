using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider2D))]
public class InteractiveObject : MonoBehaviour
{
    [SerializeField]
    private UnityEvent<Collider2D> onOverLap;

    public void InvokeOnOverlap(Collider2D contact)
    {
        onOverLap.Invoke(contact);
    }
}
