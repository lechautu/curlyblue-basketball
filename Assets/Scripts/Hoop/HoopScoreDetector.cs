using UnityEngine;

/// <summary>
/// Validates basketball score using top-then-bottom trigger sequence.
/// Prevents double-counting per flight.
/// </summary>
public class HoopScoreDetector : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Collider _topTrigger;
    [SerializeField] private Collider _bottomTrigger;
    [SerializeField] private ScoreManager _scoreManager;

    private bool _hasPassedTop;
    private bool _hasScoredThisFlight;

    private void OnEnable()
    {
        GameEvents.OnBallReset += ClearState;
        GameEvents.OnBallPickedUp += ClearState;
    }

    private void OnDisable()
    {
        GameEvents.OnBallReset -= ClearState;
        GameEvents.OnBallPickedUp -= ClearState;
    }

    /// <summary>
    /// Called by ScoreTriggerTop when ball enters.
    /// </summary>
    public void OnBallEnteredTop(Collider ballCollider)
    {
        if (_hasScoredThisFlight) return;

        BallController ball = ballCollider.GetComponentInParent<BallController>();
        if (ball == null) return;
        if (ball.CurrentState == BallController.BallState.Held) return;

        _hasPassedTop = true;
    }

    /// <summary>
    /// Called by ScoreTriggerBottom when ball enters.
    /// </summary>
    public void OnBallEnteredBottom(Collider ballCollider)
    {
        if (!_hasPassedTop || _hasScoredThisFlight) return;

        BallController ball = ballCollider.GetComponentInParent<BallController>();
        if (ball == null) return;
        if (ball.CurrentState == BallController.BallState.Held) return;

        _hasScoredThisFlight = true;
        _hasPassedTop = false;

        if (_scoreManager != null)
        {
            _scoreManager.AddScore(1);
        }
    }

    public void ClearState()
    {
        _hasPassedTop = false;
        _hasScoredThisFlight = false;
    }
}
