# Roller Shutters & Entrance Gates Driver
### IoT Smart Home Controller (C++ Firmware & C# Android Client)

An automated IoT ecosystem designed to control anti-burglary roller shutters and entrance gates, featuring automated dusk/dawn scheduling and instant mobile overrides.

---

## Key Engineering Highlights

### 1. High-Stability C++ Firmware (Server-Side)
*   **Hardware Platform:** Developed for **RP2040 (Raspberry Pi Pico W)** and **ESP8266** microcontrollers.
*   **Performance Decisions:** Written entirely in **C++** rather than MicroPython to guarantee 100% runtime uptime, eliminate garbage collection pauses, and prevent unexpected firmware crashes.
*   **Embedded HTTP Server:** The controller hosts a lightweight REST-like HTTP server that continuously listens for incoming network commands (e.g., sending an HTTP request to `GET /all/up`).

### 2. Physical Switch Interfacing (Relay Logic)
*   **The Approach:** Designed to safely interface with existing high-voltage current switches.
*   **Execution:** When a valid network request is received, the firmware triggers GPIO pins connected to solid-state or mechanical relays, simulating momentary physical button presses to safely open or close the gates and blinds.

### 3. Android Remote Control Client (C#)
*   **The App:** Developed a dedicated, fast-loading Android application in **C#** to control the entire system from anywhere within the home Wi-Fi network.
*   **Integration:** The app translates user actions into instant asynchronous HTTP requests, allowing grouped actions (e.g., closing all blinds at once) or individual zone control.

---

## Tech Stack & Hardware
*   **IoT Firmware:** C++ (Arduino Core / ESP8266 / RP2040)
*   **Mobile App:** C# (.NET / Xamarin / MAUI for Android)
*   **Network Protocols:** HTTP, TCP/IP, REST API architecture
*   **Hardware Components:** Relays (switching logic), physical momentary switches, ESP8266, Raspberry Pi Pico W (RP2040)

---
## Gallery
### Hardware
![Hardware](/images/roller-hardware-img-0.jpg)
### App Settings
![App Settings](/images/roller-app-img-1.jpg)
### App Main Screen
![App Main](/images/roller-app-img-2.jpg)
### Old Showcase Video
[Showcase Video](/images/roller-vid-showcase-1.mp4)

---

[Source Code](source-code/roller-shutter)  
