import asyncio, websockets

state: int = 0

async def client():
    global state
    url = "ws://127.0.0.1:8765"

    try:
        async with websockets.connect(url) as websocket:
            print("Połączono z serwerem.")

            while True:
                print(state)
                if state == 0:
                    _cmd = f"!any_cmd;;dmc_yna!"
                    await websocket.send(_cmd)
                    raw_response = await websocket.recv()
                    cmd_cnt = int(raw_response.split(';')[1])
                    print(f"{_cmd} -> {raw_response} -> {cmd_cnt}")
                    if cmd_cnt >= 1:
                        state = 1
                elif state == 1:
                    _cmd = f"!lidar;180{",50"*180};radil!"
                    await websocket.send(_cmd)
                    raw_response = await websocket.recv()
                    err = int(raw_response.split(';')[1])
                    print(f"{_cmd} -> {raw_response} -> {err}")
                    if err == 0:
                        state = 2
                elif state == 2:
                    _cmd = f"!get_cmds;;sdmc_teg!"
                    await websocket.send(_cmd)
                    raw_response = await websocket.recv()
                    resp = raw_response.split(';')
                    print(f"{_cmd} -> {raw_response} -> {resp[1]}:{resp[2]}")
                    state = 3
                elif state == 3:
                    _cmd = f"!cmd_done;;enod_dmc!"
                    await websocket.send(_cmd)
                    resp = raw_response.split(';')
                    print(f"{_cmd} -> {raw_response} -> {resp[1]}")
                    state = 4
                elif state == 4:
                    _cmd = f"!lidar;180{",140"*180};radil!"
                    await websocket.send(_cmd)
                    raw_response = await websocket.recv()
                    err = int(raw_response.split(';')[1])
                    print(f"{_cmd} -> {raw_response} -> {err}")
                    state = 0

                await asyncio.sleep(0.002)
    except Exception as ex:
        print(f"Błąd połączenia: {ex}")

asyncio.run(client())