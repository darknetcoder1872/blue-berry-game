using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Main player controller for movement, interaction, and state management.
/// Supports keyboard, gamepad, and mobile input.
/// </summary>
public class PlayerController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CharacterController characterController;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Animator animator;
    [SerializeField] private AudioSource audioSource;

    [Header("Movement")]
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float sprintSpeed = 10f;
    [SerializeField] private float crouchSpeed = 2.5f;
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float groundDrag = 5f;
    [SerializeField] private float airDrag = 2f;

    [Header("Look")]
    [SerializeField] private float mouseSensitivity = 1f;
    [SerializeField] private float maxLookAngle = 90f;

    [Header("Stamina")]
    [SerializeField] private float maxStamina = 100f;
    [SerializeField] private float sprintStaminaDrain = 20f;
    [SerializeField] private float staminaRegenRate = 15f;
    [SerializeField] private float staminaRegenDelay = 0.5f;

    [Header("Ground Check")]
    [SerializeField] private float groundDrag = 5f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float raycastDistance = 0.2f;

    // Input
    private Vector3 moveInput;
    private Vector2 lookInput;
    private bool wantsToSprint = false;
    private bool wantsToCrouch = false;
    private bool wantsToJump = false;

    // State
    private Vector3 velocity = Vector3.zero;
    private bool isGrounded = false;
    private bool isJumping = false;
    private bool isCrouching = false;
    private float currentStamina;
    private float staminaRegenTimer = 0f;
    private float xRotation = 0f;

    // Components
    private PlayerStats playerStats;
    private InputManager inputManager;

    private const float GRAVITY = -9.81f;
    private const float CROUCH_HEIGHT = 0.6f;
    private const float STAND_HEIGHT = 1.8f;

    private void Start()
    {
        // Get components
        playerStats = GetComponent<PlayerStats>();
        inputManager = GetComponent<InputManager>();

        if (characterController == null)
            characterController = GetComponent<CharacterController>();

        if (mainCamera == null)
            mainCamera = GetComponentInChildren<Camera>();

        currentStamina = maxStamina;

        Debug.Log("[PlayerController] Initialized");
    }

    private void Update()
    {
        // Get input
        GetInput();

        // Ground check
        CheckGrounded();

        // Handle movement
        HandleMovement();

        // Handle look
        HandleLook();

        // Handle jump
        HandleJump();

        // Handle crouch
        HandleCrouch();

        // Update stamina
        UpdateStamina();

        // Apply velocity
        ApplyMovement();
    }

    private void GetInput()
    {
        // Get movement input
        Vector2 moveDir = inputManager.GetMovementInput();
        moveInput = new Vector3(moveDir.x, 0, moveDir.y);

        // Get look input
        lookInput = inputManager.GetLookInput();

        // Get action input
        wantsToSprint = inputManager.IsSprintPressed();
        wantsToCrouch = inputManager.IsCrouchPressed();
        wantsToJump = inputManager.IsJumpPressed();
    }

    private void HandleMovement()
    {
        // Determine current speed
        float targetSpeed = walkSpeed;
        if (wantsToSprint && currentStamina > 0 && !isCrouching)
        {
            targetSpeed = sprintSpeed;
            currentStamina -= sprintStaminaDrain * Time.deltaTime;
        }
        else if (isCrouching)
        {
            targetSpeed = crouchSpeed;
        }

        // Transform movement to world space
        Vector3 moveDirection = transform.forward * moveInput.z + transform.right * moveInput.x;
        moveDirection.Normalize();

        // Apply movement
        velocity.x = moveDirection.x * targetSpeed;
        velocity.z = moveDirection.z * targetSpeed;

        // Update animator
        if (animator != null)
        {
            animator.SetFloat("Speed", moveInput.magnitude * targetSpeed);
            animator.SetBool("IsGrounded", isGrounded);
            animator.SetBool("IsCrouching", isCrouching);
        }
    }

    private void HandleLook()
    {
        // Mouse/analog look
        xRotation -= lookInput.y * mouseSensitivity;
        xRotation = Mathf.Clamp(xRotation, -maxLookAngle, maxLookAngle);

        mainCamera.transform.localRotation = Quaternion.Euler(xRotation, 0, 0);
        transform.Rotate(Vector3.up * lookInput.x * mouseSensitivity);
    }

    private void HandleJump()
    {
        if (!wantsToJump || !isGrounded || isCrouching)
            return;

        velocity.y = Mathf.Sqrt(jumpForce * -2f * GRAVITY);
        isJumping = true;

        if (animator != null)
            animator.SetTrigger("Jump");
    }

    private void HandleCrouch()
    {
        isCrouching = wantsToCrouch;

        // Adjust character controller height
        float targetHeight = isCrouching ? CROUCH_HEIGHT : STAND_HEIGHT;
        characterController.height = Mathf.Lerp(characterController.height, targetHeight, Time.deltaTime * 5f);
    }

    private void CheckGrounded()
    {
        Ray ray = new Ray(transform.position, Vector3.down);
        isGrounded = Physics.Raycast(ray, raycastDistance, groundLayer);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = 0;
            isJumping = false;
        }
    }

    private void UpdateStamina()
    {
        if (!wantsToSprint || isCrouching)
        {
            staminaRegenTimer += Time.deltaTime;
            if (staminaRegenTimer >= staminaRegenDelay)
            {
                currentStamina = Mathf.Min(currentStamina + staminaRegenRate * Time.deltaTime, maxStamina);
            }
        }
        else
        {
            staminaRegenTimer = 0f;
        }
    }

    private void ApplyMovement()
    {
        // Apply gravity
        if (!isGrounded)
        {
            velocity.y += GRAVITY * Time.deltaTime;
        }

        // Apply ground drag
        float drag = isGrounded ? groundDrag : airDrag;
        velocity.x *= 1f / (1f + drag * Time.deltaTime);
        velocity.z *= 1f / (1f + drag * Time.deltaTime);

        // Move character
        characterController.Move(velocity * Time.deltaTime);
    }

    // Getters
    public float GetStaminaPercent() => currentStamina / maxStamina;
    public bool IsGrounded => isGrounded;
    public bool IsCrouching => isCrouching;
    public float GetCurrentSpeed() => new Vector3(velocity.x, 0, velocity.z).magnitude;
}