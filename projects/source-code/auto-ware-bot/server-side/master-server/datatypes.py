from dataclasses import dataclass
from enum import IntEnum, StrEnum
from turtledemo.penrose import start

import matplotlib.pyplot as plt
import numpy as np
from matplotlib.collections import LineCollection
import heapq, math
import scipy.ndimage as nd


"""
    any_cmd
    lidar
    get_cmd
    cmd_done
    lidar
"""



GRID_MAP = np.zeros((401,401))

N = 180
katy_stopnie = np.linspace(90, 360+90, N, endpoint=False)
katy_rad = np.deg2rad(katy_stopnie)
dystanse = np.zeros(N)

plt.ion()

fig = plt.figure(figsize=(16, 7))

ax = fig.add_subplot(1, 3, 1, projection='polar')
ax.set_ylim(0, 800)
ax.set_theta_zero_location('W')
ax.set_theta_direction(-1)
ax.set_title('LIDAR LIVE', va='bottom')

scat = ax.scatter(katy_rad, dystanse, color='red', s=20, edgecolors='none')

sags = [[[0, 0], [k, 0]] for k in katy_rad]
lines = LineCollection(sags, colors='gray', linewidths=0.5, alpha=0.3)
ax.add_collection(lines)

ax_map = fig.add_subplot(1, 3, 2)
ax_map.set_title('MAPA 20X20 [m]')
ax_map.set_xlabel('X [m]')
ax_map.set_ylabel('Y [m]')

ax_debug = fig.add_subplot(1, 3, 3)
ax_debug.set_title('DEBUG A*')
ax_debug.set_xlabel('X [m]')
debug_img = ax_debug.imshow(GRID_MAP, origin='lower', cmap='hot', vmin=0, vmax=1, extent=[-10,10, -10, 10])
map_img = ax_map.imshow(GRID_MAP, origin='lower', cmap='Greys', vmin=0, vmax=1, extent=[-10,10, -10, 10]) # extent=[0, 400, 0, 400
robot_body, = ax_map.plot([], [], 'ro', markersize=8, zorder=10)
robot_nose, = ax_map.plot([], [], 'r-', linewidth=4, zorder=10)
info = ax_map.text(0.02, 0.98, "0x0r0", transform=ax_map.transAxes,  fontsize=10)
target, = ax_map.plot([], [], 'gx', markersize=8, zorder=11)
path_line, = ax_map.plot([], [], 'g-', linewidth=2, zorder=11)

inflated_map = nd.binary_dilation(GRID_MAP, structure=np.ones((3,3)))

def kinematic_astar(grid, start_pose, goal_pos):
    rows, cols = grid.shape

    rows, cols = grid.shape
    start_node = (0, start_pose[0], start_pose[1], start_pose[2])
    pq = [start_node]
    cost_so_far = {start_pose: 0}
    came_from = {start_pose: None}
    STEP_PX = 2
    DIR_DELTAS = [(0, 1), (1, 0), (0, -1), (-1, 0)]
    # DIR_DELTAS = [(1,0), (0,1), (-1, 0), (0, 0)]
    goal_y, goal_x = goal_pos
    while pq:
        current_cost, cy, cx, cdir = heapq.heappop(pq)
        if (cy - goal_y) ** 2 + (cx - goal_x) ** 2 <= 4:  # 2^2
            break
        next_moves = []
        dy, dx = DIR_DELTAS[cdir]
        next_moves.append(((cy + dy * STEP_PX, cx + dx * STEP_PX, cdir), 0.5, "FWD"))
        next_moves.append(((cy - dy * STEP_PX, cx - dx * STEP_PX, cdir), 2.8, "BWD"))
        next_moves.append(((cy, cx, (cdir + 1) % 4), 2, "ROT_L"))
        next_moves.append(((cy, cx, (cdir + 3) % 4), 2, "ROT_R"))

        for next_state, move_cost, action_name in next_moves:
            ny, nx, ndir = next_state

            if not (0 <= ny < rows and 0 <= nx < cols):
                continue

            if grid[int(ny), int(nx)] == 1.0:
                continue

            new_cost = cost_so_far[(cy, cx, cdir)] + move_cost

            if new_cost < cost_so_far.get((ny, nx, ndir), float('inf')):
                cost_so_far[(ny, nx, ndir)] = new_cost

                h = math.sqrt((ny - goal_y) ** 2 + (nx - goal_x) ** 2)
                priority = new_cost + h

                heapq.heappush(pq, (priority, ny, nx, ndir))
                came_from[(ny, nx, ndir)] = (cy, cx, cdir, action_name)

    final_node = None
    min_dist = float('inf')
    for (y, x, d) in came_from.keys():
        dist = (y - goal_y)**2 + (x - goal_x)**2
        if dist <= 4 and dist < min_dist:
            min_dist = dist
            final_node = (y, x, d)
    if final_node:
        path = []
        curr = final_node
        while curr != start_pose:
            parent_data = came_from[curr]
            path.append((curr[0], curr[1]))
            curr = (parent_data[0], parent_data[1], parent_data[2])
        path.append((start_pose[0], start_pose[1]))
        path.reverse()
        return path, came_from, final_node
    else:
        return [], {}, None

