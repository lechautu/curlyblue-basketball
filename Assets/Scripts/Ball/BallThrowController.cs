using UnityEngine;

/// <summary>
/// Handles throw charging and force computation.
/// All tuning values exposed in Inspector.
/// </summary>
public class BallThrowController : MonoBehaviour
{
    [Header("Throw Tuning")]
    [SerializeField] private float _minThrowForce = 8f;
    [SerializeField] private float _maxThrowForce = 20f;
    [SerializeField] private float _maxChargeTime = 1.5f;
    [SerializeField] private float _upwardAssist = 0.45f;
    [SerializeField] private float _torqueAmount = 10f;
    [SerializeField] private AnimationCurve _chargeCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    private float _chargeTimer;
    private bool _isCharging;

    public bool IsCharging => _isCharging;
    public float ChargePercent => _maxChargeTime > 0f ? Mathf.Clamp01(_chargeTimer / _maxChargeTime) : 0f;

    /// <summary>
    /// Start charging the throw.
    /// </summary>
    public void BeginCharge()
    {
        _chargeTimer = 0f;
        _isCharging = true;
    }

    /// <summary>
    /// Update charge timer. Call from Update.
    /// </summary>
    public void UpdateCharge()
    {
        if (!_isCharging) return;
        _chargeTimer = Mathf.Min(_chargeTimer + Time.deltaTime, _maxChargeTime);
    }

    /// <summary>
    /// Gets the current velocity based on charge without stopping it.
    /// </summary>
    public void GetCurrentThrowVelocity(Transform cameraTransform, out Vector3 velocity)
    {
        float normalized = _maxChargeTime > 0f ? Mathf.Clamp01(_chargeTimer / _maxChargeTime) : 1f;
        float force = Mathf.Lerp(_minThrowForce, _maxThrowForce, _chargeCurve.Evaluate(normalized));

        Vector3 throwDir = (cameraTransform.forward + cameraTransform.up * _upwardAssist).normalized;
        velocity = throwDir * force;
    }

    /// <summary>
    /// Compute and return throw velocity + angular velocity. Resets charge state.
    /// </summary>
    public void ComputeThrow(Transform cameraTransform, out Vector3 velocity, out Vector3 angularVelocity)
    {
        float normalized = _maxChargeTime > 0f ? Mathf.Clamp01(_chargeTimer / _maxChargeTime) : 1f;
        float force = Mathf.Lerp(_minThrowForce, _maxThrowForce, _chargeCurve.Evaluate(normalized));

        Vector3 throwDir = (cameraTransform.forward + cameraTransform.up * _upwardAssist).normalized;
        velocity = throwDir * force;

        // Backspin for believable arc
        angularVelocity = -cameraTransform.right * _torqueAmount;

        StopCharge();
    }

    /// <summary>
    /// Stop charge without throwing.
    /// </summary>
    public void StopCharge()
    {
        _isCharging = false;
        _chargeTimer = 0f;
    }
}
