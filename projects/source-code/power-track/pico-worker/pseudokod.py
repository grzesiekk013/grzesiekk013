import libs
import configparser
import json

config = configparser.ConfigParser().read("config.ini")

_config = configparser.ConfigParser().read("server_config.ini")

ADDR_IP: str = _config["[SERVER]"]["IP_ADDRESS"]
ADDR_PORT: str = _config["[SERVER]"]["PORT"]

class interfejs_modbus:
    def __init__(self, UART: str, BAUD: int, PARITY: int, STOPBITS: int, BYTES: int):
        pass
    def get_id(self):
        pass

    def ping(self):
        pass

class ServerLink:
    def __init__(self, IP_ADDRESS: str, PORT: int):
        pass
    def test_connection(self):
        pass
    def send(self, endpoint: str, data: json):
        pass
    def get(self, endpoint, data: json):
        pass

interfejsy_modbus: list[interfejs_modbus] | None = None

interfejsy = config["[INTERFACES]"].split(",")

i: int = 0
for i in interfejsy:
    __section = f"[{interfejsy[i]}]"
    i+=1
    __uart = config[__section]["UART"]
    __baud = config[__section]["BAUD"]
    __parity = config[__section]["PARITY"]
    __stopbits = config[__section]["STOPBITS"]
    __bytes = config[__section]["BYTES"]
    tmp: interfejs_modbus = interfejsy_modbus(__uart, __baud, __parity, __stopbits, __bytes)
    interfejsy_modbus.append(tmp)

for i in interfejsy_modbus:
    assert i.ping(), f"Brak komunikacji {i.get_id()}"

SERVER: ServerLink = ServerLink(IP_ADDRESS=ADDR_IP, PORT=ADDR_PORT)