def get_path_commands(came_from, final_node, start_pose):
    if not final_node:
        return []
    actions = []
    curr = final_node
    while curr != start_pose:
        parent_data = came_from[curr]
        actions.append(parent_data[3])
        curr = (parent_data[0], parent_data[1], parent_data[2])

    actions.reverse()

    optimized_commands = []
    if not actions:
        return []

    current_action = actions[0]
    count = 1

    for i in range(1, len(actions)):
        if actions[i] == current_action:
            count += 1
        else:
            optimized_commands.append((current_action, count))
            current_action = actions[i]
            count = 1
    optimized_commands.append((current_action, count))
    return optimized_commands

def update_lidar_points(new_distances):
    GRID_MAP.fill(0.0)
    dist_arr = np.array(new_distances)
    if len(dist_arr) != N:
        return
    offsets = np.column_stack((katy_rad, dist_arr))
    scat.set_offsets(offsets)
    new_segs = []
    for i in range(N):
        new_segs.append([[katy_rad[i], 0], [katy_rad[i], dist_arr[i]]])
    lines.set_segments(new_segs)

def map_lidar_to_grid(robot_x, robot_y, robot_yaw, distances):

    GRID_MAP.fill(0)
    dists_cm = np.array(distances)

    valid_mask = (dists_cm > 0 ) & (dists_cm <= 800)
    valid_dists = dists_cm[valid_mask]

    angles = katy_rad[valid_mask]

    yaw_rad = np.deg2rad(robot_yaw)
    global_angles = yaw_rad - angles # odbicie

    points_x = robot_x + valid_dists * np.cos(global_angles)
    points_y = robot_y + valid_dists * np.sin(global_angles)

    ix = (points_x / 5).astype(int) + 200
    iy = (points_y / 5).astype(int) + 200

    grid_mask = (ix >= 0) & (ix < 401) & (iy >= 0) & (iy < 401)

    GRID_MAP[iy[grid_mask], ix[grid_mask]] = 1.0

    map_img.set_data(GRID_MAP)

@dataclass
class LocationAndRotation:
    X: float
    Y: float
    YAW: float


class CommunicationPrefixes(StrEnum):
    LIDAR = '!lidar;'
    ERROR = '!error;'
    CMD_RES = '!cmd_res;'
    GET_CMDS = '!get_cmds;'
    ANY_CMD = "!any_cmd;"
    CMD_DONE = "!cmd_done;"
    NEW_TARGET = "!new_target;"


