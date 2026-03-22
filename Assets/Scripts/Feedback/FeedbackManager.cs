using UnityEngine;

/// <summary>
/// Centralized feedback manager. Subscribes to GameEvents and plays
/// audio (via SFXManager) and visual feedback (Camera Shake).
/// </summary>
public class FeedbackManager : MonoBehaviour
{
    [Header("SFX IDs (Must match SFXDatabase)")]
    [SerializeField] private string _throwSfxId = "throw";
    [SerializeField] private string _rimHitSfxId = "rim";
    [SerializeField] private string _backboardHitSfxId = "backboard_hit";
    [SerializeField] private string _scoreSfxId = "score";
    [SerializeField] private string _groundHitSfxId = "ball_hit_ground";

    [Header("Camera Shake")]
    [SerializeField] private CameraShakeController _cameraShake;
    [SerializeField] private float _scoreShakeIntensity = 0.15f;
    [SerializeField] private float _scoreShakeDuration = 0.2f;
    [SerializeField] private float _rimHitShakeIntensity = 0.08f;
    [SerializeField] private float _rimHitShakeDuration = 0.15f;
    [SerializeField] private float _backboardHitShakeIntensity = 0.1f;
    [SerializeField] private float _backboardHitShakeDuration = 0.2f;

    private void OnEnable()
    {
        GameEvents.OnThrowReleased += HandleThrow;
        GameEvents.OnRimHit += HandleRimHit;
        GameEvents.OnBackboardHit += HandleBackboardHit;
        GameEvents.OnBallScored += HandleScore;
        GameEvents.OnBallHitGround += HandleBallHitGround;
    }

    private void OnDisable()
    {
        GameEvents.OnThrowReleased -= HandleThrow;
        GameEvents.OnRimHit -= HandleRimHit;
        GameEvents.OnBackboardHit -= HandleBackboardHit;
        GameEvents.OnBallScored -= HandleScore;
        GameEvents.OnBallHitGround -= HandleBallHitGround;
    }

    private void HandleThrow()
    {
        if (SFXManager.Instance != null)
        {
            SFXManager.Instance.PlaySFX(_throwSfxId, null); // 2D sound at camera/listener
        }
    }

    private void HandleRimHit(Vector3 point)
    {
        if (SFXManager.Instance != null)
        {
            SFXManager.Instance.PlaySFX(_rimHitSfxId, point); // 3D sound at the rim
        }
        
        if (_cameraShake != null)
        {
            _cameraShake.Shake(_rimHitShakeIntensity, _rimHitShakeDuration);
        }
    }

    private void HandleBackboardHit(Vector3 point)
    {
        if (SFXManager.Instance != null)
        {
            SFXManager.Instance.PlaySFX(_backboardHitSfxId, point); // 3D sound at backboard
        }
        
        if (_cameraShake != null)
        {
            _cameraShake.Shake(_backboardHitShakeIntensity, _backboardHitShakeDuration);
        }
    }

    private void HandleScore(int totalScore)
    {
        // 2D sound
        if (SFXManager.Instance != null)
        {
            SFXManager.Instance.PlaySFX(_scoreSfxId, null); 
        }

        if (_cameraShake != null)
        {
            _cameraShake.Shake(_scoreShakeIntensity, _scoreShakeDuration);
        }
    }

    private void HandleBallHitGround(Vector3 point)
    {
        if (SFXManager.Instance != null)
        {
            SFXManager.Instance.PlaySFX(_groundHitSfxId, point);
        }
    }
}
