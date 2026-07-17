# AreoVis: AI-Driven Emergency Response & 3D Flight Simulation System
### Autonomous Dispatch & High-Fidelity Fixed-Wing UAV Simulation (Spaceshield Hack 2026)

AreoVis is a next-generation emergency response platform inspired by the standard 112 dispatch system. It couples a user-facing web interface for live incident reporting with an AI-powered triage dispatcher and an interactive, real-time 3D reconnaissance flight simulator. The system dramatically reduces first-responder reaction times by automating drone deployments directly to reported crisis zones.

---

> **My Role: Project Lead, System Architect & Simulation Lead**
> As the leader of this hackathon team, I designed the complete system architecture linking the web-based reporting system with the Unreal Engine simulation. On the simulation front, I designed the virtual environment, modeled the fixed-wing drone, programmed the high-fidelity aerodynamic flight model using **JSBSim**, and developed the raw gamepad integration script using **PyGame** to allow seamless hardware-controlled manual piloting.

---

## Key Engineering Highlights

### 1. End-to-End System Architecture (Designed by Me)
*   **The Pipeline:** Designed a robust Client-Server-Simulation workflow. When a user reports an incident, the data flows to a centralized **Flask (Python)** server, passes through an **Ollama AI** model for triage routing, and instantly initializes a drone flight path inside **Unreal Engine 5** corresponding to the precise GPS/map coordinates of the emergency.
*   **Decoupled Architecture:** Kept the database-backed web layer separate from the heavy 3D rendering engine, enabling reliable operation and smooth dashboard visualization even while the simulation executes intensive physical calculations.

### 2. High-Fidelity 3D Flight Simulation & Aerodynamics
*   **Unreal Engine Environment:** Generated a realistic 3D map of the city of Stalowa Wola to serve as the emergency flight airspace, ensuring accurate topographical testing and visual situational awareness.
*   **JSBSim Flight Dynamics:** Programmed the flight physics of the **fixed-wing drone** using the open-source **JSBSim** engine. This allowed realistic aerodynamics, wind resistance forces, and inertia calculations instead of arcade-like translation movements.
*   **PyGame Input Handler:** Developed a custom Python gamepad input handler (`/UnrealPycode`) using **PyGame** to interface physical controllers with Unreal Engine, simulating a real-world ground control station (GCS) joystick pilot interface.

### 3. AI Dispatcher & Dashboard Triage
*   **Intelligent Routing:** Integrated a local **Ollama** LLM model to analyze emergency text descriptions in real time. The AI automatically classifies the crisis severity, decides whether to dispatch a reconnaissance drone, and routes the ticket to the proper municipal agency (Police, Fire Department, etc.).
*   **Multi-Tiered Command Panels:** Built custom dashboard views including a Global Command Panel for overall operations and separate secure panels tailored for specific unit commanders to mark crises as "Resolved."

---

## Tech Stack & Architecture
*   **Role:** Project Lead, System Architect & Lead Simulation Developer
*   **Simulation Engine:** Unreal Engine 5 (UE5)
*   **Flight Physics:** JSBSim Flight Dynamics Model (FDM)
*   **Hardware Interface:** PyGame (gamepad/controller integration)
*   **Backend & Server:** Flask (Python), Ollama Local AI integration
*   **Frontend:** HTML5, CSS3, JS (Interactive Leaflet Map/Coordinates)

---

## Gallery
Create a Ticket   
![Client Side reporting View](/images/areovis-img-0.png)  
AI Recommendation  
![AI Recommendation](/images/areovis-img-1.png)  
Drones Management View  
![Drones Management View](/images/areovis-img-3.png)  
Simulation Screenshots  
![Simulation Screenshot](/images/areovis-img-4.png)
![Simulation Screenshot 2](/images/areovis-img-5.png)

---

Source Code Not Available

---

* **Jakub Lidzbarski** — Co-Author - Unreal Engine Cesium Integration 
* **Kamil Łania** — Co-Author - Web App Development