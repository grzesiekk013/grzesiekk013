from core.classes.networking.ModbusInterfaces import ModbusInterfaces

"""
OR-WE-504 kWh meter
"""

class Energy_Meter_Base:
   """
      Klasa odpowiada za definicje wirtualnych metod dla wszystkich liczników w systemie
   """
   def __init__(self, _MODBUS_INTERFACE: ModbusInterfaces, _SLAVE_ADDRESS: int, _MODEL: str, _METER_ID: int, _ALERT_PROFILE_ID):
      self.MODEL: str = _MODEL
      self.MODBUS_INTERFACE: ModbusInterfaces = _MODBUS_INTERFACE
      self.SLAVE_ADDRESS: int = _SLAVE_ADDRESS
      self.LAST_READ_TIME = -1
      self.METER_ID: int = _METER_ID
      self.ALERT_PROFILE_ID: int = _ALERT_PROFILE_ID

   def get_channel_name(self):
      return self.MODBUS_INTERFACE.get_channel_name()

   def get_channel_id(self):
      return self.MODBUS_INTERFACE.get_channel_id()

   def read_voltage(self) -> list[float]:
      """
         Definicja metody, której zadaniem jest odczytanie obecnego napięcia
      :return:
      """
      pass

   def read_current(self) -> list[float]:
      """
         Definicja metody, której zadaniem jest odczytanie obecnego natężenia
      :return:
      """
      pass

   def read_frequency(self) -> list[float]:
      """
         Definicja metody, której zadaniem jest odczytanie obecnej częstotliwości
      :return:
      """
      pass

   def read_active_power(self) -> list[float]:
      """
         Definicja metody, której zadaniem jest odczytanie mocy czynnej
      :return:
      """
      pass

   def read_reactive_power(self) -> list[float]:
      """
         Definicja metody, której zadaniem jest odczytanie mocy biernej
      :return:
      """
      pass

   def read_apparent_power(self) -> list[float]:
      """
         Definicja metody, której zadaniem jest odczytanie mocy pozornej
      :return:
      """
      pass

   def read_power_factor(self) -> list[float]:
      """
         Definicja metody, której zadaniem jest odczytanie współczynnika mocy
      :return:
      """
      pass

   def read_active_energy(self) -> list[float]:
      """
         Definicja metody, której zadaniem jest odczytanie wartości energii czynnej
      :return:
      """
      pass

   def read_reactive_energy(self) -> list[float]:
      """
         Definicja metody, której zadaniem jest odczytanie wartości energi biernej
      :return:
      """
      pass

   def read_all(self) -> (bool, dict):
      """
         Metoda, która wykonuje wszystkie metody read
      :return: Zwraca słownik wszystkich odczytów
      """
      pass

   def get_info(self) -> dict:
      """
         Zwraca informacje o urządzeniu
      :return:
      """
      return {
         "MODEL": self.MODEL,
         "METER_ID": self.METER_ID,
         "SLAVE_ADDRESS": self.SLAVE_ADDRESS
      }