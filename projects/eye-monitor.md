# EYE: Environmental Monitoring & Workspace Optimization System
### Secure, Multi-Threaded IoT Platform (ESP32-S3 & Django)

An advanced, end-to-end IoT ecosystem designed to track indoor environmental variables (air quality, CO2, temperature, humidity) that directly impact cognitive performance and study/work efficiency. The system automates real-time comfort optimization by triggering targeted alerts and actions when parameters exceed defined thresholds.

---

> **My Role: Project Lead & Full-Stack Systems Architect**
> As the leader of this project, I architected the entire end-to-end system, designed the user interface (UI/UX), designed and programmed the MySQL database, wrote the most Django backend, soldered and integrated the physical hardware, and authored the vast majority of the dual-core C++ firmware.

---

## Key Engineering Highlights

### 1. Dual-Core Multi-Threaded C++ Firmware (ESP32-S3)
*   **Hardware Platform:** FireBeetle 2 ESP32-S3-WROOM.
*   **Task Isolation:** I engineered a highly responsive firmware architecture utilizing the dual-core processor. Pinning tasks to specific cores ensures that high-frequency sensor sampling never blocks or delays critical network communication or encryption loops.
*   **Persistent Storage:** I utilized the non-volatile Flash memory (`Preferences` library) to securely store network configurations, encryption keys, and system states across reboot cycles.
*   **I2C Sensor Cluster:** Coordinated and soldered the multi-sensor data acquisition over the I2C bus, integrating:
    *   **SCD4x (SEN-22396):** Photoacoustic CO2, temperature, and humidity sensor.
    *   **ENS160:** Digital metal-oxide multi-gas sensor for volatile organic compounds (VOCs) and AQI.
    *   **MLX90614 (GY-906):** Contactless infrared temperature sensor.

### 2. Hardware-Level Cryptography (AES-256)
*   **Data Security:** I implemented an onboard symmetric cryptographic pipeline utilizing **AES-256** encryption to protect telemetry payloads.
*   **Unique Key Derivation:** To prevent spoofing and eavesdropping on the network, the 256-bit encryption key is dynamically generated using the device's hardware UUID.
*   **Payload Delivery:** Data is packed into a secure JSON structure, encrypted on-chip, and transmitted over HTTPS via a REST API to the backend server.

### 3. Interactive Serial Configuration Utility (UART Terminal)
*   **Provisioning Tool:** I developed a robust, interactive command-line interface accessible via the serial port. 
*   **Capabilities:** Allows technicians to configure local Wi-Fi credentials, update API endpoints, provision encryption keys, synchronize system time via NTP, and run real-time sensor diagnostic self-tests before actual physical deployment.

### 4. Distributed Backend & Real-Time Web Dashboard
*   **Server Stack:** Hosted on a lightweight **OrangePi Zero 3 (Linux)** running a **Django (Python)** framework backed by a **MySQL** relational database.
*   **Real-time Analytics:** I designed the UI/UX, database schema, and backend logic featuring customizable thresholds (incorporating fuzzy logic comfort zones) and live room-by-room telemetry plotting using interactive charts.
*   **Automation:** Monitors incoming decrypted JSON payloads and triggers instant alerts or webhook-based reactive events when air quality deteriorates.

---

## Tech Stack & Hardware Components
*   **Role:** Project Lead, System Architect & Full-Stack Developer
*   **Microcontroller:** FireBeetle 2 ESP32-S3-WROOM (Dual-Core Tensilica LX7)
*   **Embedded Firmware:** C++ (Arduino Core, FreeRTOS, ESP32 Threading, Wire API)
*   **Sensors:** ENS160 (AQI/VOC), MLX90614 (Contactless Temp), SCD4x (CO2/Temp/RH)
*   **Networking & Gateway:** Wi-Fi (HTTPClient), MikroTik Routing, NTP Time Sync
*   **Server & DB:** Django (Python), MySQL Database, OrangePi Zero 3 Linux Server
*   **Cryptography:** AES-256 (Symmetric Encryption)

---

### Encrypted Telemetry Payload Structure

```json
// Example of the secure JSON packet sent by the ESP32-S3
{
  "device_uuid": "ESP32-S3-5C-E0-C5-3A-7F-9B",
  "payload": "U2FsdGVkX19qSjhEMzRkeWq89Y+RjLp8Vq1mJzR0Wk..." // AES-256 Encrypted Telemetry Data
}
```

## Gallery

### Hardware 
![Hardware](/images/eye-hardware-0.png)
### ERD Diagram
![ERD Diagram](/images/eye-erd-diagram.png)
### [Webpage Showcase](/images/eye-webpage.mp4)
### [Hardware Video]("/images/eye-hardware.mp4)

---

[Source Code](source-code/eye-monitor)  
[Docs](/docs/eye-monitor/eye-monitor-presentation.pdf)

---

**Jakub Lidzbarski** — Co-Author - Encryption  
**Kamil Grzybek** — Co-Author - Database fillup  
**Jarosław Wojciuk** — Co-Author - Frontend implementation
