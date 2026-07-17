# Ant Colony Maze Solver
### Swarm Intelligence Simulation & Dual-Heatmap Visualization (C# .NET)

An interactive, high-performance desktop application that simulates swarm intelligence using the Ant Colony Optimization (ACO) algorithm to navigate and solve dynamically generated mazes.

---

> **My Role: Project Lead, Front-End Designer & Core Backend Developer**
> As the leader of this two-person project, I drove the design and implementation from start to finish. I designed and developed the entire front-end interface (UI/UX), programmed the optimized double-buffered graphics rendering engine, and co-developed the core backend logic—including the custom graph data structures, DFS maze generation, and the multi-threaded simulation coordinator.

---

## Key Engineering Highlights

### 1. High-Performance Front-End & Dual-Heatmap Visualization
*   **Zero-Flicker Rendering:** To bypass the standard rendering limitations of Windows Forms, I engineered an optimized rendering pipeline using **Direct Bitmap manipulation** translated into `MemoryStreams` for rapid `PictureBox` updates. This allows the UI to draw hundreds of moving agents and update dense heatmaps fluidly at high framerates.
*   **Dual-Panel UI/UX:** Designed a highly responsive and intuitive split-view panel:
    *   **Ant View (Top Panel):** Displays active paths and agent density, shifting dynamically from light yellow to dark red based on real-time occupancy.
    *   **Pheromone View (Bottom Panel):** A real-time mathematical heatmap representing pheromone trails, transitioning into deep magenta/purple shades to highlight paths highly favored by the swarm.

### 2. Advanced Algorithmic Logic & Death Loop Prevention
*   **The "Death Spiral" Challenge:** In basic ACO implementations, ants often get trapped in infinite dead-end loops because local pheromones become too strong to ignore, reinforcing a sub-optimal path.
*   **Fast Reset System:** To solve this, we implemented a custom heuristic based on a *Colony Lifespan* parameter. Every ant is assigned a step counter. If an agent fails to locate the exit before its counter reaches zero, the system kills and respawns it at the starting node. This stops the loop reinforcement, allowing the pheromones in the dead-end to evaporate naturally so subsequent ants find the correct path.

### 3. Dynamic Environment & Parameter Tweakability
*   **On-the-Fly Adjustments:** Built a comprehensive control panel allowing users to adjust critical mathematical variables of the ACO algorithm in real-time (Ant Count, Pheromone Deposit/Evaporation rates, and Lifespan limits).
*   **DFS Maze Generator:** Implemented a Depth-First Search (DFS) recursive-backtracking maze generator that carves out complex, guaranteed-solvable grids based on custom width/height inputs.

### 4. Robust Backend & Concurrency
*   **Custom Graph Representation:** Represented the maze internally as a customized graph utilizing optimized Vertices and Edges to represent traversable pathways, streamlining the path-probability math.
*   **Multithreaded Execution:** Implemented safe background workers to run the heavy simulation loops in "Automatic Mode", keeping the UI thread fully interactive and responsive during complex calculations.

---

## Tech Stack & Architecture
*   **Role:** Project Lead, Front-End Designer & Core Backend Developer
*   **Language & Framework:** C# (.NET Framework / Windows Forms)
*   **Rendering Pipeline:** Direct Bitmap Memory I/O, GDI+
*   **Data Structures:** Custom Graph (Vertices/Edges Matrices)
*   **Concurrency:** Multithreaded Background Workers (Task-Parallelism)


---

## Gallery
![Maze Solver GIF](/images/maze-solver-gif.gif)

---

[Source Code](source-code/ant-colony-maze-solver)  
[Playable Demo](source-code/ant-colony-maze-solver/demo)

---

* **Jakub Lidzbarski** — Co-Author 