## Hi there, I'm Grzegorz 👋
### IoT & Embedded Systems Engineer | Simulation & R&D Specialist

I am a versatile Software and Hardware Engineer, graduate of Gdynia Maritime University (with a honors degreeL 5.5/A). I specalize in bridging the gap between physical hardware, low-level firmware and high-fidelity virtual simulations (Unreal Engine).

---

## Table of Contents
* [Tech Stack & Skills](#tech-stack-and-skills)
* [Featured Projects](#featured-projects)
* [Commercial R&D Impact & Global Exhibitions](#commercial-r-and-d)
* [Other R&D & Hardware Projects](#other-r-and-d)
* [Certifications & Vocational Qualifications](#certifications)
* [Quick Hobby & Software Experiments](#hobby)
* [Quick Code Guide for Recruiters](#recruiters)
---
<a name="tech-stack-and-skills"></a>
## Tech Stack & Skills

*   **Languages:** Python, C++, Java (academic), C# (academic), Bash, Batch, SQL, Lua
*   **Embedded & IoT:** RP2040, ESP32, STM32, Betaflight, I2C / SPI / UART / MODBUS
*   **Simulations & 3D:** Unreal Engine 5, AirSim, JSBSim, Blender, AutoCAD, FDM/SLA 3D Printing
*   **Systems & Virtualization:** Linux & Windows (Administration, Custom Scripting, OS Deployment), XCP-ng Hypervisor
*   **Networks & Infrastructure:** MikroTik, Cisco (academic), OpenWrt, pfSense, QNAP NAS, Samba, FTP
*   **Hardware & Diagnostics:** PC Diagnostics, Assembly & Hardware Testing, Soldering (THT)
*   **AI/ML:** OpenAI / Gemini API integration, Local LLMs, Object Detection (YOLO)

---
<a name="featured-projects"></a>
## Featured Projects (Main showcases)

Click on the links below to see dedicated pages with detailed descriptions.  
### **[1. Mini-Redis as In-Memory Cache Engine](https://github.com/grzesiekk013/mini-redis)**
   <img width="1000" height="100" alt="Projekt bez nazwy (12)" src="https://github.com/user-attachments/assets/257d285c-3a31-4f16-b998-c57ee8868371" />

   * **What it is:** A minimalist, high-performance in-memory key-value storage engine built from scratch to demonstrate low-level memory efficiency, persistence, and precise time tracking without external dependencies.
   * **Key features:** Custom TTL expiration via `std::chrono`, automated CSV serialization/deserialization, type-safe null handling using `std::optional`, RAII-based memory management.
   * **Tech stack:** C++17, std::unordered_map, std::filesystem
     
### [2. UAV & Maritime Simulations](projects/simulations.md)
<img width="1584" height="396" alt="Banner" src="https://github.com/user-attachments/assets/1d99669d-7f3e-44ab-863c-8c27cc6bb05d" />

*   **What it is:** High-fidelity simulations enviroments built in *   *   **Unreal Engine** using JSBSim for testing real-case scenarios and maritime crane behaviors operating unrer wave movements.
*   **My role:** Designed the virtual physics enviroments, integrated the outputs and prepared the simulation pipeline.

  

### [3. PowerTrack (Engineering Thesis)](projects/power-track.md)
<img width="1584" height="396" alt="Banner" src="https://github.com/user-attachments/assets/abc9fa49-21c7-43ab-b832-2d48392ceb27" />

*   **What it is:** My final engineering project at Gdynia Maritime University. A smart power-tracking optimization system designed for IoT devices operating in professional enviroments.
*   **My role:** Full stack development from hardware selection to the backend database and user panel.
*   **Tech stack:** LuckFox SBC, Python, Django, SQL


### [4. Hardware-In-The-Loop (HITL) Simulation & VR Integration](projects/hitl.md)
<img width="1584" height="396" alt="Banner" src="https://github.com/user-attachments/assets/f71f3217-aa58-4c54-a1fa-9d803c57f0f3" />

*   **What it is:** An HITL testing rig streaming real-time video feed from an Unreal Engine drone simulation directly to physical FPV video tramsitters (VTX) and VTX video recerivers / Meta Quest VR googles.
*   **My role:** Designed the video transmission and mapped the physical controller output to control the simulated drone and receive it's POV on gogles.
*   **Tech stack:** C++, Unreal Engine, Python, FPV VTX hardware

---
<a name="commercial-r-and-d"></a>
## Commercial R&D Impact & Global Exhibitions

While the core enterprise codebase remains proprietary, I architected and built the simulation environments, physics models, and hardware integrations for systems successfully showcased at major international aerospace and defense exhibitions:

### Drone Expo 2026 | Quadcopter HITL Simulation & Hardware Platform
*   **Role:** Lead Simulation & Hardware Developer
*   **Core Contributions:**
    *   **Simulation & Physics:** Developed the complete quadcopter simulation from scratch, including custom environment modeling, drone flight physics, a dynamic weather configuration application, and real-time GPS localization.
    *   **Hardware Assembly:** Built the physical quadcopter drone from the ground up, which was utilized for the physical demonstration side of the setup.
    *   **RF & VTX Integration:** Integrated a live video transmitter (VTX) system that streamed the virtual camera feed from the simulation directly to physical field monitors and FPV goggles over the air (proving successful real-world RF broadcasting during the exhibition).
*   **System Architecture:** Developed as a Hardware-in-the-Loop (HITL) system where the physical drone mirrored the real movements of the simulation. *(Note: Radio signal routing/splitting and the basic Python/C++ communication layer were co-developed with the Lead Architect).*
<p align="center">
<img width="695" height="389" alt="image" src="https://github.com/user-attachments/assets/eea6155d-099e-4fe3-9f11-34f098147ced" />
</p>
[Showcase Video](/images/drone_expo26.mp4)   


---

### Seoul ADEX 2025 | Fixed-Wing Flight Simulation (JSBSim & Unreal Engine)
*   **Role:** Systems Integration, Telemetry Developer & Simulation Developer
*   **Core Contributions:**
    *   **Flight Dynamics Integration:** Engineered a full, integration between **Unreal Engine** and **JSBSim** (an open-source Flight Dynamics Model framework) using Python.
    *   **Geospatial Mapping App:** Developed a dedicated tracking application that captured the drone's live GPS telemetry data from the simulation and overlaid it in real-time onto a digital street map interface for operator tracking.
*   **System Architecture:** Implemented high-performance, real-time data exchange utilizing Python scripts. *(Note: The underlying low-level RAM-disk file-based communication channel between Python and Unreal Engine was co-developed with the Lead Architect).*
<p align="center">
<img width="343" height="376" alt="image" src="https://github.com/user-attachments/assets/ad99795d-62f9-4c9b-8b45-0b513fa4bfe4" />
</p>  
  
---  
  
<a name="other-r-and-d"></a>  
  
## Other R&D & Hardware Projects

Click on the links below to see dedicated pages with detailed descriptions.

*   **[Ant Colony Maze Solver](projects/ant-colony-maze-solver.md)** - Pathfinding algorith visualizer using and colony optimization. <img width="1584" height="396" alt="Beige Simple Elegant Personal LinkedIn Banner (3)" src="https://github.com/user-attachments/assets/aaf5fcfe-fad2-4e98-8f9f-1c54905591f9" />

*   **[The Ship Game](projects/the-ship-game.md)** - Ship game in a digital environment. <img width="800" height="200" alt="Projekt bez nazwy (6)" src="https://github.com/user-attachments/assets/f2717f42-f8ea-48b3-8e0e-e07b75d10b8b" />

*   **[Auto Ware Bot](projects/auto-ware-bot.md)** - Autonomus robot built on ESP32.<img width="800" height="200" alt="Projekt bez nazwy (8)" src="https://github.com/user-attachments/assets/265aa35a-835e-42a6-a65f-b45131dca03d" />

*   **[Roller Shutter Gates Driver](projects/roller-shutter.md)** - Custom RP2040/ESP32 smart home relay driver with a mobile app.<img width="800" height="200" alt="Projekt bez nazwy (9)" src="https://github.com/user-attachments/assets/98ebe40f-eec5-401e-a588-d3940a19ff04" />

*   **[EYE Monitor](projects/eye-monitor.md)** - Smart multi-sensor IoT node for office workspace optimization.<img width="800" height="200" alt="Projekt bez nazwy (10)" src="https://github.com/user-attachments/assets/aa038edf-00c2-47ec-8824-119c2abefba0" />

*   **[Thermal Scope Attachment](projects/infrared-scope.md)** - Infrared heat-source tracker with integrated display for rifles.<img width="800" height="200" alt="Projekt bez nazwy (11)" src="https://github.com/user-attachments/assets/07dfeb1d-f27c-4ad6-8ef6-bc97ed317bd7" />

****

---
<a name="certifications"></a>
## Certifications & Vocational Qualifications
*   **EE.08 Professional Qualification** - Assembly, maintenance, and diagnostics of computer systems, peripheral devices, and networks.
*   **EE.09 Professional Qualification** - Programming, creation, and administration of websites and databases.
*   **Cisco Certification** - CCNAv7: Introduction to Networks & Cisco® IT Essentials: PC Hardware and Software

---
<a name="hobby"></a>
### Quick Hobby & Software Experiments
*   **UAV Drone Build (A to Z):** Full mechanical and electrical build of an FPV drone. Modeled frame parts in Blender, 3D printed them, soldered the electronicts, and tuned Betaflight.
*   **LUA GameDev Scripts:** Custom state machines and game logic, scripting scenarios and tests.

---
<a name="recruiters"></a>
### Quick Code Guide for Recruiters
If you are here to quickly evaluate my programming proficiency and architectural style, here is a direct map to my best and most up-to-date source code showcases:

*   **Modern C++ (C++17):** Look straight into the **[Mini-Redis Engine](https://github.com/grzesiekk013/mini-redis)** repo. It demonstrates how I handle C++ language.
*   **Python Engineering:** Check out my **[PowerTrack (Engineering Thesis)](projects/power-track.md)** for a full-stack Django & SQL setup on embedded SBCs. See pico-worker/core and backend/PowerTrack/core.

--- 

*  🌱 I’m currently learning: C++ and STM32 microcontrollers (focusing on HAL, LL drivers, and direct register access).
<!--
**grzesiekk013/grzesiekk013** is a ✨ _special_ ✨ repository because its `README.md` (this file) appears on your GitHub profile.

Here are some ideas to get you started:

- 🔭 I’m currently working on ...
- 🌱 I’m currently learning ...
- 👯 I’m looking to collaborate on ...
- 🤔 I’m looking for help with ...
- 💬 Ask me about ...
- 📫 How to reach me: ...
- 😄 Pronouns: ...
- ⚡ Fun fact: ...
-->
