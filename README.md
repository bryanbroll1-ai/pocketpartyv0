# Pocket Party Prototype

Mobile-first local party board game prototype for Unity 6 or recent Unity LTS. It uses code-generated placeholder visuals and Unity UI only, with no copyrighted characters, names, sounds, board layouts, or external assets.

## What Is Implemented

- Main Menu, Game Setup, Board HUD, Settings, minigame results, and final results screens.
- 2 to 4 local players with one human and local bots.
- Turn loop with dice rolls, board movement, tile effects, periodic minigames, coin awards, round limit, and winner by coins.
- One generated board with Normal, CoinPlus, CoinMinus, Event, and Minigame tiles.
- A lightweight 2.5D board presentation with raised tiles, capsule player pieces, path segments, generated island dressing, trees, rocks, crystals, flags, and water patches.
- A smooth perspective camera that follows the current turn's player.
- Placeholder characters: Bloop, Nomi, Taro, and Pippa.
- Reusable minigame framework through `MinigameBase`, `MinigameManager`, and `MinigameResult`.
- Six touch-friendly minigames:
  - Time Stop
  - Color Rush
  - Hold & Release
  - Swipe Dash
  - Tap Targets
  - Memory Tiles
- Mouse fallback for Editor play mode.
- Performance defaults through `PerformanceSettings`: 30 FPS, battery saver on, disabled realtime shadows, no postprocessing dependency.
- Placeholder `AudioManager` methods for future sounds.

## Minimal Unity Scene Setup

1. Open this folder as a Unity project.
2. Open any scene, such as `Assets/Scenes/main.unity` or `Assets/dadad.unity`.
3. Press Play.

`AutoGameBootstrapper` can create `GameBootstrapper` automatically after a scene loads if the scene does not already contain one. `GameBootstrapper` then creates the camera, event system, managers, board visuals, and UI at runtime.

## Recommended Project Settings

- Orientation: Portrait.
- Target frame rate: controlled by `PerformanceSettings`.
- Default quality: low or medium for mobile.
- Disable realtime shadows for this prototype.
- Use IL2CPP and ARM64 when making mobile builds later.

## Next Iteration TODO

- Replace procedural primitive visuals with a small original art kit when the prototype direction is locked.
- Add richer character animations for idle, hop, win, and lose moments.
- Add polished pause/resume handling and route all timers through a shared pause-aware timer helper.
- Improve bot personalities and difficulty bands per minigame.
- Add object pooling for Tap Targets if target spawn visuals become richer.
- Add simple original sound effects through `AudioManager`.
- Add automated Unity PlayMode tests for one full board round and Time Stop scoring.

## Performance Notes

- The board uses generated sprites and no physics.
- Only active minigames use `Update`, and each destroys itself after completion.
- UI is built from Unity UGUI primitives, avoiding heavy rendering features.
- `Application.targetFrameRate` defaults to 30 for battery life, with a 60 FPS option in Settings.
- Realtime shadows, soft particles, and reflection probes are disabled by default.
