using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerControllerNew : MonoBehaviour
{
    [Header("Input (drag actions from your Input Actions asset)")]
    [SerializeField] private InputActionReference moveAction;
    [SerializeField] private InputActionReference interactAction;
    [SerializeField] private InputActionReference jumpAction;

    [Header("Movement")]
    public float moveSpeed = 5f;
    [SerializeField] private float jumpHeight = 1.2f;
    [SerializeField] private float gravity = -9.81f;

    private CharacterController controller;
    private Vector2 moveInput;
    private float verticalVelocity;
    private bool jumpRequested;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    private void OnEnable()
    {
        moveAction.action.Enable();
        interactAction.action.Enable();
        jumpAction.action.Enable();

        moveAction.action.performed += OnMove;
        moveAction.action.canceled += OnMove; // fires when the stick/keys are released
        interactAction.action.performed += OnInteract;
        jumpAction.action.performed += OnJump;
    }

    private void OnDisable()
    {
        moveAction.action.performed -= OnMove;
        moveAction.action.canceled -= OnMove;
        interactAction.action.performed -= OnInteract;
        jumpAction.action.performed -= OnJump;

        moveInput = Vector2.zero;
    }

    private void OnMove(InputAction.CallbackContext context) => moveInput = context.ReadValue<Vector2>();

    private void OnInteract(InputAction.CallbackContext context)
    {
        Debug.Log("Interact button pressed!");
    }

    private void OnJump(InputAction.CallbackContext context) => jumpRequested = true;

    private void Update()
    {
        bool grounded = controller.isGrounded;
        if (grounded && verticalVelocity < 0f)
            verticalVelocity = -2f; // small downward push keeps the controller grounded

        if (jumpRequested && grounded)
            verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
        jumpRequested = false;

        verticalVelocity += gravity * Time.deltaTime;

        Vector3 move = (transform.right * moveInput.x + transform.forward * moveInput.y) * moveSpeed;
        move.y = verticalVelocity;
        controller.Move(move * Time.deltaTime);
    }
}
