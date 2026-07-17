from asyncio import timeout

from core.classes.Config import *
from core.classes.Datatypes import *
# from pymodbus.client import ModbusSerialClient as MobusClient
from core.OrnoModbusClient import OrnoModbusClient
from pymodbus.exceptions import ModbusException

# from core.energy_meters.EnergyMeterBase import Energy_Meter_Base


class ModbusInterfaces:
   """
      Klasa odpowiada za ...
   """
   def __init__(self, id: int, name: str, uart: str, baud: int, parity: str, stop_bits: int, data_length: int):
      self.__id: int = id
      self.__channel: str = name
      self.__uart: str = uart
      self.__baud: int = baud
      self.__parity: str = parity
      self.__stop_bits: int = stop_bits
      self.__data_length: int = data_length
      self.__status: bool = False
      self.client: OrnoModbusClient | None = None
      self.__connected: bool = False

   # @property
   # def KNOWN_ADDRESSESS(self) -> list[int]:
   #    return self.__known_adresses

   def init(self) -> bool:
      """
         Metoda inicjalizująca interfejs Modbus przy wykorzystaniu konwertera Modbus->TTL
      :return:
      """
      try:
         self.client = OrnoModbusClient(
            # framer=FramerType.RTU,
            port=self.__uart,
            baudrate=self.__baud,
            parity=self.__parity,
            stopbits=self.__stop_bits,
            bytesize=self.__data_length,
            timeout=1
         )
         self.__connected = self.client.connect()
         return self.__connected
      except ModbusException as e:
         print(f"[ModbusInterfaces] Wystąpił błąd inicjalizacji Modbus: {e}.")
         return False
      finally:
         pass

   def get_channel_name(self) -> str | None:
      if self.__channel:
         return self.__channel
      else:
         return None

   def get_channel_id(self) -> int:
      return self.__id

   def __repr__(self):
      return f"u{self.__uart}:b{self.__baud}:p{self.__parity}:s{self.__stop_bits}:d{self.__data_length}"

   def get_uart(self) -> str:
      return self.__uart

   def close(self):
      if self.client:
         self.client.close()
         self.__connected = False

   def read_holding_registers(self, SLAVE_ADDRESS: int, ADDRESS: int, COUNT: int, BUGGY_DEVICE: bool=False) -> list[int] | None:
      # Logic hidden for security and copyright protection reasons (engineering thesis).
      # Full source code is available to recruiters upon request.
      pass

   def write_registers(self, SLAVE_ADDRESS: int, ADDRESS: int, VALUES: list[int]) -> bool:
      # Logic hidden for security and copyright protection reasons (engineering thesis).
      # Full source code is available to recruiters upon request.
      pass
