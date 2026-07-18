import asyncio, websockets

print("="*50)
print("Dostępne polecenia:")
print("!new_target;x_cm;y_cm;tegrat_wen!")
print("="*50)
msg = ""
async def client():
    global state, msg
    url = "ws://127.0.0.1:8765"
    try:
        async with websockets.connect(url) as websocket:
            print("=== POŁĄCZONO ===")

            while True:
                try:
                    msg = await asyncio.to_thread(input, "/> ")
                    if not msg:
                        continue
                    await websocket.send(msg.replace(' ', ''))
                    print(f"[WYSŁANO] {msg}")
                    response = await websocket.recv()
                    print(f"[ODEBRANO] {response}")
                    msg = ""
                except Exception as ex :
                    print(f"[BŁĄD] {ex}")

                await asyncio.sleep(0.01)
    except Exception as ex:
        print(f"[BŁĄD] {ex}")

asyncio.run(client())
