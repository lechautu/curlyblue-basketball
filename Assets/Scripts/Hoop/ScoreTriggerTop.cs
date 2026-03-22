using UnityEngine;

/// <summary>
/// Top score trigger. Detects ball entry and notifies HoopScoreDetector.
/// Must be placed above bottom trigger inside the hoop.
/// </summary>
[RequireComponent(typeof(Collider))]
public class ScoreTriggerTop : MonoBehaviour
{
    [SerializeField] private HoopScoreDetector _scoreDetector;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ball"))
        {
            _scoreDetector.OnBallEnteredTop(other);
        }
    }
}
