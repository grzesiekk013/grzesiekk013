import asyncio, websockets, time

#  -- -- CONFIG -- --
HOST: str = "127.0.0.1"
PORT: int = 30001

# -- -- CODE -- --
async def run_client():
   async with websockets.connect(f"ws://{HOST}:{PORT}") as ws:
      await ws.send("!get_robot")
      response = await ws.recv()
      print(f"Got Response: {response}")

if __name__ == '__main__':
    while True:
       asyncio.run(run_client())
       time.sleep(2)