using UnityEngine;

/// <summary>
/// Simple camera shake via position offset with decay.
/// Attach to camera or camera parent.
/// </summary>
public class CameraShakeController : MonoBehaviour
{
    private float _shakeIntensity;
    private float _shakeDuration;
    private float _shakeTimer;
    private Vector3 _originalLocalPosition;

    private void Awake()
    {
        _originalLocalPosition = transform.localPosition;
    }

    /// <summary>
    /// Start a shake with given intensity and duration.
    /// </summary>
    public void Shake(float intensity, float duration)
    {
        _shakeIntensity = intensity;
        _shakeDuration = duration;
        _shakeTimer = duration;
    }

    private void LateUpdate()
    {
        if (_shakeTimer > 0f)
        {
            _shakeTimer -= Time.deltaTime;
            float decay = _shakeTimer / _shakeDuration;
            Vector3 offset = Random.insideUnitSphere * (_shakeIntensity * decay);
            transform.localPosition = _originalLocalPosition + offset;
        }
        else
        {
            transform.localPosition = _originalLocalPosition;
        }
    }
}
