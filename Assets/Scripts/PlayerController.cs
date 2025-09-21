using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Collider2D), typeof(Rigidbody2D))]
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
    private Rigidbody2D rb;

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
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        InteractWithSurronding();
    }

    void FixedUpdate()
    {
        Move();
    }

    private void Move()
    {
        Vector3 moveAmount = moveAction.ReadValue<Vector2>();
        rb.linearVelocity = speed * moveAmount;
    }

    private void InteractWithSurronding()
    {
        if (interactAction.WasPressedThisFrame())
        {
            Interactor.InteractOverlapping(interactionCollider);
        }
    }
}
