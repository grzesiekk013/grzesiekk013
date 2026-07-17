import time, os
from core.classes.Config import *
from core.classes.Datatypes import *
from core.classes.RamdiskQueue import RamdiskQueue
from core.classes.networking.ModbusInterfaces import *
from core.classes.networking.ServerLink import ServerLink
from core.threads.FlushThread import FlushThread
from core.threads.PollingThread import PollingThread
from core.threads.MasterThread import MasterThread

"""
    Główny kod programu
"""

MODBUS_LIST: list[ModbusInterfaces] = []

# -- -- Konfiguracja -- --

CWD: str = os.getcwd()

APPLICATION_CONFIG: Config = Config(CONFS_PATH=f"{CWD}/config",CONF_NAME="application.ini")

assert APPLICATION_CONFIG.init(), "Wystąpił problem z odczytaniem konfiguracji aplikacji"

UUID: str = APPLICATION_CONFIG.read_str(SECTION=Sections.CREDENTIALS, KEY=Keys.UUID)
PASSWORD: str = APPLICATION_CONFIG.read_str(SECTION=Sections.CREDENTIALS, KEY=Keys.PASSWORD)
SERVER_ADDRESS: str = APPLICATION_CONFIG.read_str(SECTION=Sections.SERVER, KEY=Keys.IP_ADDRESS)
SERVER_PORT: int = APPLICATION_CONFIG.read_int(SECTION=Sections.SERVER, KEY=Keys.PORT)

"""
   Wydruk podstawowej konfiguracji
"""

print("KONFIGURACJA SIECIOWA")
print(f" UUID: {UUID}")
print(f" SERWER: {SERVER_ADDRESS}:{SERVER_PORT}")

"""
   Inicjalizacja Kolejki
"""
RAMDISK_QUEUE: RamdiskQueue = RamdiskQueue(APPLICATION_CONFIG)

print(f"[RAMDISK_QUEUE] init()")
assert RAMDISK_QUEUE.init(), "ERROR"


"""
   Inicjalizacja połączenia z serwerem
"""

SERVER_LINK: ServerLink = ServerLink(
   IP_ADDRESS=SERVER_ADDRESS,
   PORT=SERVER_PORT,
   DEVICE_UUID=UUID,
   PASSWORD=APPLICATION_CONFIG.read_str(SECTION=Sections.CREDENTIALS, KEY=Keys.PASSWORD),
)
print(f"[SERVER_LINK] init()")
assert SERVER_LINK.init(), "ERROR"


"""
   Inicjalizacja wątków
"""

POLLING_THREAD: PollingThread = PollingThread(
   _QUEUE=RAMDISK_QUEUE
)
FLUSH_THREAD: FlushThread = FlushThread(
   _SERVER_LINK=SERVER_LINK,
   _QUEUE=RAMDISK_QUEUE
)

MASTER_THREAD = MasterThread = MasterThread(
   _SERVER_LINK=SERVER_LINK,
   _QUEUE=RAMDISK_QUEUE,
   _POLLING_THREAD=POLLING_THREAD,
   _FLUSHING_THREAD=FLUSH_THREAD
)
print(f"[MASTER_THREAD] init()")
assert MASTER_THREAD.init(), "ERROR"
MASTER_THREAD.start()

if __name__ == '__main__':
   try:
      while True:
         print(f"[MAIN LOOP] {MASTER_THREAD.status()=} | {POLLING_THREAD.status()=} | {FLUSH_THREAD.status()=}")
         time.sleep(5)
   except KeyboardInterrupt as ki:
      print(f"[PROGRAM END] {ki}")





