using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Centralized input handling for keyboard, gamepad, and mobile touch.
/// </summary>
public class InputManager : MonoBehaviour
{
    private PlayerInput playerInput;
    private Vector2 movementInput = Vector2.zero;
    private Vector2 lookInput = Vector2.zero;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        if (playerInput == null)
            playerInput = gameObject.AddComponent<PlayerInput>();
    }

    public Vector2 GetMovementInput()
    {
        if (playerInput == null) return Vector2.zero;
        return playerInput.currentActionMap["Move"].ReadValue<Vector2>();
    }

    public Vector2 GetLookInput()
    {
        if (playerInput == null) return Vector2.zero;
        return playerInput.currentActionMap["Look"].ReadValue<Vector2>();
    }

    public bool IsSprintPressed()
    {
        if (playerInput == null) return false;
        return playerInput.currentActionMap["Sprint"].IsPressed();
    }

    public bool IsCrouchPressed()
    {
        if (playerInput == null) return false;
        return playerInput.currentActionMap["Crouch"].IsPressed();
    }

    public bool IsJumpPressed()
    {
        if (playerInput == null) return false;
        return playerInput.currentActionMap["Jump"].WasPressedThisFrame();
    }

    public bool IsInteractPressed()
    {
        if (playerInput == null) return false;
        return playerInput.currentActionMap["Interact"].WasPressedThisFrame();
    }

    public bool IsFirePressed()
    {
        if (playerInput == null) return false;
        return playerInput.currentActionMap["Fire"].IsPressed();
    }

    public bool IsAimPressed()
    {
        if (playerInput == null) return false;
        return playerInput.currentActionMap["Aim"].IsPressed();
    }
}