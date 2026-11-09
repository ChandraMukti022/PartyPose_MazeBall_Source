# MainMekanik - AR Puzzle Game 🎮

[![Play on itch.io](https://img.shields.io/badge/Play_on-itch.io-FA5C5C?style=for-the-badge&logo=itch.io)](https://shinchanstudio.itch.io/partypose-maze-ball-ar)

MainMekanik is an interactive 2D/3D mini puzzle game developed in Unity, utilizing Augmented Reality (AR) body tracking as the primary controller. This project was developed as a Technical Interview assignment to demonstrate a clear core gameplay loop, solid code architecture, and creative non-standard inputs.

**👉 You can play and download the playable build here:** [https://shinchanstudio.itch.io/partypose-maze-ball-ar](https://shinchanstudio.itch.io/partypose-maze-ball-ar)

## 🌟 Key Features (Fulfilling Assessment Priorities)
- **Non-Standard Player Interaction:** Uses camera-based body tracking (MediaPipe) to control the puzzle, providing a unique and engaging twist on traditional keyboard/mouse inputs.
- **Progressive Level System:** Features 3 progressively challenging levels of "Maze Ball" managed cleanly via `ScriptableObject` (`LevelData`).
- **Screenshot to Disk:** Includes a built-in camera feature that captures the player's winning moment and automatically saves the `.png` image to the local Windows `Pictures` folder or the game's executable directory.
- **Polished Feedback:** Complete with UI transitions (Fade In/Out), visual prompts, Voice Over (3-2-1 GO), and Audio SFX/BGM to reward player actions.
- **Clean Code Architecture:** Driven by a centralized `GameFlowManager` state machine and dynamic prefab instantiation, ensuring a modular and scalable codebase.

## 🔄 Core Gameplay Loop
The game relies on a highly repeatable Action-Feedback-Progression loop:
1. **Action:** The player stands in front of the camera, waits for the AR setup scan, and physically tilts/moves their body to navigate the ball through the maze.
2. **Feedback:** Visual timers tick down, dynamic UI updates show the current state, and sound effects trigger upon successful puzzle completion or failure.
3. **Progression:** Upon winning, a snapshot is taken, and the player is presented with a Result Screen. Clicking "Lanjut" (Next) seamlessly transitions the player to a harder level without re-scanning, keeping the arcade momentum alive.

## 🏗️ Code Structure & Design Decisions
- **GameFlowManager.cs:** Acts as the brain of the game using a simple State logic (`Setup`, `Gameplay`, `Result`). It handles UI panel toggles, audio playback, and level transitions.
- **ScriptableObjects (`LevelData`):** Used to decouple level configurations from the core logic. Each level defines its own time limit and specific maze prefab, making it incredibly easy for game designers to add 100+ levels without touching a single line of code.
- **Dynamic Instantiation:** Levels are instantiated as child objects of the UI/Gameplay panel only when needed, and destroyed immediately upon level completion to ensure optimal memory usage and clean hierarchy.

## 🚀 Expansion Ideas
If given more time, the project could be expanded with:
- **Multi-Mini-Game Hub:** Integrating the experimental *Voice Command Memory Card* game using Windows Speech Recognition alongside the AR Maze.
- **Local Multiplayer:** Tracking two bodies simultaneously so players can compete side-by-side.
- **Online Leaderboard:** Fetching and posting completion times to a cloud backend to enhance replayability.

## 🛠️ How to Play (Build Instructions)
1. Download the latest release `.zip` from the [itch.io page](https://shinchanstudio.itch.io/partypose-maze-ball-ar) or clone this repository.
2. If compiling from source, open the project in **Unity 2022.3** (or later).
3. Open the `Main Menu` scene in the `Scenes` folder.
4. Ensure your webcam/microphone permissions are enabled.
5. Press **Play** and enjoy!
