# Auto Ware Robot
> *An autonomous mobile robot built and programmed in 48 hours as a student university project.* 

## About The Project
Auto Ware Robot is a custom-built autonomous mobile vehicle designed to map its surroundings and navigate dynamically to specific coordinates. Utilizing limited resources and hardware acquired specifically for this project, our team managed to build the physical casing, wire the electronics, and develop the complete software stack (both embedded and server-side) within a strict 48-hour timeframe. 

The robot relies on real-time LIDAR scanning to build an environment map. Once target coordinates are provided, it calculates the most optimal and kinematically feasible route, adjusting its path dynamically if new obstacles appear.

## Key Features & Assumptions
* **Custom WebSocket Communication:** Two-way real-time data exchange between the ESP32-S3 microcontroller and a central Python server.
* **Real-Time LIDAR Scanning:** Continuous environment mapping and obstacle detection.
* **Independent Motor Control:** Precise movement using two independent stepper motors.
* **Dynamic Route Planning:** Server-side map generation and route calculation offloading processing power from the rover.
* **Custom Enclosure:** Designed and built specifically for the hardware configuration.

---

## Hardware Specifications
* **Microcontroller:** FireBeetle 2 ESP32-S3-WROOM
* **Sensor:** LIDAR Delta 2A
* **Motors:** 2x Stepper Motors NEMA 17 42mm (RepRap 3D)
* **Motor Drivers:** TMC2208 (for silent and precise stepping)
* **Power Supply:** 4x 4.2V Lithium-Ion batteries
* **Display:** 7-segment display used as a battery voltage monitor

---

## System Architecture & Software

### ESP32 Dual-Core Architecture (C++)
To ensure smooth operation without bottlenecks, the embedded software on the ESP32-S3 was designed to utilize both available processor cores:
* **Core 1:** Handles low-level hardware interactions via UART, specifically receiving data from the LIDAR and sending precise step/direction signals to the stepper motors.
* **Core 2:** Manages high-level logic, including maintaining the WiFi connection, handling WebSocket communication with the Python server, and controlling the current Work State of the robot.

### Central Hub (Python)
A Python application acts as the central brain of the system. It receives the raw LIDAR data from the robot, interacts with the user, performs heavy calculations, and sends back movement instructions.


---

## Navigation & Pathfinding (Kinematic A*)
The core of the robot's autonomy is based on the **Kinematic A\*** algorithm. 
Unlike standard pathfinding, this algorithm takes into account the physical movement constraints (kinematics) of a differential-drive robot, resulting in realistic and executable trajectories.

1. **Occupancy Grid:** Raw LIDAR data is used in real-time to construct a 2D occupancy grid map containing obstacles.
2. **Dynamic Replanning:** The algorithm continuously monitors the grid. If a new obstacle appears in the robot's path, it recalculates the route on the fly.
3. **Execution:** The output is a sequence of velocity and orientation commands executed by the motor drivers, ensuring smooth and safe movement in a dynamic environment.

</p>

---

## State Machine
The robot's operational logic is governed by a strict state machine to ensure reliable communication and execution:

* `INIT` - Initial startup state.
* `NO_WIFI` - Triggered when the WiFi connection drops.
* `NO_CONNECTION_TO_SERVER` - Triggered when the WebSocket connection fails. If a connection is established, transitions to `IS_THERE_ANY_INSTRUCTION`.
* `IS_THERE_ANY_INSTRUCTION` - The robot asks the server for tasks using the command `!any_cmd;;dmc_yna!`. Upon positive response, transitions to `SENDING_LIDAR_DATA`.
* `SENDING_LIDAR_DATA` - Transmits scanned environment data via WebSocket using `!lidar;;radil!`. Transitions to `GET_INSTRUCTION` after receiving an acknowledgment.
* `GET_INSTRUCTION` - Requests actual movement commands using `!get_cmds;;sdmc_teg!`. Once received, transitions to `PROCEEDING_INSTRUCTION`.
* `PROCEEDING_INSTRUCTION` - The robot executes the physical movement.
* `PROCEEDING_INSTRUCTION_DONE` - Triggered internally once the movement task is physically completed. Transitions to `SENDING_DONE`.
* `SENDING_DONE` - Notifies the server of task completion using `!cmd_done;;enod_dmc!`. Returns to `IS_THERE_ANY_INSTRUCTION` to await new commands.

## Note
The source code is incomplete. I do not remember where is the final source code


