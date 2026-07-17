# Auto Ware Bot
### Autonomous Mobile Robot (ESP32-S3 & Python / Kinematic A\* Navigation)

*Built and programmed from scratch during an intensive 48-hour rapid prototyping engineering challenge.*  
Auto Ware Bot is an autonomous, self-navigating differential-drive mobile robot designed to dynamically map unknown indoor environments and execute precise pathfinding trajectories to target coordinates.

---

> **My Role: Project Lead, Hardware Architect & Lead Developer**
> As the leader of this high-pressure 48-hour sprint, I procured the entire hardware stack, designed the complete electronic schematics, and wired/soldered the robot's physical control boards. On the software side, I wrote the majority of the dual-core C++ embedded firmware, designed the robust fault-tolerant communication state machine, and developed the centralized Python server that manages the robot's high-level planning and movement.

---

## Key Engineering Highlights

### 1. Dual-Core Real-Time Task Allocation (ESP32-S3)
To guarantee smooth, bottleneck-free execution under tight timing constraints, I designed the embedded C++ firmware to run concurrently across both physical cores of the **ESP32-S3**:
*   **Core 1 (Hardware & Motion Control):** Dedicated entirely to low-level, high-priority hardware tasks. It parses raw serial data frames from the **LIDAR Delta 2A** over UART and generates precise, jitter-free step/direction signal pulses for the motor drivers.
*   **Core 2 (Network & Logic):** Manages high-level system operations, including maintaining Wi-Fi stability, handling asynchronous **WebSocket** communication, and running the core robot State Machine.

### 2. Custom Protocol & Symmetric State Machine
I designed a custom, bidirectional communication protocol running over WebSockets. The protocol features unique "symmetric wrapper commands" (e.g., `!command;;dnammoc!`) that act as light data-integrity filters, preventing corrupted or truncated packets from triggering false hardware actions:

```text
               [INIT]
                 │
             [NO_WIFI]
                 │
     [NO_CONNECTION_TO_SERVER]
                 │
   [IS_THERE_ANY_INSTRUCTION]  <- (Requests task: "!any_cmd;;dmc_yna!")
                 │
        [SENDING_LIDAR_DATA]   - (Sends scan: "!lidar;;radil!")
                 │
          [GET_INSTRUCTION]    - (Requests trajectory: "!get_cmds;;sdmc_teg!")
                 │
       [PROCEEDING_INSTRUCTION]
                 │
     [PROCEEDING_INSTRUCTION_DONE]
                 │
           [SENDING_DONE]      - (Reports completion: "!cmd_done;;enod_dmc!")
```

### 3. Hardware Integration & Silent Stepper Control
*   **Power Distribution:** Designed a robust battery management schematic powered by a 4-cell Lithium-Ion pack. Integrated a physical 7-segment display directly connected to an analog-to-digital converter (ADC) line to serve as an onboard, real-time voltage monitor.
*   **Silent & Precise Motion:** Wired and configured two **TMC2208 stepper drivers** in StealthChop mode. This allowed silent, highly precise micro-stepping for the dual **NEMA 17 stepper motors**, eliminating slippage and improving odometry estimations during autonomous movement.

### 4. Offloaded Kinematic Pathfinding & Mapping (Python Server)
To keep the robot's physical payload light and power-efficient, we offloaded heavy computational math to a centralized Python server:
*   **Occupancy Grid Mapping:** The server continuously ingests raw polar coordinate data from the onboard LIDAR over WebSockets, transforming it into a real-time 2D occupancy grid map of the environment.
*   **Kinematic A\* Algorithm:** Developed a pathfinding engine using a Kinematic A\* path planning algorithm. Unlike standard grid-based pathfinding, this model factors in the actual physical turn-radius limits and differential constraints of the physical chassis, generating smooth, executable curves instead of jagged, impossible paths.
*   **Dynamic Replanning:** If a dynamic obstacle (like a human walking in front of the robot) is introduced, the occupancy grid updates instantly, causing the server to recalculate and stream a modified trajectory sequence on-the-fly.

---

## Tech Stack & Hardware Components
*   **Role:** Project Lead, Hardware Engineer & Lead Software Developer
*   **Microcontroller:** FireBeetle 2 ESP32-S3-WROOM (Dual-Core LX7)
*   **Sensing & Feedback:** Delta 2A LIDAR (UART), 7-Segment Voltmeter (ADC)
*   **Actuators & Drivers:** 2x NEMA 17 Stepper Motors, 2x TMC2208 Silent Step Drivers
*   **Embedded Firmware:** C++ (Arduino Core, ESP32 Multi-Core SDK, WebSockets Client)
*   **Server Stack:** Python (WebSocket Server, Occupancy Grid Generator, Kinematic A\* Path Planner)
*   **Power & Fabrication:** 4S Lithium-Ion Battery Array, Custom Enclosure, Solder-wired Proto-boards

---

## Gallery
### How it looks  
![Robot Picture](/images/auto-ware-bot-enclosure.png)  

### Robot Idea  
![Robot Idea](/images/auto-ware-bot-idea-1.png)  

### [Server Side Live Data Preview Video](/images/auto-ware-bot-live-data.mp4)

### [Robot Doing Task Video](/images/auto-ware-bot-in-work.mp4)

---

[Source Code](source-code/auto-ware-bot)  
[Docs](/docs/auto-ware-bot/auto-ware-bot-presentation.pdf)

---

* **Jakub Lidzbarski** — Co-Author - C++ program testing, soldering, Research
