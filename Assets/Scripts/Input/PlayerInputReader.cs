using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Wraps Unity Input System actions. Exposes input state for polling.
/// Contains NO gameplay logic.
/// </summary>
public class PlayerInputReader : MonoBehaviour
{
    private InputAction _pickupThrowAction;
    private InputAction _resetAction;

    public bool IsPickupThrowPressed { get; private set; }
    public bool WasPickupThrowPressedThisFrame { get; private set; }
    public bool WasPickupThrowReleasedThisFrame { get; private set; }
    public bool WasResetPressedThisFrame { get; private set; }

    private void OnEnable()
    {
        _pickupThrowAction = new InputAction("PickupThrow", InputActionType.Button, "<Mouse>/leftButton");
        _resetAction = new InputAction("Reset", InputActionType.Button, "<Keyboard>/r");

        _pickupThrowAction.Enable();
        _resetAction.Enable();
    }

    private void OnDisable()
    {
        _pickupThrowAction?.Disable();
        _resetAction?.Disable();
    }

    private void Update()
    {
        IsPickupThrowPressed = _pickupThrowAction.IsPressed();
        WasPickupThrowPressedThisFrame = _pickupThrowAction.WasPressedThisFrame();
        WasPickupThrowReleasedThisFrame = _pickupThrowAction.WasReleasedThisFrame();
        WasResetPressedThisFrame = _resetAction.WasPressedThisFrame();
    }
}
