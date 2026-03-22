using UnityEngine;

/// <summary>
/// Manages ball state machine and provides API for pickup, throw, and reset.
/// States: IdleOnGround, Held, Thrown, ScoredCooldown, Resetting.
/// </summary>
public class BallController : MonoBehaviour
{
    public enum BallState
    {
        IdleOnGround,
        Held,
        Thrown,
        ScoredCooldown,
        Resetting
    }

    [Header("References")]
    [SerializeField] private Rigidbody _rb;
    [SerializeField] private SphereCollider _collider;

    public BallState CurrentState { get; private set; } = BallState.IdleOnGround;
    public Rigidbody Rb => _rb;

    private Vector3 _spawnPosition;
    private Quaternion _spawnRotation;
    private Transform _holdPoint;

    private void Awake()
    {
        if (_rb == null) _rb = GetComponent<Rigidbody>();
        if (_collider == null) _collider = GetComponent<SphereCollider>();

        _spawnPosition = transform.position;
        _spawnRotation = transform.rotation;
    }

    private void OnEnable()
    {
        GameEvents.OnBallScored += HandleScored;
    }

    private void Update()
    {
        if (CurrentState == BallState.Held && _holdPoint != null)
        {
            transform.position = _holdPoint.position;
            transform.rotation = _holdPoint.rotation;
        }
    }

    private void OnDisable()
    {
        GameEvents.OnBallScored -= HandleScored;
    }

    /// <summary>
    /// Transition to Held state. Attach to hold point, disable physics.
    /// </summary>
    public void SetHeldState(Transform holdPoint)
    {
        if (CurrentState == BallState.Held) return;

        _holdPoint = holdPoint;
        CurrentState = BallState.Held;

        _rb.linearVelocity = Vector3.zero;
        _rb.angularVelocity = Vector3.zero;
        _rb.isKinematic = true;

        transform.SetParent(holdPoint);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;

        GameEvents.BallPickedUp();
    }

    /// <summary>
    /// Transition to Thrown state. Detach, re-enable physics, apply velocity.
    /// </summary>
    public void Throw(Vector3 velocity, Vector3 angularVelocity)
    {
        if (CurrentState != BallState.Held) return;

        CurrentState = BallState.Thrown;

        transform.SetParent(null);
        _rb.isKinematic = false;
        _rb.linearVelocity = velocity;
        _rb.angularVelocity = angularVelocity;

        GameEvents.ThrowReleased();
    }

    /// <summary>
    /// Reset ball to spawn position. Clear all state.
    /// </summary>
    public void ResetToSpawn()
    {
        CurrentState = BallState.Resetting;

        transform.SetParent(null);
        _rb.isKinematic = true;
        _rb.linearVelocity = Vector3.zero;
        _rb.angularVelocity = Vector3.zero;

        transform.position = _spawnPosition;
        transform.rotation = _spawnRotation;

        _rb.isKinematic = false;
        CurrentState = BallState.IdleOnGround;

        GameEvents.BallReset();
    }

    private void HandleScored(int score)
    {
        CurrentState = BallState.ScoredCooldown;
        // Ball continues physics movement after scoring — state prevents re-scoring
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (CurrentState == BallState.Thrown || CurrentState == BallState.ScoredCooldown)
        {
            CurrentState = BallState.IdleOnGround;
        }
    }

    /// Automatically reset to idle state if landing on ground, though player can intercept mid-air.
    /// </summary>
    public bool CanPickup => CurrentState != BallState.Held;
}
