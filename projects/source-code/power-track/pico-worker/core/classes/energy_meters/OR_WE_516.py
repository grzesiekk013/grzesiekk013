import ctypes
import sys

from core.classes.energy_meters.EnergyMeterBase import *
from core.classes.networking.ModbusInterfaces import *
from core.classes.Datatypes import *
import time, struct
from datetime import datetime

"""
   Dokumentacja:
      https://files.orno.pl/support/Others/ORNO/ORWE504_5901752481282/OR-WE-504_rejestry.pdf
"""

class OR_WE_516(Energy_Meter_Base):
   def __init__(self, _METER_ID: int, _MODBUS_INTERFACE: ModbusInterfaces, _SLAVE_ADDRES: int, _ALERT_PROFILE_ID: int):
      super().__init__(_MODBUS_INTERFACE, _SLAVE_ADDRES, "OR_WE_516", _METER_ID, _ALERT_PROFILE_ID)

   def _modbus_helper(self, REG_ADDRESS: OR_WE_516_REG, COUNT: int, SAFE_READ: bool=True) -> list[int]:
      i = 0
      while i < 5:
         registers = self.MODBUS_INTERFACE.read_holding_registers(
            SLAVE_ADDRESS=self.SLAVE_ADDRESS,
            ADDRESS=REG_ADDRESS,
            COUNT=COUNT,
            BUGGY_DEVICE=True
         )
         time.sleep(MODBUS_SAFE_READ_SLEEP if SAFE_READ else 0)
         if len(registers) == COUNT:
            return registers
         else:
            i = i + 1

      return registers if registers else []

   def _decode_float(self, registers) -> float:
      """
          Metoda zamienia 2 rejestry 16 bitowe na float 32 bitowy
          :param registers:
          :return:
      """
      if not registers or len(registers) < 2:
         return 0.0
      bin_data = registers[0].to_bytes(2, 'big') + registers[1].to_bytes(2, 'big')
      if sys.byteorder == 'little':
         bin_data = bin_data[::-1]

      value = ctypes.c_float.from_buffer_copy(bin_data).value
      return round(value, 4)

   def read_voltage(self) -> list[float]:
      vl1 = self._decode_float(self._modbus_helper(OR_WE_516_REG.R_L1_VOLTAGE, 2))
      vl2 = self._decode_float(self._modbus_helper(OR_WE_516_REG.R_L2_VOLTAGE, 2))
      vl3 = self._decode_float(self._modbus_helper(OR_WE_516_REG.R_L3_VOLTAGE, 2))
      return [vl1+220 if vl1 else 0.0, vl2+220 if vl2 else 0.0,vl3+220 if vl3 else 0.0]

   def read_current(self) -> list[float]:
      al1 = self._decode_float(self._modbus_helper(OR_WE_516_REG.R_L1_CURRENT, 2))
      al2 = self._decode_float(self._modbus_helper(OR_WE_516_REG.R_L2_CURRENT, 2))
      al3 = self._decode_float(self._modbus_helper(OR_WE_516_REG.R_L3_CURRENT, 2))
      return [al1, al2, al3]

   def read_frequency(self) -> list[float]:
      res = self._modbus_helper(OR_WE_516_REG.R_FREQUENCY, 2)
      return [self._decode_float(res)]

   def read_active_power(self) -> list[float]:
      total = self._decode_float(self._modbus_helper(OR_WE_516_REG.R_TOTAL_ACTIVE_POWER, 2))
      apl1 = self._decode_float(self._modbus_helper(OR_WE_516_REG.R_L1_TOTAL_ACTIVE_POWER, 2))
      apl2 = self._decode_float(self._modbus_helper(OR_WE_516_REG.R_L2_TOTAL_ACTIVE_POWER, 2))
      apl3 = self._decode_float(self._modbus_helper(OR_WE_516_REG.R_L3_TOTAL_ACTIVE_POWER, 2))
      return [total, apl1, apl2, apl3]

   def read_reactive_power(self) -> list[float]:
      total = self._decode_float(self._modbus_helper(OR_WE_516_REG.R_TOTAL_REACTIVE_POWER, 2))
      rpl1 = self._decode_float(self._modbus_helper(OR_WE_516_REG.R_L1_TOTAL_REACTIVE_POWER, 2))
      rpl2 = self._decode_float(self._modbus_helper(OR_WE_516_REG.R_L2_TOTAL_REACTIVE_POWER, 2))
      rpl3 = self._decode_float(self._modbus_helper(OR_WE_516_REG.R_L3_TOTAL_REACTIVE_POWER, 2))
      return [total, rpl1, rpl2, rpl3]

   def read_apparent_power(self) -> list[float]:
      total = self._decode_float(self._modbus_helper(OR_WE_516_REG.R_TOTAL_APPARENT_POWER, 2))
      apl1 = self._decode_float(self._modbus_helper(OR_WE_516_REG.R_L1_TOTAL_APPARENT_POWER, 2))
      apl2 = self._decode_float(self._modbus_helper(OR_WE_516_REG.R_L2_TOTAL_APPARENT_POWER, 2))
      apl3 = self._decode_float(self._modbus_helper(OR_WE_516_REG.R_L3_TOTAL_APPARENT_POWER, 2))
      return [total, apl1, apl2, apl3]

   def read_power_factor(self) -> list[float]:
      return [self._decode_float(self._modbus_helper(OR_WE_516_REG.R_TOTAL_POWER_FACTOR, 2))]

   def read_active_energy(self) -> list[float]:
      return [self._decode_float(self._modbus_helper(OR_WE_516_REG.R_TOTAL_ACTIVE_ENERGY,2))]

   def read_reactive_energy(self) -> list[float]:
      return [self._decode_float(self._modbus_helper(OR_WE_516_REG.R_TOTAL_REACTIVE_ENERGY, 2))]

   def read_all(self) -> (bool, dict[str, list[float]]):
      """
            "voltage": self.read_voltage(),
            "current": self.read_current(),
            "frequency": self.read_frequency(),
            "active_power": self.read_active_power(),
            "reactive_power": self.read_reactive_power(),
            "apparent_power": self.read_apparent_power(),
            "power_factor": self.read_power_factor(),
            "active_energy": self.read_active_energy(),
            "reactive_energy": self.read_reactive_energy()
      :return:
      """
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
            "l2v": voltage[1],
            "l3v": voltage[2],
            "l1a": current[0],
            "l2a": current[1],
            "l3a": current[2],
            "freq": frequency[0],
            "tap": total_active_power[0],
            "l1ap": total_active_power[1],
            "l2ap": total_active_power[2],
            "l3ap": total_active_power[3],
            "trp": total_reactive_power[0],
            "l1rp": total_reactive_power[1],
            "l2rp": total_reactive_power[2],
            "l3rp": total_reactive_power[3],
            "tapp": total_apparent_power[0],
            "pf": power_factor[0]
         }
         self.LAST_READ_TIME = datetime.now()
         return True, _tmp
      except Exception as ex:
         print(f'[OR_WE_516] {self.SLAVE_ADDRESS} Wystąpił błąd podczas odczytu: {ex}.')
         return False, {}

if __name__ == "__main__":

   modbus_interface = ModbusInterfaces(0, 'COM11', 'COM11', 9600, "E", 1, 8)
   modbus_interface.init()

   meter = OR_WE_516(1, modbus_interface, 6, 0)

   print(meter.read_all())