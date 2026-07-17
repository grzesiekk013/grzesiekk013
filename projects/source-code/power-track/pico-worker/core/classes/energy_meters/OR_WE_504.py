from datetime import datetime

from core.classes.energy_meters.EnergyMeterBase import *
from core.classes.Datatypes import OR_WE_504_REG
from core.classes.networking.ModbusInterfaces import *
from core.classes.Datatypes import *
import time

"""
   Dokumentacja:
      https://files.orno.pl/support/Others/ORNO/ORWE504_5901752481282/OR-WE-504_rejestry.pdf
"""

class OR_WE_504(Energy_Meter_Base):
   def __init__(self, _METER_ID: int, _MODBUS_INTERFACE: ModbusInterfaces, _SLAVE_ADDRES: int, _ALERT_PROFILE_ID: int):
      super().__init__(_MODBUS_INTERFACE, _SLAVE_ADDRES, "OR_WE_504", _METER_ID, _ALERT_PROFILE_ID)


   def _modbus_helper(self, REG_ADDRESS: OR_WE_504_REG, COUNT: int, SAFE_READ: bool=True) -> list[int]:
      i = 0
      while i < 5:
         registers = self.MODBUS_INTERFACE.read_holding_registers(
            SLAVE_ADDRESS=self.SLAVE_ADDRESS,
            ADDRESS=REG_ADDRESS.value,
            COUNT=COUNT
         )
         time.sleep(MODBUS_SAFE_READ_SLEEP if SAFE_READ else 0)
         if len(registers) == COUNT:
            return  registers
         else:
            i = i + 1

      return registers if registers else []

   def read_voltage(self) -> list[float]:
      res = self._modbus_helper(OR_WE_504_REG.R_VOLTAGE, 1)
      return [float(res[0] / 10.0)] if res else [0.0]

   def read_current(self) -> list[float]:
      res = self._modbus_helper(OR_WE_504_REG.R_CURRENT, 1)
      return [float(res[0]/10.0)] if res else [0.0]

   def read_frequency(self) -> list[float]:
      res = self._modbus_helper(OR_WE_504_REG.R_FREQUENCY, 1)
      return [float(res[0]/10.0)] if res else [0.0]

   def read_active_power(self) -> list[float]:
      res = self._modbus_helper(OR_WE_504_REG.R_TOTAL_ACTIVE_POWER, 1)
      return [float(res[0])] if res else [0.0]

   def read_reactive_power(self) -> list[float]:
      res = self._modbus_helper(OR_WE_504_REG.R_TOTAL_REACTIVE_POWER, 1)
      return [float(res[0])] if res else [0.0]

   def read_apparent_power(self) -> list[float]:
      res = self._modbus_helper(OR_WE_504_REG.R_TOTAL_APPARENT_POWER, 1)
      return [float(res[0])] if res else [0.0]

   def read_power_factor(self) -> list[float]:
      res = self._modbus_helper(OR_WE_504_REG.R_TOTAL_POWER_FACTOR, 1)
      return [float(res[0])/1000.0] if res else [0.0]

   def read_active_energy(self) -> list[float]:
      res = self._modbus_helper(OR_WE_504_REG.R_TOTAL_ACTIVE_ENERGY, 2)
      if len(res) == 2:
         _tmp = (res[0] << 16) | res[1]
         return [float(_tmp / 1000.0)]
      return [0.0]

   def read_reactive_energy(self) -> list[float]:
      res = self._modbus_helper(OR_WE_504_REG.R_TOTAL_REACTIVE_ENERGY, 2)
      if len(res) == 2:
         _tmp = (res[0] << 16) | res[1]
         return [float(_tmp / 1000.0)]
      return [0.0]

   def read_all(self) -> (bool, dict[str,list[float]]):

      try:
         total_active_energy = self.read_active_energy()
         total_reactive_energy = self.read_reactive_energy()
         voltage = self.read_voltage()
         current = self.read_current()
         frequency = self.read_frequency()
         total_active_power = self.read_active_power()
         total_reactive_power = self.read_reactive_power()
         total_apparent_power = self.read_apparent_power()
         power_factor = self.read_power_factor()
         _tmp: dict[str, list[float]] = {
            "tae": total_active_energy[0],
            "tre": total_reactive_energy[0],
            "l1v": voltage[0],
            "l1a": current[0],
            "freq": frequency[0],
            "tap": total_active_power[0],
            "l1ap": total_active_power[0],
            "trp": total_reactive_power[0],
            "l1rp": total_reactive_power[0],
            "tapp": total_apparent_power[0],
            "pf": power_factor[0]
         }
         self.LAST_READ_TIME = datetime.now()
         return True, _tmp
      except Exception as ex:
         print(f'[OR_WE_504] {self.SLAVE_ADDRESS} Wystąpił błąd podczas odczytu: {ex}.')
         return False, {}

if __name__ == "__main__":

   modbus_interface = ModbusInterfaces(0, 'COM11', 'COM11', 9600, "E", 1, 8)
   modbus_interface.init()

   meter = OR_WE_504(1, modbus_interface, 2, 0)

   print(meter.read_all())