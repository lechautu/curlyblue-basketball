using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// HUD controller. Creates its own Canvas + TMP elements at runtime if not assigned.
/// Manages score text, instruction text, score pop animation, crosshair, and charge bar.
/// </summary>
public class HUDController : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI _scoreText;
    [SerializeField] private TextMeshProUGUI _instructionText;
    // [SerializeField] private Image _crosshairDot;
    [SerializeField] private Slider _chargeSlider;
    [SerializeField] private Image _chargeSliderFill;
    [SerializeField] private TextMeshProUGUI _timerText;

    [Header("Game Over Popup")]
    [SerializeField] private GameObject _gameOverRoot;
    [SerializeField] private TextMeshProUGUI _finalScoreText;
    [SerializeField] private Button _replayButton;

    [Header("Manager References")]
    [SerializeField] private GameManager _gameManager;

    [Header("Score Pop Settings")]
    [SerializeField] private float _popScale = 1.4f;
    [SerializeField] private float _popDuration = 0.25f;

    private Coroutine _popCoroutine;

    private void Start()
    {
        SetScore(0);
        SetChargePercent(0f);

        if (_instructionText != null)
        {
            _instructionText.text = "CLICK ball to pick up | HOLD to charge | RELEASE to throw | R to reset";
        }

        if (_gameOverRoot != null) _gameOverRoot.SetActive(false);
        if (_replayButton != null) _replayButton.onClick.AddListener(OnReplayClicked);

        GameEvents.OnGameOver += ShowGameOver;
    }

    private void OnDestroy()
    {
        GameEvents.OnGameOver -= ShowGameOver;
    }

    private void Update()
    {
        UpdateTimerUI();
    }

    private void UpdateTimerUI()
    {
        if (_gameManager != null && _timerText != null)
        {
            float t = _gameManager.GetTimeRemaining();
            int minutes = Mathf.FloorToInt(t / 60f);
            int seconds = Mathf.FloorToInt(t % 60f);
            _timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
            
            // Turn timer red when low (last 10s)
            _timerText.color = t < 10.1f ? Color.red : Color.white;
        }
    }

    private void ShowGameOver(int finalScore)
    {
        if (_gameOverRoot != null)
        {
            _gameOverRoot.SetActive(true);
        }

        if (_finalScoreText != null)
        {
            _finalScoreText.text = "FINAL SCORE: " + finalScore;
        }

        // Hide other HUD elements
        if (_instructionText != null) _instructionText.gameObject.SetActive(false);
        if (_chargeSlider != null) _chargeSlider.gameObject.SetActive(false);
    }

    private void OnReplayClicked()
    {
        GameEvents.GameRestart();
    }

    // ──────────────────────────────────
    // Public API
    // ──────────────────────────────────

    /// <summary>
    /// Update score display.
    /// </summary>
    public void SetScore(int score)
    {
        if (_scoreText != null)
        {
            _scoreText.text = score.ToString();
        }
    }

    /// <summary>
    /// Set charge bar value (0..1) via Slider. Shows/hides automatically.
    /// </summary>
    public void SetChargePercent(float percent)
    {
        if (_chargeSlider == null) return;

        bool show = percent > 0.01f;
        if (_chargeSlider.gameObject.activeSelf != show)
        {
            _chargeSlider.gameObject.SetActive(show);
        }

        _chargeSlider.value = Mathf.Clamp01(percent);

        // Color ramp: orange → red at full charge
        if (_chargeSliderFill != null)
        {
            _chargeSliderFill.color = Color.Lerp(
                new Color(0f, 1f, 0f, 1f),
                new Color(1f, 0f, 0f, 1f),
                percent);
        }
    }

    /// <summary>
    /// Play a simple scale punch on the score text.
    /// </summary>
    public void PlayScorePop()
    {
        if (_scoreText == null) return;

        if (_popCoroutine != null)
        {
            StopCoroutine(_popCoroutine);
        }
        _popCoroutine = StartCoroutine(ScorePopRoutine());
    }

    private IEnumerator ScorePopRoutine()
    {
        Transform textTransform = _scoreText.transform;
        Vector3 originalScale = Vector3.one;
        Vector3 targetScale = originalScale * _popScale;

        float halfDuration = _popDuration * 0.5f;

        // Scale up
        float elapsed = 0f;
        while (elapsed < halfDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / halfDuration;
            textTransform.localScale = Vector3.Lerp(originalScale, targetScale, t);
            yield return null;
        }

        // Scale down
        elapsed = 0f;
        while (elapsed < halfDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / halfDuration;
            textTransform.localScale = Vector3.Lerp(targetScale, originalScale, t);
            yield return null;
        }

        textTransform.localScale = originalScale;
        _popCoroutine = null;
    }
}
