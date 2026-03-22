using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Cinematic and Replay Manager.
/// Tracks ball positions and plays back when a score is detected.
/// </summary>
public class CinematicManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private BallTracker _ballTracker;
    [SerializeField] private BallController _ballController;
    [SerializeField] private Camera _replayCamera;
    
    [Header("Settings")]
    [SerializeField] private Vector3 _cameraOffset = new Vector3(0f, 0.4f, -1.2f);
    [SerializeField] private float _slowMoFactor = 0.85f;

    private bool _isReplaying;

    private void Start()
    {
        GameEvents.OnBallScored += StartReplaySequence;
    }

    private void OnDestroy()
    {
        GameEvents.OnBallScored -= StartReplaySequence;
    }

    private void StartReplaySequence(int score)
    {
        if (_isReplaying) return;
        StartCoroutine(ExecuteReplayRoutine());
    }

    private IEnumerator ExecuteReplayRoutine()
    {
        _isReplaying = true;
        // Use the invoker method
        GameEvents.ReplayToggle(true);

        if (_replayCamera != null) _replayCamera.gameObject.SetActive(true);

        var frames = _ballTracker.GetTotalFrames();
        if (frames.Count == 0)
        {
            ResetGameplay();
            yield break;
        }

        // Snap ball to start
        _ballController.transform.position = frames[0].pos;
        _ballController.transform.rotation = frames[0].rot;
        
        _ballController.Rb.isKinematic = true;

        float spf = 0.0166f / _slowMoFactor; // Approx frame time scaled by slow mo
        
        yield return new WaitForSeconds(0.2f);

        for (int i = 0; i < frames.Count; i++)
        {
            _ballController.transform.position = frames[i].pos;
            _ballController.transform.rotation = frames[i].rot;
            
            if (_replayCamera != null)
            {
                // Simple close-up follow
                _replayCamera.transform.position = frames[i].pos + _cameraOffset;
                _replayCamera.transform.LookAt(frames[i].pos);
            }

            yield return new WaitForSeconds(spf);
        }

        yield return new WaitForSeconds(0.5f);
        
        ResetGameplay();
    }

    private void ResetGameplay()
    {
        _isReplaying = false;
        if (_replayCamera != null) _replayCamera.gameObject.SetActive(false);
        _ballController.Rb.isKinematic = false;
        // Use the invoker method
        GameEvents.ReplayToggle(false);
        _ballController.ResetToSpawn();
    }
}
