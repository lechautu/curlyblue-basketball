using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Minimal game manager. Handles quit via Input System.
/// </summary>
public class GameManager : MonoBehaviour
{
    [Header("Timer Settings")]
    [SerializeField] private float _gameDuration = 60f;
    private float _timeRemaining;
    private bool _isGameOver;

    [Header("References")]
    [SerializeField] private ScoreManager _scoreManager;

    private InputAction _quitAction;

    private void Start()
    {
        _timeRemaining = _gameDuration;
        GameEvents.GameStart();
    }

    private void OnEnable()
    {
        _quitAction = new InputAction("Quit", InputActionType.Button, "<Keyboard>/escape");
        _quitAction.Enable();

        GameEvents.OnGameRestart += RestartGame;
    }

    private void OnDisable()
    {
        _quitAction?.Disable();
        GameEvents.OnGameRestart -= RestartGame;
    }

    private void Update()
    {
        HandleQuit();

        if (_isGameOver) return;

        if (_timeRemaining > 0)
        {
            _timeRemaining -= Time.deltaTime;
            if (_timeRemaining <= 0)
            {
                _timeRemaining = 0;
                EndGame();
            }
        }
    }

    public float GetTimeRemaining() => _timeRemaining;

    private void EndGame()
    {
        _isGameOver = true;
        int finalScore = _scoreManager != null ? _scoreManager.CurrentScore : 0;
        GameEvents.GameOver(finalScore);
        
        // Lock cursor so user can click UI
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void RestartGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }

    private void HandleQuit()
    {
        if (_quitAction.WasPressedThisFrame())
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}
