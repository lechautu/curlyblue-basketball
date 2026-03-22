using UnityEngine;

/// <summary>
/// Tracks score and broadcasts score events.
/// </summary>
public class ScoreManager : MonoBehaviour
{
    [SerializeField] private HUDController _hudController;

    private int _currentScore;

    public int CurrentScore => _currentScore;

    private void Awake()
    {
        if (_hudController == null)
        {
            Debug.LogError("[ScoreManager] HUDController reference is missing!");
        }
    }

    /// <summary>
    /// Add points and update UI + events.
    /// </summary>
    public void AddScore(int points)
    {
        _currentScore += points;

        if (_hudController != null)
        {
            _hudController.SetScore(_currentScore);
            _hudController.PlayScorePop();
        }

        GameEvents.BallScored(_currentScore);
    }

    /// <summary>
    /// Reset score to zero (if needed for game modes).
    /// </summary>
    public void ResetScore()
    {
        _currentScore = 0;

        if (_hudController != null)
        {
            _hudController.SetScore(_currentScore);
        }
    }
}
