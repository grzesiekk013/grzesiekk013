# UAV & Maritime Flight Simulations
### Physics-Based Flight Simulators in Unreal Engine

This project showcase details three advanced Unreal Engine UAV flight simulations developed for emergency, military, and maritime R&D purposes, utilizing high-fidelity physics.

---

### 1. AreoVis: Fixed-Wing Emergency Response Simulation
*Developed during Spaceshield Hack 2026*

![Areovis Banner](/images/areovis-img-6.png)

* **What it is:** A next-generation 112 emergency response platform combining a Flask-based incident web app, AI triage dispatcher, and a 3D flight simulation of a **fixed-wing drone** over a 3D map of Stalowa Wola.
* **My Role:** I designed the entire system architecture from scratch, modeled the fixed-wing drone, created the 3D environment in Unreal Engine, and programmed the flight physics using **JSBSim**.

[Click for more](projects/areovis-uav.md)

---

### 2. FPV Kamikaze Tactical Training (Quadcopter)
*Military simulation developed for tactical drone operator training*
![FPV Kamikaze Banner](/images/fpv-kamikaze-gif.gif)
* **What it is:** A highly realistic FPV kamikaze training simulator designed for Ukrainian forces to practice hitting tank targets in dynamic scenarios.
* **My Role:** I designed and developed the simulator from end to end. I generated the custom terrain from phycical heightmaps of the **Orzysz military base** in Unreal Engine, modeled the FPV quadcopter drone.

---

### 3. Tethered Crane Drone Docking (Pendulum Drone)
*Maritime industrial physics simulation*
![Crane Drone](/images/crane-img-0.png)
* **What it is:** A simulation of a drone tethered by a rope/cable to a ship's crane (Vestdavit systems), vectoring in the X and Y planes to precisely lock a docking connector onto a moving vessel.
* **My Role:** I developed the entire simulator from scratch, programmed the pendulum tether constraints, implemented the vector thrust dynamics in Unreal Engine.

---

### Tech Stack & Tools Used
* **Simulation & Physics:** Unreal Engine, JSBSim, PyGame, Python
* **3D Design:** Blender, AutoCAD, Heightmap terrain generation
* **Backend:** Python (Flask), Ollama (Local AI integration)