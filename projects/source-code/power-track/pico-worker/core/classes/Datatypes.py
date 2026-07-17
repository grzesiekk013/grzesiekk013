from enum import Enum, IntEnum, StrEnum

class Sections(StrEnum):
   DEVICE = "DEVICE"
   SERVER = "SERVER"
   INTERFACES = "INTERFACES"
   DEFAULT = "DEFAULT"
   CREDENTIALS = "CREDENTIALS"

class Keys(StrEnum):
   UUID = "UUID"
   LIST = "LIST"
   UART = "UART"
   BAUD = "BAUD"
   PARITY = "PARITY"
   STOPBITS = "STOPBITS"
   DATA_LENGTH = "DATA_LENGTH"
   IP_ADDRESS = "IP_ADDRESS"
   PORT = "PORT"
   RAMDISK_PATH = "RAMDISK_PATH"
   POLLING_INTERVAL = "POLLING_INTERVAL"
   FLUSHING_INTERVAL = "FLUSHING_INTERVAL"
   CHANNEL = "CHANNEL"
   PASSWORD = "PASSWORD"
   QUEUE_TYPE = "QUEUE_TYPE"
   # SCANNING_INTERVAL = "SCANNING_INTERVAL"

MODBUS_STARTING_ADDRESS: int = 1
MODBUS_ENDING_ADDRESS: int = 247

class EventName(StrEnum):
   EM_CONN_LOST = "UTRACONO_POŁĄCZENIE_Z_LICZNIKIEM"
   EM_CONN_ESTAB = "NAWIĄZANO_PONOWNE_POŁĄCZENIE_Z_LICZNIKIEM"
   EM_CONN_READ_ERR = "BŁĄD_ODCZYTU_LICZNIKA"
   EM_PARSE_ERR = "BŁĄD_PARSOWANIA_ODCZYTU"
   EM_UNKNOWN_EM = "WYKRYTO_NOWY_LICZNIK"

   MB_CH_ERR = "PUSTY_KANAŁ_LUB_BRAK_POŁĄCZENIA"

   SRV_CONN_SEND_ERR = "BŁĄD_WYSYŁANIA"
   BUFF_IS_FULL = "PRZEPEŁNIONY_BUFOR"
   SRV_CONN_LOST = "UTRACONO_POŁĄCZENIE_Z_SERWEREM"
   SRV_CONN_ERR = "BŁĄD_POŁĄCZENIA_Z_SERWEREM"
   NO_IP = "NIE_UZYSKANO_IP"
   SRV_CONN_OK = "NAWIĄZANO_POŁĄCZENIE_Z_SERWEREM"

   CONF_ERR = "BŁĄD_KONFIGURACJI"
   UNKNOWN_ERR = "NIEZNANY_BŁĄD"

   TOO_HIGH_SHORT_AMP = "PRZEKROCZENIE_MOCY_CHWILOWEJ"
   TOO_HIGH_LONG_AMP = "PRZEKROCZENIE_MOCY_DŁUGOTRWAŁEJ"

"""
   Liczniki
"""

class OR_WE_504_REG(IntEnum):
   """
      Klasa - enumerator - przechowuje adresy rejestrów dla poszczególnych pól
      Adresy rejestrów odczytano ze strony:
      https://files.orno.pl/support/Others/ORNO/ORWE504_5901752481282/OR-WE-504_rejestry.pdf
   """
   # -- -- Funkcje -- -- #
   F_READ_REGISTERS = 0x03
   F_WRITE_REGISTERS = 0x10
   F_DEVICE_SETTINGS = 0x28
   # -- -- Napięcie -- -- #
   R_VOLTAGE = 0x00 # S1, D10
   # -- -- Prąd -- -- #
   R_CURRENT = 0x01 # S1, D10
   # -- -- Częstotliwość -- -- #
   R_FREQUENCY = 0x02 # S1, D10
   # -- -- Energia czynna -- -- #
   R_TOTAL_ACTIVE_ENERGY = 0x07 # S2, D1000
   # -- -- Energia bierna -- -- #
   R_TOTAL_REACTIVE_ENERGY = 0x09 # S2, D1000
   # -- -- Moc czynna -- -- #
   R_TOTAL_ACTIVE_POWER = 0x03 # S1, D0
   # -- -- Moc bierna -- -- #
   R_TOTAL_REACTIVE_POWER = 0x04 # S1, D0
   # -- -- Moc pozorna -- -- #
   R_TOTAL_APPARENT_POWER = 0x05 # S1, D0
   # -- -- Współczynnik mocy -- -- #
   R_TOTAL_POWER_FACTOR = 0x06 # S1, D0
   # -- -- Konfiguracja -- -- #
   R_WRITE_ADDRESS = 0x0F # S1, D0


class OR_WE_516_REG(IntEnum):
   """
      Klasa - enumerator - przechowuje adresy rejestrów do poszczególnych danych
      Adresy rejestrów zaczerpano ze strony: https://library.loxone.com/detail/orno-or-we-516-1312/overview
   """
   # -- -- Funkcje -- --
   F_READ_REGISTERS = 0x03
   F_WRITE_REGISTERS = 0x06
   F_WRITE_MULTIPLE_REGISTERS = 0x10
   # -- -- Napięcie -- -- #
   R_L1_VOLTAGE = 0x000E
   R_L2_VOLTAGE = 0x0010
   R_L3_VOLTAGE = 0x0012
   # -- -- Prąd -- -- #
   R_L1_CURRENT = 0x0016
   R_L2_CURRENT = 0x0018
   R_L3_CURRENT = 0x001A
   # -- -- Częstotliwość -- -- #
   R_FREQUENCY = 0x0014
   # -- -- Energia czynna -- -- #
   R_TOTAL_ACTIVE_ENERGY = 0x0100
   # -- -- Energia bierna -- -- #
   R_TOTAL_REACTIVE_ENERGY = 0x0118
   # -- -- Moc czynna -- -- #
   R_TOTAL_ACTIVE_POWER = 0x001C
   R_L1_TOTAL_ACTIVE_POWER = 0x001E
   R_L2_TOTAL_ACTIVE_POWER = 0x0020
   R_L3_TOTAL_ACTIVE_POWER = 0x0022
   # -- -- Moc bierna -- -- #
   R_TOTAL_REACTIVE_POWER = 0x0024
   R_L1_TOTAL_REACTIVE_POWER = 0x0026
   R_L2_TOTAL_REACTIVE_POWER = 0x0028
   R_L3_TOTAL_REACTIVE_POWER = 0x002A
   # -- -- Moc pozorna -- -- #
   R_TOTAL_APPARENT_POWER = 0x002C
   R_L1_TOTAL_APPARENT_POWER = 0x002E
   R_L2_TOTAL_APPARENT_POWER = 0x0030
   R_L3_TOTAL_APPARENT_POWER = 0x0032
   # -- -- Współczynnik mocy -- -- #
   R_TOTAL_POWER_FACTOR = 0x0034
   # -- -- Komunikacja -- -- #
   R_WRITE_ADDRESS = 0x0002


MODBUS_SAFE_READ_SLEEP: float = 1