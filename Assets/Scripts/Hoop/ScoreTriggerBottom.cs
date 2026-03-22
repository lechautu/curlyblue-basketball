using UnityEngine;

/// <summary>
/// Bottom score trigger. Detects ball entry and notifies HoopScoreDetector.
/// Must be placed below top trigger inside the hoop.
/// </summary>
[RequireComponent(typeof(Collider))]
public class ScoreTriggerBottom : MonoBehaviour
{
    [SerializeField] private HoopScoreDetector _scoreDetector;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ball"))
        {
            _scoreDetector.OnBallEnteredBottom(other);
        }
    }
}
