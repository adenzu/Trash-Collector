using System.Collections.Generic;
using UnityEngine;

public static class Interactor
{
    public static void InteractOverlapping(Collider2D collider)
    {
        List<Collider2D> overlappedColliders = new();

        collider.Overlap(overlappedColliders);

        foreach (var overlappedCollider in overlappedColliders)
        {
            bool isInteractiveObject = overlappedCollider.TryGetComponent(out InteractiveObject interactiveObject);
            if (isInteractiveObject)
            {
                interactiveObject.InvokeOnOverlap(overlappedCollider);
            }
        }
    }
}
