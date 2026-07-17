##!/usr/bin/env python3

import sys
import time
import minimalmodbus

"""
    Program umożliwia przeskanowanie interfejsu modbus, celem odkrycia liczników
"""

REG_504_ID = 0x0F
REG_516_ID = 0x0002

print()
print("== == Skaner liczników po interfejsie Modbus == ==")
print()
# -- -- podpowiedź w przypadku braku argumentów -- -- #
if len(sys.argv) == 1:
    print(f"Użycie: ./modbus_scanner.py serial_device addres_range baudrate parity timeout")
    print("np. ./modbus_scanner /dev/ttyUSB0 10-20 9600 E 1")
    exit(1)

address_range = sys.argv[2].strip().split('-')
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

    found_adresses = []

    print(f" > Start skanowania: {address_range[0]}-{address_range[1]} <")

    for i in range(int(address_range[0]), int(address_range[1]) + 1 ):
        try:
            time.sleep(0.5)
            print(f"Skanowanie adresu: {i} -> {address_range[1]}", end="")
            _msg = None

            time.sleep(0.5)

            try:
                r504 = client.read_holding_registers(address=REG_504_ID, count=1, device_id=i)
                time.sleep(0.5)
                if not r504.isError():
                    val = r504.registers[0]
                    print(f" [{val}]", end="")
                    if val == i:
                        found_adresses.append(i)
                        _msg = " - Znaleziono OR_WE_504"
                        print(_msg)
                        continue
                    else:
                        _msg = " - Nie pasuje - 504"
                else:
                    _msg = " - Brak odpowiedzi - 504"
            except Exception as ex:
                _msg = f" - Wyjątek - 504 {ex}"
            print(_msg, end="")

            time.sleep(0.5)

            try:
                r516 = client.read_holding_registers(address=REG_516_ID, count=1, device_id=i)
                time.sleep(0.5)
                if not r516.isError():
                    val = r516.registers[0]
                    print(f" [{val}]", end="")
                    if val == i:
                        found_adresses.append(i)
                        _msg = " - Znaleziono OR_WE_516"
                        print(_msg)
                        continue
                    else:
                        _msg = " - Nie pasuje - 516"
                else:
                    _msg = " - Brak odpowiedzi - 516"
            except Exception as ex:
                _msg = f" - Wyjątek - 516 {ex}"
            print(_msg, end="")

            print()
        except KeyboardInterrupt:
            print(" Zamykanie...")
            exit(0)
        except Exception as ex:
            print(" - błąd.")
            print(ex)

    print(f"Znaleziono adresy: {found_adresses}")


if __name__ == "__main__":
    main()

