using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField, Min(0)]
    private float speed;

    [SerializeField]
    private InputActionAsset inputActions;

    private InputAction moveAction;

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
    }

    void Update()
    {
        Vector3 moveAmount = moveAction.ReadValue<Vector2>();
        transform.position += speed * Time.deltaTime * moveAmount;
    }
}
