# PowerTrack
### Industrial OT/IT Integration Platform for Automated Electricity Metering (Modbus RTU & Django)

*My Engineering Thesis at Gdynia Maritime University (Graduated with Honors, Grade: 5.5/A).*  
An end-to-end, budget-friendly industrial energy monitoring system designed to bridge the gap between Operational Technology (OT) and Information Technology (IT). The platform automates energy data acquisition, detects anomalies, and visualizes power metrics in real time.

---

## System Architecture: Client-Server-Worker

The entire system is designed around a decoupled, highly resilient **Client-Server-Worker** architectural pattern to ensure that network outages or server downtime do not interrupt data acquisition.

### 1. Multi-threaded IoT Gateway (Worker)
Running as a native system service on a **LuckFox** embedded computer, the gateway utilizes a **Producer-Consumer** architecture backed by an in-memory **Ramdisk Queue** to prevent flash-wear and ensure data persistence during offline states.
* **Master Thread (Supervisor):** Controls execution state, initializes sub-threads, and handles dynamic "Hot-Reload" configuration updates.
* **Polling Thread (Producer):** Queries physical **ORNO electricity meters** over **Modbus RTU (RS-485)**. It resolves Modbus data fragmentation by combining split 16-bit registers into single 32-bit float values, queuing them as JSON objects.
* **Flush Thread (Consumer):** Periodically drains the Ramdisk Queue and pushes data payloads to the centralized server via a REST API.

### 2. High-Performance Server & API
Built with **Django** utilizing the Model-View-Template (MVT) pattern and Django ORM.
* **Stateless Authentication:** Features a custom middleware layer for stateless device-to-server security.
* **Dynamic Config & Signals:** Leverages **Django Signals** to trigger instant runtime re-configurations on edge gateways (Hot-Reload) without interrupting physical metering loops.

### 3. Analytics, Reporting & Incident Management
* **Data Visualization:** Interactive, paginated dashboards with real-time and historical consumption charts.
* **Anomalies & Ticket System:** Integrated alerting for threshold overruns and a built-in issue reporting system for on-site technicians.
* **PDF Engine:** Automated generation of detailed compliance and consumption reports.

---

## Tech Stack & Hardware
* **Hardware & OT:** ORNO Modbus RTU Meters, LuckFox Embedded Computer, MikroTik Routers, RS-485 Bus
* **IoT Firmware/Worker:** Python, `pymodbus`, Ramdisk I/O, Multi-threading (Threading API)
* **Backend & Web:** Django Framework (MVT), SQLite/PostgreSQL, Django ORM
* **Data Transport:** JSON, REST API, HTTP with custom Stateless Token Middleware
* **Testing Methodology:** Black-Box Integration Testing, simulated network split (offline queue verification)

---
## Gallery
### Hardware Wiring
![Hardware Wiring](/images/power-track-hardware-0.png)
### Application Welcome Screen
![Application Welcome Screen](/images/power-track-app-1.png)
### Power Meter Details
![Power Meter Current State](/images/power-track-app-2.png)
![Power Meter Graph](/images/power-track-app-3.png)
### Infrastructure Management
![Device Management](/images/power-track-app-4.png)
### Diagram ERD
![Diagram ERD](/images/power-track-erd-diagram.png)
---

**[Source Code](source-code/power-track)**  
[Engineering Thesis Parts](/docs/power-track/Grzegorz_Lademann_Praca_Dyplomowa.pdf)  
[Docs](/docs/power-track/power-track-presentation.pdf)