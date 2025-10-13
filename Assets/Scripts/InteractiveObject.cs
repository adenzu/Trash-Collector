using System;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider2D))]
public class InteractiveObject : MonoBehaviour
{
    [SerializeField]
    private Interaction[] m_interactions;

    [SerializeField]
    private InteractionOption m_option;

    public void InvokeOnOverlap(GameObject interactor)
    {
        foreach (var interaction in m_interactions)
        {
            if (interaction.CanInteractorTrigger(interactor))
            {
                if (!InvokeOnOverlapStep(interaction, interactor))
                {
                    break;
                }
            }
        }
    }

    private bool InvokeOnOverlapStep(Interaction interaction, GameObject interactor)
    {
        interaction.InvokeOnOverlap(interactor);

        return m_option switch
        {
            InteractionOption.All => true,
            InteractionOption.Any => false,
            _ => true,
        };
    }

    [Serializable]
    private class Interaction
    {
        [SerializeField]
        private string interactorTagFilter;

        [SerializeField]
        private UnityEvent<GameObject> onOverLap;

        public bool CanInteractorTrigger(GameObject interactor)
        {
            return IsTagAttached(interactor);
        }

        public void InvokeOnOverlap(GameObject interactor)
        {
            onOverLap.Invoke(interactor);
        }

        private bool IsTagAttached(GameObject interactor)
        {
            return interactor.CompareTag(interactorTagFilter);
        }
    }

    private enum InteractionOption
    {
        All,
        Any,
    }
}
