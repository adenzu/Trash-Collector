using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Collider2D))]
public class PlayerController : MonoBehaviour
{
    [SerializeField, Min(0)]
    private float speed;

    [SerializeField]
    private InputActionAsset inputActions;

    [SerializeField]
    private Collider2D interactionCollider;

    private InputAction moveAction;
    private InputAction interactAction;

    void OnEnable()
    {
        inputActions.FindActionMap("Player").Enable();
    }

    void OnDisable()
    {
        inputActions.FindActionMap("Player").Disable();
    }

    void Awake()
    {
        moveAction = InputSystem.actions.FindAction("Move");
        interactAction = InputSystem.actions.FindAction("Interact");
    }

    void Update()
    {
        Move();
        InteractWithSurronding();
    }

    private void Move()
    {
        Vector3 moveAmount = moveAction.ReadValue<Vector2>();
        transform.position += speed * Time.deltaTime * moveAmount;
    }

    private void InteractWithSurronding()
    {
        if (interactAction.WasPressedThisFrame())
        {
            Interactor.InteractOverlapping(interactionCollider);
        }
    }
}
