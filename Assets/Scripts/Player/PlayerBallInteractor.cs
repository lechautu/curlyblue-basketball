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

    [Header("Trajectory Settings")]
    [SerializeField] private int _trajectorySegments = 30;
    [SerializeField] private float _trajectoryTimeStep = 0.05f;
    [SerializeField] private float _trajectoryWidth = 0.05f;

    private bool _isHoldingBall;
    private bool _isGameOver;
    private LineRenderer _trajectoryLine;

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
                _ballLayer = ~0;
            }
        }

        if (_hudController == null)
        {
            Debug.LogError("[PlayerBallInteractor] HUDController reference is missing! Please assign it in the Inspector.");
        }

        GameEvents.OnGameOver += HandleGameOver;
        SetupTrajectoryLine();
    }

    private void SetupTrajectoryLine()
    {
        GameObject trajObj = new GameObject("ThrowTrajectory");
        trajObj.transform.SetParent(this.transform);
        _trajectoryLine = trajObj.AddComponent<LineRenderer>();
        _trajectoryLine.positionCount = _trajectorySegments;
        _trajectoryLine.startWidth = _trajectoryWidth;
        _trajectoryLine.endWidth = _trajectoryWidth * 0.2f;
        
        // Setup Material and Blue Color with alpha (approx 60%)
        _trajectoryLine.material = new Material(Shader.Find("Sprites/Default"));
        Color blueAlpha = new Color(0f, 0.4f, 1f, 0.6f); 
        _trajectoryLine.startColor = blueAlpha;
        _trajectoryLine.endColor = blueAlpha;
        _trajectoryLine.enabled = false;
    }

    private void OnDestroy()
    {
        GameEvents.OnGameOver -= HandleGameOver;
    }

    private void HandleGameOver(int score)
    {
        _isGameOver = true;
        if (_isHoldingBall)
        {
            ResetBall();
        }
        if (_trajectoryLine != null) _trajectoryLine.enabled = false;
    }

    private void Update()
    {
        if (_isGameOver) return;

        if (_inputReader.WasResetPressedThisFrame)
        {
            ResetBall();
            return;
        }

        if (!_isHoldingBall)
        {
            if (_inputReader.WasPickupThrowPressedThisFrame)
            {
                TryPickup();
            }
        }
        else
        {
            if (_inputReader.WasPickupThrowPressedThisFrame)
            {
                _throwController.BeginCharge();
            }

            if (_throwController.IsCharging)
            {
                _throwController.UpdateCharge();

                if (_hudController != null)
                {
                    _hudController.SetChargePercent(_throwController.ChargePercent);
                }

                // Draw trajectory while charging
                _throwController.GetCurrentThrowVelocity(_mainCamera.transform, out Vector3 currentVel);
                UpdateTrajectory(_ballHoldPoint.position, currentVel);

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
            }
        }
    }

    private void ExecuteThrow()
    {
        _throwController.ComputeThrow(_mainCamera.transform, out Vector3 velocity, out Vector3 angularVelocity);
        _ballController.Throw(velocity, angularVelocity);
        _isHoldingBall = false;

        if (_trajectoryLine != null) _trajectoryLine.enabled = false;

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
            if (_hudController != null) _hudController.SetChargePercent(0f);
        }
        _ballController.ResetToSpawn();
        if (_trajectoryLine != null) _trajectoryLine.enabled = false;
    }

    private void UpdateTrajectory(Vector3 startPos, Vector3 velocity)
    {
        if (_trajectoryLine == null) return;
        _trajectoryLine.enabled = true;

        float g = Physics.gravity.y;
        for (int i = 0; i < _trajectorySegments; i++)
        {
            float t = i * _trajectoryTimeStep;
            float x = startPos.x + velocity.x * t;
            float z = startPos.z + velocity.z * t;
            float y = startPos.y + velocity.y * t + 0.5f * g * t * t;

            _trajectoryLine.SetPosition(i, new Vector3(x, y, z));
        }
    }
}
