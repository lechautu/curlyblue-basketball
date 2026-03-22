# Basketball Feel Test

A 3D basketball shooting prototype built in Unity, focusing on game feel, clean code, and believable physics.

## Unity Version

**Unity 6000.3.11** (URP)

## Controls

| Input | Action |
|-------|--------|
| **Mouse Move** | Aim / look around |
| **Left Click** | Pick up ball (click on it) |
| **Hold Left Click** | Charge throw power |
| **Release Left Click** | Throw ball |
| **R** | Reset ball to spawn position |
| **Esc** | Quit game |

## Features

### Core Loop
- **Raycast pickup** — click on the basketball to grab it
- **Charge-and-throw** — hold LMB to charge, release to throw with variable force
- **Scoring** — top-to-bottom trigger validation prevents false positives
- **Instant reset** — R key returns the ball to spawn immediately

### Game Feel
- **Charge indicator** — orange-to-red bar below crosshair shows throw power
- **Score pop** — scale punch animation on the score text
- **Camera shake** — on rim hits, backboard hits, and scoring
- **Procedural SFX** — whoosh, metallic clank, thud, and ding generated at runtime
- **Backspin** — thrown ball has angular velocity for believable arc
- **Physics materials** — tuned bounciness and friction on ball, rim, backboard, floor

### Architecture
- **Event-driven** — static `GameEvents` hub decouples all systems
- **State machine** — `BallController` manages 5 ball states (Idle, Held, Thrown, ScoredCooldown, Resetting)
- **Modular scripts** — single responsibility per script, organized by feature folder
- **Inspector-tunable** — all gameplay parameters exposed as serialized fields

## Project Structure

```
Assets/Scripts/
├── Ball/           BallController, BallThrowController, BallCollisionFeedback
├── Core/           GameEvents, GameManager, ScoreManager, PhysicsSetup
├── Feedback/       FeedbackManager, CameraShakeController
├── Hoop/           HoopScoreDetector, ScoreTriggerTop, ScoreTriggerBottom
├── Input/          PlayerInputReader
├── Player/         PlayerBallInteractor, CameraController
└── UI/             HUDController
```

## How to Run

1. Open the project in **Unity 6000.3.11**
2. Open `Assets/Scenes/SampleScene.unity`
3. Press **Play**
4. Click the ball → hold to charge → release to throw → score!
