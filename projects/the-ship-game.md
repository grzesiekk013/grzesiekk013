# The Ship Game
### Retro-Styled LAN Multiplayer & Dynamic AI Battleship (C# .NET)

A modernized, retro-themed digital desktop adaptation of the classic Battleship board game, featuring low-latency local network multiplayer and a smart heuristic AI opponent.

---

> **My Role: Project Lead & Lead Software Architect**
> As the leader of this project, I architected the entire application, designed and developed the complete front-end user interface with its custom retro-style graphics, and programmed a significant portion of the backend systems—including the WebSocket-based LAN networking layer and the dynamic heuristic AI decision engine.

---

## Key Engineering Highlights

### 1. Asynchronous LAN Networking (WebSockets)
*   **Architecture:** I built a responsive, non-blocking multiplayer system using a peer-to-peer style architecture (`PLAY AS HOST` and `PLAY WITH FRIEND` modes).
*   **Communication:** I utilized the **WebSocket protocol** for lightweight, bidirectional data transfer over LAN, ensuring instantaneous turn synchronization and state updates without freezing the user interface.

### 2. Heuristic AI Decision Engine
*   **Phase 1 (Search):** I programmed the bot to operate using pseudo-random targeting guided by an optimized search space list to prevent redundant moves.
*   **Phase 2 (Scan & Track):** Upon registering a hit, the AI dynamically switches to a tracking state machine that I implemented, scanning adjacent coordinate lines (cross-pattern). Once a second hit is registered, it automatically computes the ship’s orientation (horizontal/vertical) to rapidly sink the target.

### 3. Collision-Free Auto-Positioning
*   I designed and implemented a recursive random placement algorithm for ships (ranging from size 4 down to 1) utilizing boundary-check methods (`isInBounds`) and coordinate collision matrices to ensure legal board setups instantly.

### 4. Modular Windows Forms UI & Retro Graphics
*   **Visual Assets:** I designed and integrated the retro-style visual aesthetics and assets to give the game its nostalgic look and feel.
*   **Layout:** I engineered the front-end using decoupled **Custom User Controls** within .NET Framework 4.8, allowing seamless, flicker-free transitions between the Retro Game Lobby, Settings, Credits, and the persistent global Scoreboard.

---

## Tech Stack & Libraries
*   **Language & Framework:** C# (.NET Framework 4.8)
*   **UI Framework:** Windows Forms (Custom User Controls)
*   **Networking:** WebSocket Protocol
*   **Audio Pipeline:** NAudio library (for interactive UI sound effects and background music)
*   **Configuration & Persistence:** Custom flat-file configuration database with automated fallback for user preferences (volume, nickname, high scores).

---

## Gallery
![The Ship Game Showcase GIF](/images/the-ship-game-gif.gif)

---

[Source Code](source-code/the-ship-game)  
[Playable Demo](source-code/the-ship-game/Lademann/bin/Debug/Lademann.exe) *Note: install projects/source-code/the-ship-game/Mario-Kart-DS.ttf font*  
[Docs](/docs/the-ship-game/the-ship-game-documentation.pdf)

---

* **Jakub Lidzbarski** — Co-Author
* **Cezary Piernikowski** — Co-Author - C# Consultant, Detection Algorithm