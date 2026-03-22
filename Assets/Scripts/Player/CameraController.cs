using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Simple mouse-look camera controller using Input System.
/// Locks cursor and rotates PlayerRig based on mouse delta.
/// </summary>
public class CameraController : MonoBehaviour
{
    [Header("Sensitivity")]
    [SerializeField] private float _mouseSensitivity = 0.1f;
    [SerializeField] private float _verticalClamp = 80f;

    [Header("References")]
    [SerializeField] private Transform _playerRig;

    private float _verticalRotation;
    private bool _isGameOver;
    private bool _isReplaying;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        if (_playerRig == null) _playerRig = transform.parent;

        GameEvents.OnGameOver += HandleGameOver;
        GameEvents.OnReplayToggle += (active) => _isReplaying = active;
    }

    private void OnDestroy()
    {
        GameEvents.OnGameOver -= HandleGameOver;
    }

    private void HandleGameOver(int score)
    {
        _isGameOver = true;
    }

    private void Update()
    {
        if (_isGameOver || _isReplaying || _playerRig == null) return;
        if (Time.frameCount < 5) return;
        
        Vector2 mouseDelta = Mouse.current != null ? Mouse.current.delta.ReadValue() : Vector2.zero;

        float mouseX = mouseDelta.x * _mouseSensitivity;
        float mouseY = mouseDelta.y * _mouseSensitivity;

        // Horizontal rotation on the parent (PlayerRig)
        _playerRig.Rotate(Vector3.up * mouseX);

        // Vertical rotation on this camera
        _verticalRotation -= mouseY;
        _verticalRotation = Mathf.Clamp(_verticalRotation, -_verticalClamp, _verticalClamp);
        transform.localEulerAngles = new Vector3(_verticalRotation, 0f, 0f);
    }
}
