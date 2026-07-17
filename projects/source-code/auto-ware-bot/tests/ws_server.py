import asyncio, websockets, random

#  -- -- CONFIG -- --
HOST: str = "127.0.0.1"
PORT: int = 30001

# -- -- ROBOT -- --
class ROBOT:
   def __init__(self):
      self.X: float = 0.0
      self.Y: float = 0.0
      self.YAW: float = 0.0
      self.L_WHEEL: float = 0.0
      self.R_WHEEL: float = 0.0
      self.L_SERVO: float = 0.0
      self.R_SERVO: float = 0.0

   def update(self):
      self.X = random.randint(-100,100)
      self.Y = random.randint(-100, 100)
      self.YAW = random.randint(0, 360)
      self.L_WHEEL = random.randint(0, 360)
      self.R_WHEEL = random.randint(0, 360)
      self.L_SERVO = random.randint(0, 360)
      self.R_SERVO = random.randint(0, 360)

   def __repr__(self):
      return f"{self.X},{self.Y},{self.YAW},{self.L_WHEEL},{self.R_WHEEL},{self.L_SERVO},{self.R_SERVO}"

   def frame(self) -> str:
      return f"!robot_data;{self};atad_torob!"

# -- -- --
_robot: ROBOT = ROBOT()

# -- -- --

# -- -- HANDLER -- --
async def receive_handler(websocket):
   global _robot
   async for msg in websocket:
      print(f"Received: {msg}")
      if "!get_robot" in msg:
         _robot.update()
         await websocket.send(_robot.frame())
# -- -- SENDER -- --

async def sender():
   async with websockets.serve(receive_handler, HOST, PORT):
      print(f"WebSocket server started on: {HOST}:{PORT}")
      await asyncio.Future()

if __name__ == '__main__':
    asyncio.run(sender())