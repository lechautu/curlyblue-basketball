using System;
using UnityEngine;

/// <summary>
/// Static event hub for decoupled gameplay communication.
/// No instance required — all events are static.
/// </summary>
public static class GameEvents
{
    public static event Action OnBallPickedUp;
    public static event Action OnThrowReleased;
    public static event Action<int> OnBallScored;
    public static event Action OnBallReset;
    public static event Action<Vector3> OnRimHit;
    public static event Action<Vector3> OnBackboardHit;
    public static event Action<Vector3> OnBallHitGround;

    // Game Mode Events
    public static event Action OnGameStart;
    public static event Action<int> OnGameOver;
    public static event Action OnGameRestart;

    public static void BallPickedUp() => OnBallPickedUp?.Invoke();
    public static void ThrowReleased() => OnThrowReleased?.Invoke();
    public static void BallScored(int totalScore) => OnBallScored?.Invoke(totalScore);
    public static void BallReset() => OnBallReset?.Invoke();
    public static void RimHit(Vector3 contactPoint) => OnRimHit?.Invoke(contactPoint);
    public static void BackboardHit(Vector3 contactPoint) => OnBackboardHit?.Invoke(contactPoint);
    public static void BallHitGround(Vector3 contactPoint) => OnBallHitGround?.Invoke(contactPoint);

    public static void GameStart() => OnGameStart?.Invoke();
    public static void GameOver(int finalScore) => OnGameOver?.Invoke(finalScore);
    public static void GameRestart() => OnGameRestart?.Invoke();
}
