##!/usr/bin/env python3
import time
import struct
import sys
from pymodbus.client import ModbusSerialClient

# --- KONFIGURACJA ---
PORT = 'COM11'
BAUD = 9600
PARITY = 'E'
ID_516 = 6


def calculate_crc16(data: bytes) -> bytes:
    crc = 0xFFFF
    for pos in data:
        crc ^= pos
        for i in range(8):
            if (crc & 1) != 0:
                crc >>= 1
                crc ^= 0xA001
            else:
                crc >>= 1
    return struct.pack('<H', crc)


def decode_float(byte_data):
    # Big Endian Float
    return round(struct.unpack('>f', byte_data)[0], 3)


def identify_value(val):
    """Próba zgadnięcia co to za wartość"""
    if 215 < val < 255: return "NAPIĘCIE (V) ?"
    if 49.5 < val < 50.5: return "CZĘSTOTLIWOŚĆ (Hz) ?"
    if 0 < val < 50: return "Prąd (A) lub Moc (kW) ?"
    if val > 1000: return "Energia (kWh) ?"
    return ""


def main():
    print(f"--- GŁĘBOKIE SKANOWANIE ID: {ID_516} (Szukam wartości > 0) ---")

    client = ModbusSerialClient(
        port=PORT,
        baudrate=BAUD,
        parity=PARITY,
        stopbits=1,
        bytesize=8,
        timeout=0.2
    )

    if not client.connect():
        print("Błąd portu.")
        exit(1)

    ser = client.socket
    found_something = False

    # Skanujemy od 0 do 100 (co 2 rejestry)
    for reg in range(0, 100, 2):
        try:
            # Budowa ramki na chama (omijamy walidację Pymodbus)
            # [ID] [03] [RegHi] [RegLo] [00] [02] [CRC]
            pdu = struct.pack('>BBHH', ID_516, 3, reg, 2)
            frame = pdu + calculate_crc16(pdu)

            ser.reset_input_buffer()
            ser.write(frame)

            # Odbieramy 9 bajtów
            response = ser.read(9)

            if len(response) == 9:
                # Odbiór poprawny (nawet jak kod funkcji w odpowiedzi jest 01)
                val = decode_float(response[3:7])

                # WYPISUJEMY TYLKO NIE-ZERA
                if val > 0.001:
                    guess = identify_value(val)
                    print(f"✅ Rejestr HEX: {reg:04X} | DEC: {reg:03d} | Wartość: {val:<10}  <-- {guess}")
                    found_something = True

        except Exception:
            pass

        # Małe opóźnienie
        time.sleep(0.02)

    client.close()

    if not found_something:
        print(
            "\n[WYNIK] Znaleziono same zera. Jeśli licznik jest podłączony do prądu, to jest uszkodzony lub zasilany tylko awaryjnie.")
    else:
        print("\n[WYNIK] Koniec skanowania. Dopasuj teraz adresy z listy powyżej do swojego Enuma.")


if __name__ == "__main__":
    main()