class AutoWareBot:
    def __init__(self):
        self.LIDAR_DATA: list[int] = []
        self.LIDAR_DATA_LENGTH: int = -1
        self.POSITION: LocationAndRotation = LocationAndRotation(0, 0, -90)
        self.TARGET_X: int = 0
        self.TARGET_Y: int = 0
        self.COMMANDS: [] = []
        self.VALUES: [] = []

    def exec_next_command(self, action_name, count):
        step = 100

        if action_name == "FWD":
            print("Dodaje do przodu")
            for i in range(1, 1+count):
                self.COMMANDS.append("1")
                self.VALUES.append(step)
        elif action_name == "BWD":
            print("Dodaje do tyłu")
            for i in range(1, 1 + count):
                self.COMMANDS.append("0")
                self.VALUES.append(step)
        elif action_name == "ROT_L":
            print("Dodaje w lewo")
            for i in range(1, 1 + count):
                self.COMMANDS.append("2")
                self.VALUES.append(1)
        elif action_name == "ROT_R":
            print("Dodaje w prawo")
            for i in range(1, 1 + count):
                self.COMMANDS.append("3")
                self.VALUES.append(1)

    def get_position(self) -> LocationAndRotation:
        return self.POSITION

    def goto_target(self, target_x_m, target_y_m):

        target_x_cm = target_x_m * 100
        target_y_cm = target_y_m * 100

        dx = self.POSITION.X - target_x_cm
        dy = self.POSITION.Y - target_y_cm
        dist_eucl = math.sqrt(dx ** 2 + dy ** 2)

        if dist_eucl < 11.0:
            print(f"ROBOT DOTARŁ DO CELU.")
            path_line.set_data([], [])
            self.COMMANDS.clear()
            self.VALUES.clear()
            return

        if len(self.COMMANDS) > 0:
            return

        start_ix = int(self.POSITION.X / 5) + 200
        start_iy = int(self.POSITION.Y / 5) + 200

        curr_yaw_norm = self.POSITION.YAW % 360
        start_dir = int(round(curr_yaw_norm / 90)) % 4

        goal_ix = int(target_x_m * 100 / 5) + 200
        goal_iy = int(target_y_m * 100 / 5) + 200

        start_ix = np.clip(start_ix, 0, 400)
        start_iy = np.clip(start_iy, 0, 400)
        goal_ix = np.clip(goal_ix, 0, 400)
        goal_iy = np.clip(goal_iy, 0, 400)

        start_pose = (start_iy, start_ix, start_dir)
        goal_pos = (goal_iy, goal_ix)

        safety_margin = np.ones((6,6))
        inflated_grid = nd.binary_dilation(GRID_MAP, structure=safety_margin).astype(float)

        debug_img.set_data(inflated_grid)

        for line in ax_debug.lines:
            line.remove()

        rx_m = self.POSITION.X / 100.0
        ry_m = self.POSITION.Y / 100.0
        ax_debug.plot(rx_m, ry_m, 'co', markersize=6, label='Bot')

        ax_debug.plot(target_x_m, target_y_m, 'mx', markersize=8, markeredgewidth=3, label='Cel')

        _path, _came_from, _final_node = kinematic_astar(inflated_grid, start_pose, goal_pos)

        if _path:
            path_y_idx = [p[0] for p in _path]
            path_x_idx = [p[1] for p in _path]


            path_x_m = (np.array(path_x_idx) - 200) * 0.05
            path_y_m = (np.array(path_y_idx) - 200) * 0.05

            path_line.set_data(path_x_m, path_y_m)


            ax_debug.plot(path_x_m, path_y_m, 'y-', linewidth=1.5, alpha=0.7)


            cmds = get_path_commands(_came_from, _final_node, (start_iy, start_ix, start_dir))
            if cmds:
                action, count = cmds[0]
                self.exec_next_command(action, count)
        else:
            print(f"Cel nieosiągalny! Start: {start_ix},{start_iy} | Cel: {goal_ix},{goal_iy}")
            path_line.set_data([], [])

    def add_location(self, distance):
        self.POSITION.YAW = self.POSITION.YAW % 360
        if self.POSITION.YAW == 0:
            self.POSITION.X += distance
        elif self.POSITION.YAW == 90:
            self.POSITION.Y += distance
        elif self.POSITION.YAW == 180:
            self.POSITION.X -= distance
        elif self.POSITION.YAW == 270:
            self.POSITION.Y -= distance
        else:
            raise ValueError("addlocation ")
    def add_rotation(self, direction, angle):
        self.POSITION.YAW += direction*angle % 360
    def update_position(self, x, y, yaw):
        self.POSITION.X = x
        self.POSITION.Y = y
        self.POSITION.YAW = yaw

    def save_lidar_data(self, msg) -> bool:
        clean_msg = msg.replace(CommunicationPrefixes.LIDAR.value, '')
        clean_msg = clean_msg.replace(CommunicationPrefixes.LIDAR.value[::-1], '')
        parts = clean_msg.replace(';', ',').split(',')
        parts = [p for p in parts if p]

        try:
            declared_len = int(parts[0])
            data_values = [int(x) for x in parts[1:]]

            if len(data_values) >= declared_len:
                self.LIDAR_DATA.clear()
                self.LIDAR_DATA = [800 if int(x) < 10 else x for x in data_values[:declared_len]]
                # self.LIDAR_DATA = self.LIDAR_DATA[::-1]
                print(self.LIDAR_DATA[0])
                self.LIDAR_DATA_LENGTH = declared_len
                return True
            return False
        except (ValueError, IndexError):
            return False

    def draw_lidar_data(self) -> bool:

        if len(self.LIDAR_DATA) == N:
            update_lidar_points(self.LIDAR_DATA)

            map_lidar_to_grid(
                self.POSITION.X,
                self.POSITION.Y,
                self.POSITION.YAW,
                self.LIDAR_DATA
            )
            rx_m = self.POSITION.X / 100
            ry_m = self.POSITION.Y / 100

            robot_body.set_data([rx_m], [ry_m])
            arrow_len = 0.3

            nose_x = rx_m + np.cos(np.deg2rad(self.POSITION.YAW)) * arrow_len
            nose_y = ry_m + np.sin(np.deg2rad(self.POSITION.YAW)) * arrow_len

            status = (
                f"POS X: {self.POSITION.X} cm\n"
                f"POS Y: {self.POSITION.Y} cm\n"
                f"AZIMUTH: {self.POSITION.YAW}°\n"
                f"TARGET X: {self.TARGET_X} cm\n"
                f"TARGET Y: {self.TARGET_Y} cm\n"
            )
            info.set_text(status)

            robot_nose.set_data([rx_m, nose_x], [ry_m, nose_y])
            target.set_data([self.TARGET_X/100], [self.TARGET_Y/100])

            target_mx = self.TARGET_X / 100.0
            target_my = self.TARGET_Y / 100.0
            self.goto_target(target_mx, target_my)

            fig.canvas.draw_idle()
            fig.canvas.flush_events()
            return True
        return False

class Command(IntEnum):
    MOVE_FWD = 0
    MOVE_BWD = 1
    ROT_LFT = 2
    ROT_RGT = 3

class CommandResult(IntEnum):
    OK = 0
    OBSTLE = 1

@dataclass
class CommandResultValue:
    result: CommandResult
    value: float

@dataclass
class LidarReading:
    model: str
    data: list[int]
    divider: int

@dataclass
class RobotCommand:
    cmd: Command
    param: int
    result: CommandResultValue