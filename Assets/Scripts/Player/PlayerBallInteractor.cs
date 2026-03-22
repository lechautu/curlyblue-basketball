using UnityEngine;

/// <summary>
/// Player interaction controller. Handles raycast pickup, charge, throw, and reset.
/// Feeds charge percentage to HUD each frame while charging.
/// </summary>
public class PlayerBallInteractor : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerInputReader _inputReader;
    [SerializeField] private Camera _mainCamera;
    [SerializeField] private Transform _ballHoldPoint;
    [SerializeField] private BallController _ballController;
    [SerializeField] private BallThrowController _throwController;
    [SerializeField] private HUDController _hudController;

    [Header("Pickup Settings")]
    [SerializeField] private float _pickupRange = 15f;
    [SerializeField] private LayerMask _ballLayer;

    private bool _isHoldingBall;
    private bool _isGameOver;

    private void Awake()
    {
        // Auto-set ball layer if not configured in Inspector
        if (_ballLayer == 0)
        {
            int layer = LayerMask.NameToLayer("Ball");
            if (layer >= 0)
            {
                _ballLayer = 1 << layer;
            }
            else
            {
                // Fallback: use Everything
                _ballLayer = ~0;
            }
        }

        if (_hudController == null)
        {
            Debug.LogError("[PlayerBallInteractor] HUDController reference is missing! Please assign it in the Inspector.");
        }

        GameEvents.OnGameOver += HandleGameOver;
    }

    private void OnDestroy()
    {
        GameEvents.OnGameOver -= HandleGameOver;
    }

    private void HandleGameOver(int score)
    {
        _isGameOver = true;
        // If holding, force release
        if (_isHoldingBall)
        {
            ResetBall();
        }
    }

    private void Update()
    {
        if (_isGameOver) return;

        // Reset request
        if (_inputReader.WasResetPressedThisFrame)
        {
            ResetBall();
            return;
        }

        if (!_isHoldingBall)
        {
            // Try pickup on click (Down)
            if (_inputReader.WasPickupThrowPressedThisFrame)
            {
                TryPickup();
            }
        }
        else
        {
            // If already holding, and user presses again, start charging
            if (_inputReader.WasPickupThrowPressedThisFrame)
            {
                _throwController.BeginCharge();
            }

            // Charging logic
            if (_throwController.IsCharging)
            {
                _throwController.UpdateCharge();

                // Update charge bar on HUD
                if (_hudController != null)
                {
                    _hudController.SetChargePercent(_throwController.ChargePercent);
                }

                // Release to throw
                if (_inputReader.WasPickupThrowReleasedThisFrame)
                {
                    ExecuteThrow();
                }
            }
        }
    }

    private void TryPickup()
    {
        Ray ray = _mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        if (Physics.Raycast(ray, out RaycastHit hit, _pickupRange, _ballLayer))
        {
            BallController ball = hit.collider.GetComponentInParent<BallController>();
            if (ball != null && ball.CanPickup)
            {
                ball.SetHeldState(_ballHoldPoint);
                _isHoldingBall = true;
                // Removed BeginCharge() from here - only pickup, no charge yet
            }
        }
    }

    private void ExecuteThrow()
    {
        _throwController.ComputeThrow(_mainCamera.transform, out Vector3 velocity, out Vector3 angularVelocity);
        _ballController.Throw(velocity, angularVelocity);
        _isHoldingBall = false;

        // Hide charge bar
        if (_hudController != null)
        {
            _hudController.SetChargePercent(0f);
        }
    }

    private void ResetBall()
    {
        if (_isHoldingBall)
        {
            _throwController.StopCharge();
            _isHoldingBall = false;

            // Hide charge bar
            if (_hudController != null)
            {
                _hudController.SetChargePercent(0f);
            }
        }
        _ballController.ResetToSpawn();
    }
}
