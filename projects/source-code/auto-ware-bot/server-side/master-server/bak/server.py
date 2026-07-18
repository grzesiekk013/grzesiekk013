import asyncio
import websockets
from datatypes import *
# from datatypes import AutoWareBot
import time

DEBUG_PRINT: bool = True
CARTON_ROBOT: AutoWareBot = AutoWareBot()

idx = 0
cmd_send: bool = True
PATH_COMMANDS = None
last_any_cmd_time = 0
rate_limit_sec = 1
once = False

async def handle_connection(websocket):
    global idx, VALUES, CARTON_ROBOT, DEBUG_PRINT, cmd_send, PATH_COMMANDS, once
    try:
        async for msg in websocket:
            # if DEBUG_PRINT:
            #     print(f'[RECEIVED] {msg}')
            # # -- -- serwer odbiera zapytanie czy robot czeka na jakies polecenia
            # if msg.startswith(CommunicationPrefixes.ANY_CMD.value):
            #
            #     _msg = f'{CommunicationPrefixes.ANY_CMD.value}{len(CARTON_ROBOT.COMMANDS)}{CommunicationPrefixes.ANY_CMD.value[::-1]}'
            #     await websocket.send(_msg)
            #     print(f"[SEND] {_msg}")
            # # -- -- odbiór danych lidar
            # elif msg.startswith(CommunicationPrefixes.LIDAR.value):
            #     _msg = msg.replace(CommunicationPrefixes.LIDAR.value, '')
            #     _msg = _msg.replace(CommunicationPrefixes.LIDAR.value[::-1], '')
            #     if CARTON_ROBOT.save_lidar_data(_msg):
            #         start_ix = int(CARTON_ROBOT.POSITION.X / 5) + 200
            #         start_iy = int(CARTON_ROBOT.POSITION.Y / 5) + 200
            #         curr_yaw_norm = CARTON_ROBOT.POSITION.YAW % 360
            #         start_dir = int(round(curr_yaw_norm / 90)) % 4
            #
            #         goal_ix = np.clip(CARTON_ROBOT.TARGET_X, 0, 400)
            #         goal_iy = np.clip(CARTON_ROBOT.TARGET_Y, 0, 400)
            #
            #         start_pose = (start_iy, start_ix, start_dir)
            #         goal_pos = (goal_iy, goal_ix)
            #
            #         path, came_from, final_node = kinematic_astar(GRID_MAP, start_pose, goal_pos)
            #
            #         if path:
            #             PATH_COMMANDS = get_path_commands(came_from=came_from, final_node=final_node, start_pose=start_pose)
            #             action, count = PATH_COMMANDS[0]
            #             CARTON_ROBOT.exec_next_command(action, count)
            #
            #
            #     _msg = f'{CommunicationPrefixes.LIDAR.value}{CommandResult.OK.value}{CommunicationPrefixes.LIDAR.value[::-1]}'
            #     await websocket.send(_msg)
            #     print(f"[SEND] {_msg}")
            #
            #
            # # -- -- przetworzenie zapytania o komende i wysłanie komendy
            # elif msg.startswith(CommunicationPrefixes.GET_CMDS.value):
            #     __tmp = CARTON_ROBOT.COMMANDS.pop(0)
            #     __tmpv = CARTON_ROBOT.VALUES.pop(0)
            #     if __tmp  in range(0, 2):
            #         # weryfikacja
            #         _tmp = CARTON_ROBOT.POSITION
            #         _tmp_cnt = 0
            #
            #         for i in range(1, 4):
            #             if _tmp.YAW == 0:
            #                 for j in range(1, 9):
            #                     _tmp_cnt += GRID_MAP[CARTON_ROBOT.POSITION.X+i][CARTON_ROBOT.POSITION.Y+j]
            #
            #         if _tmp_cnt > 0:
            #             _msg = f'{CommunicationPrefixes.GET_CMDS.value}0;0{CommunicationPrefixes.GET_CMDS.value[::-1]}'
            #             await websocket.send(_msg)
            #             print(f"[SEND] {_msg}")
            #         else:
            #             _msg = f'{CommunicationPrefixes.GET_CMDS.value}{__tmp};{__tmpv}{CommunicationPrefixes.GET_CMDS.value[::-1]}'
            #             await websocket.send(_msg)
            #             if __tmp in range(0, 2):
            #                 CARTON_ROBOT.add_location(__tmpv/10 * (1 if __tmp == 0 else -1))
            #                 curr_ix = int(CARTON_ROBOT.POSITION.X / 5) + 200
            #                 curr_iy = int(CARTON_ROBOT.POSITION.Y / 5) + 200
            #                 check_area = GRID_MAP[curr_iy - 2:curr_iy + 3, curr_ix - 2:curr_ix + 3]
            #                 if np.any(check_area == 1.0):
            #                     _msg = f'{CommunicationPrefixes.GET_CMDS.value}0;0{CommunicationPrefixes.GET_CMDS.value[::-1]}'
            #                     # Jeśli widzisz przeszkodę, wyczyść stare komendy, by wymusić re-planning
            #                     CARTON_ROBOT.COMMANDS.clear()
            #                     CARTON_ROBOT.VALUES.clear()
            #                     await websocket.send(_msg)
            #                     continue
            #                 else:
            #                     _msg = f'{CommunicationPrefixes.GET_CMDS.value}{__tmp};{__tmpv}{CommunicationPrefixes.GET_CMDS.value[::-1]}'
            #                     await websocket.send(_msg)
            #                     if __tmp in range(2, 4):
            #                         CARTON_ROBOT.add_rotation(__tmp * 2 - 1, __tmpv * 90)
            #                     continue
            #         # -- -- odbiór informacji o zakończeniu polecenia
            # elif msg.startswith(CommunicationPrefixes.CMD_DONE.value):
            #     _msg = f'{CommunicationPrefixes.CMD_DONE.value}0{CommunicationPrefixes.CMD_DONE.value[::-1]}'
            #     await websocket.send(_msg)
            #     print(f"[SEND] {_msg}")
            if DEBUG_PRINT: print(f'[RECEIVED] {msg}')

            if msg.startswith(CommunicationPrefixes.ANY_CMD.value):
                # Informujemy robota, ile komend czeka w kolejce
                current_time = time.time()
                if current_time - last_any_cmd_time < rate_limit_sec:
                    _msg = f'{CommunicationPrefixes.ANY_CMD.value}{len(CARTON_ROBOT.COMMANDS)}{CommunicationPrefixes.ANY_CMD.value[::-1]}'
                    await websocket.send(_msg)
                    print(f'[WySŁANO] {_msg}')
                elif once == False:
                    _msg = f'{CommunicationPrefixes.ANY_CMD.value}1{CommunicationPrefixes.ANY_CMD.value[::-1]}'
                    await websocket.send(_msg)
                    print(f'[WySŁANO] {_msg}')
                    once = True
                else:
                    _msg = f'{CommunicationPrefixes.ANY_CMD.value}0{CommunicationPrefixes.ANY_CMD.value[::-1]}'
                    await websocket.send(_msg)
                    print(f'[WySŁANO] {_msg}')
            elif msg.startswith(CommunicationPrefixes.LIDAR.value):
                _msg_clean = msg.replace(CommunicationPrefixes.LIDAR.value, '').replace(
                CommunicationPrefixes.LIDAR.value[::-1], '')
                if CARTON_ROBOT.save_lidar_data(_msg_clean):
                    # Rysujemy dane i planujemy ścieżkę tylko jeśli kolejka jest pusta (re-planning)
                    CARTON_ROBOT.draw_lidar_data()

                    if not CARTON_ROBOT.COMMANDS:
                        start_ix = np.clip(int(CARTON_ROBOT.POSITION.X / 5) + 200, 0, 400)
                        start_iy = np.clip(int(CARTON_ROBOT.POSITION.Y / 5) + 200, 0, 400)
                        start_dir = int(round((CARTON_ROBOT.POSITION.YAW % 360) / 90)) % 4

                        goal_ix = np.clip(int(CARTON_ROBOT.TARGET_X / 5) + 200, 0, 400)
                        goal_iy = np.clip(int(CARTON_ROBOT.TARGET_Y / 5) + 200, 0, 400)

                        path, came_from, final_node = kinematic_astar(GRID_MAP, (start_iy, start_ix, start_dir),
                                                                      (goal_iy, goal_ix))

                        if path:
                            cmds = get_path_commands(came_from, final_node, (start_iy, start_ix, start_dir))
                            if cmds:
                                # DODAJEMY pierwszy manewr do wykonania
                                action, count = cmds[0]
                                CARTON_ROBOT.exec_next_command(action, count)

                await websocket.send(f'{CommunicationPrefixes.LIDAR.value}0{CommunicationPrefixes.LIDAR.value[::-1]}')

            elif msg.startswith(CommunicationPrefixes.GET_CMDS.value):
            # Pobieramy komendę tylko jeśli istnieje
                if CARTON_ROBOT.COMMANDS:
                    cmd = CARTON_ROBOT.COMMANDS.pop(0)
                    val = CARTON_ROBOT.VALUES.pop(0)

                    # Prosta weryfikacja bezpieczeństwa przed wysłaniem ruchu
                    if cmd in ["1", "0"]:  # FWD/BWD
                        curr_ix = int(CARTON_ROBOT.POSITION.X / 5) + 200
                        curr_iy = int(CARTON_ROBOT.POSITION.Y / 5) + 200
                        # Sprawdzamy mały obszar przed robotem na mapie
                        if np.any(GRID_MAP[curr_iy - 1:curr_iy + 2, curr_ix - 1:curr_ix + 2] == 1.0):
                            cmd, val = "0", 0  # Zatrzymaj jeśli przeszkoda
                            CARTON_ROBOT.COMMANDS.clear()
                            CARTON_ROBOT.VALUES.clear()
                else:
                    cmd, val = "0", 0  # Brak komend

                # WYSYŁAMY TYLKO RAZ
                _resp = f'{CommunicationPrefixes.GET_CMDS.value}{cmd};{val}{CommunicationPrefixes.GET_CMDS.value[::-1]}'
                await websocket.send(_resp)

                # Aktualizujemy lokalną wiedzę o pozycji robota po wysłaniu komendy
                if val > 0:
                    if cmd == "1":
                        CARTON_ROBOT.add_location(val / 10)  # Przelicz mm/cm na jednostki mapy
                    elif cmd == "0":
                        CARTON_ROBOT.add_location(-val / 10)
                    elif cmd == "2":
                        CARTON_ROBOT.add_rotation(1, 90)
                    elif cmd == "3":
                        CARTON_ROBOT.add_rotation(-1, 90)

            elif msg.startswith(CommunicationPrefixes.CMD_DONE.value):
                await websocket.send(f'{CommunicationPrefixes.CMD_DONE.value}0{CommunicationPrefixes.CMD_DONE.value[::-1]}')
                once = False


    except websockets.exceptions.ConnectionClosedOK:
        print("[ZAKOŃCZONO POŁĄCZENIE]")



async def main():
    async with websockets.serve(handle_connection,"0.0.0.0", 8765):
        await asyncio.Future()

if __name__ == "__main__":
    asyncio.run(main())
