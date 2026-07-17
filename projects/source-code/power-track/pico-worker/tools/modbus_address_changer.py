##!/usr/bin/env python3

import sys
import time

from pymodbus.client import ModbusSerialClient

"""
    Program umożliwia zmianę adresu SLAVE w urządzeniu po interfejsie modbus
"""

REG_504_ID = 0x0002
REG_516_ID = 0x0002

print()
print("== == Skaner liczników po interfejsie Modbus == ==")
print()
# -- -- podpowiedź w przypadku braku argumentów -- -- #
if len(sys.argv) == 1:
    print(f"Użycie: ./modbus_address_changer.py serial_device old_address->new_address baudrate parity timeout")
    print("np. ./modbus_address_changer.py /dev/ttyUSB0 1->18 9600 E 1")
    exit(1)

address_range = sys.argv[2].strip().split('->')
old_address = int(address_range[0])
new_address = int(address_range[1])
baud_rate = int(sys.argv[3])
_tmp = sys.argv[4].upper()
# -- --  sprawdzenie poprawności parzystości -- -- #
if not _tmp in ["N", "E", "O"]:
    print("Niepoprawna parzystość [N, E, O].")
    exit(1)
parity = _tmp
timeout = float(sys.argv[5])

def main():
    client = ModbusSerialClient(
        port=sys.argv[1],
        baudrate=int(baud_rate),
        parity=parity,
        stopbits=1,
        bytesize=8,
        timeout=float(timeout),
        retries=3
    )
    print(f" > Start klienta modbus", end="")
    if client.connect():
        print(" OK")
    else:
        print(f" BŁĄD - port {sys.argv[1]}")
        exit(1)

    print(f" > Szukanie urządzenia o adresie: {old_address} <")

    try:
        print(f"Skanowanie adresu: {old_address} -> {new_address}", end="")

        target_reg = None

        _msg = None
        try:
            r504 = client.read_holding_registers(address=REG_504_ID, count=1, device_id=int(old_address))
            if not r504.isError():
                val = r504.registers[0]
                if val == old_address:
                    _msg = f" - Znaleziono OR_WE_504 o adresie {old_address}"
                    target_reg = REG_504_ID
                else:
                    _msg = " - Nie znaleziono - 504"
        except Exception as ex:
            _msg = " - Nie znaleziono {ex}"

        time.sleep(0.5)
        try:
            r516 = client.read_holding_registers(address=REG_516_ID, count=1, device_id=old_address)
            if not r516.isError():
                val = r516.registers[0]
                if val == old_address:
                    _msg = f" - Znaleziono OR_WE_516 o adresie {old_address}"
                    target_reg = REG_516_ID
                else:
                    if not _msg:
                        _msg = f" - Nie znaleziono - 516"
        except Exception as ex2:
            _msg = f" - Nie znaleziono {ex2}"

        if target_reg:
            print(_msg)
            print(f" Próba zmiany adresu na {new_address} ", end="")
            time.sleep(1)
            write_res = client.write_registers(address=target_reg, values=[new_address], device_id=old_address)
            if write_res.isError():
                print(" - Niepowodzenie")
                exit(1)
            print(" - Czekam", end="")
            time.sleep(2)
            r = client.read_holding_registers(address=target_reg, count=1, device_id = int(new_address))
            if r.isError():
                print(" - Niepowodzenie")
                exit(1)
            if r.registers[0] == new_address:
                print(f" - Pomyślnie zmieniono adres na {new_address}")
                exit(0)
            else:
                print(f" - Błąd odczytano {r.registers[0]} na adresie {new_address}")
        else:
            print(_msg)
    except KeyboardInterrupt:
        print(" Zamykanie...")
        exit(0)
    except Exception as ex:
        print(" - Błąd krytyczny")
        print(f"{ex}")



if __name__ == "__main__":
    main()

