# Tactical Thermal Scope Attachment
### Custom Infrared Heat-Source Tracker (C++ Firmware & 3D Printed Hardware)

A fully custom, weapon-mountable thermal imaging attachment designed for sports rifles, capable of rendering real-time heatmaps and highlighting the hottest active thermal targets.

---

## Key Engineering Highlights

### 1. Thermal Data Acquisition & Processing
*   **Sensor Integration:** Integrated the **MLX90640 far-infrared thermal sensor** array ($32 \times 24$ pixels) over a high-speed **I2C bus**.
*   **Real-time Rendering:** Programmed the **ESP32** in **C++** to read raw temperature matrices, perform bilinear interpolation to smooth out the thermal grid, and map temperatures to a visual color spectrum (Ironbow/Rainbow palette) at fluid framerates.
*   **Target Tracking:** Implemented an on-screen target tracking algorithm that dynamically identifies, locks, and frames the hottest coordinate on the display in real time.

### 2. Mechanical Design & 3D Prototyping
*   **CAD Modeling:** Designed a custom, dual-part rugged enclosure featuring a precise mount compatible with standard tactical weapon rails (e.g., Picatinny rail).
*   **Additive Manufacturing:** Optimized the design for 3D printing (FDM/SLA), selecting materials capable of withstanding mechanical vibrations and outdoor conditions, and assembled the entire optoelectronic stack manually.

---

## Tech Stack & Components
*   **Microcontroller:** ESP32 (Dual-Core, high clock speed for calculation-heavy interpolation)
*   **Sensor:** MLX90640 Thermal Camera Array
*   **Display:** SPI TFT/OLED Display
*   **Firmware:** C++ (with low-level I2C and SPI display drivers)
*   **Hardware Design:** Blender / CAD Modeling, FDM 3D Printing, Manual Soldering & Assembly

---

Source Code Not Available

---

## Gallery

![Thermal Scope Attachment IMG](/images/ir-cam-img-0.png)
![Thermal Scope Attachment IMG](/images/ir-cam-img-1.png)