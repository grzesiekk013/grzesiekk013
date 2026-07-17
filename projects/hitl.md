# Hardware-In-The-Loop (HITL) FPV Video Pipeline
### Virtual Unreal Engine Camera to Physical Analog FPV Goggles

A low-latency, custom-built hardware-in-the-loop (HITL) video pipeline designed to stream real-time virtual camera feeds from Unreal Engine directly to physical FPV VTX (Video Transmitter) goggles. This rig enables pilot training and autopilot hardware testing under realistic analog static conditions.

---

## Key Engineering Highlights

### 1. High-Speed Software Pipeline (I/O Bottleneck Elimination)
*   **Ramdisk Storage:** To bypass SSD/HDD write-wear and physical I/O bottlenecks caused by high-frequency screenshots, Unreal Engine saves real-time camera frames directly to a virtual **Ramdisk** (in-memory storage).
*   **Asynchronous Watchdog:** Developed a lightweight **Python** background script utilizing the `watchdog` library to monitor the Ramdisk. It instantly detects and grabs new image files as they are generated.
*   **Virtual Loopback Webcam:** The script feeds these grabbed frames into a virtual webcam device using `pyfakewebcam` (Linux/V4L2), which is then rendered on a secondary display via `ffplay` with minimal latency.

### 2. Physical Hardware Bridge (Digital-to-Analog RF)
*   **Signal Conversion:** The display output (HDMI) rendering the virtual camera feed is connected to an **HDMI-to-AV (RCA)** converter.
*   **Analog VTX Integration:** The analog composite video output (AV) is soldered/wired directly into the video input pin of a physical **FPV Video Transmitter (VTX)**.
*   **Wireless Broadcast:** The VTX broadcasts the simulated camera view over the 5.8 GHz analog band, allowing the pilot to view the Unreal Engine simulation directly inside real **FPV goggles** in real-time, exactly as if they were flying a physical drone.

---

## Tech Stack & Hardware Components
*   **Software & APIs:** Unreal Engine 5, Python (`watchdog` API, `pyfakewebcam`), FFplay (FFmpeg toolset)
*   **Operating System Integration:** Ramdisk management, virtual video loopback devices (V4L2)
*   **Physical Hardware:** 5.8 GHz Analog FPV Video Transmitter (VTX), FPV Goggles, HDMI-to-AV (RCA) Hardware Converter, HDMI display interfaces

---

## Gallery
![SKYZONE POV](/images/hitl-img-0.png)
![VTX RECEIVER POV](/images/hitl-img-1.png)
---

Souce Code Not Available