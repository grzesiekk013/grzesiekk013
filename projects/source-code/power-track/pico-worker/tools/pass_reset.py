import sys,time,serial

"""
    Program realizuje zdjęcie hasła na ORNO OR WE 504
"""

PORT = sys.argv[1]
BAUD = 9600
PARITY = serial.PARITY_EVEN

def send_raw(serial, hex_str):
    """
        Funcja przesyła surową ramkę
    :param serial:
    :param hex_str:
    :param desc:
    :return:
    """
    p = bytes.fromhex(hex_str.replace(" ", ""))
    print(f"Wysyłam: {p.hex()}")

    serial.write(p)
    time.sleep(0.5)

    if serial.in_waiting:
        response = serial.read(serial.in_waiting)
        print(f"Odebrano: {response.hex()}")
        return True
    else:
        print("Odebrano: brak")
        return False

def main():
    try:
        s = serial.Serial(
            port = PORT,
            baudrate = BAUD,
            parity = PARITY,
            stopbits = serial.STOPBITS_ONE,
            bytesize = serial.EIGHTBITS,
            timeout = 1
        )
    except Exception as ex:
        print(f"Nie udało się otworzyć portu szeregowego: {PORT} | {ex}")
        exit(1)
    finally:
        pass

    print(f"Otwarto port szeregowy: {PORT}")
    print("Odblokowywanie....")
    unlock_hex = "01 28 FE 01 00 02 04 00 00 00 00 FB 12"
    print("Odblokowano pomyślnie" if send_raw(s, unlock_hex) else "Nie udało się odblokować")

    print("Sprawdź poprawność odblokowania przy użyciu modbus_scanner.py")

    s.close()

if __name__ == "__main__":
    main()