
import sys
import time

import minimalmodbus as mm


if len(sys.argv) != 4:
   print("./chngadr /dev/ttyUSBx oldAddress newAddress")
   exit(0)

ttydev = str(sys.argv[1])
adrold = int(sys.argv[2])
adrnew = int(sys.argv[3])

inst = mm.Instrument(ttydev, 1)
inst.serial.baudrate = 9600
inst.serial.parity = "E"
inst.serial.timeout = 0.20
inst.debug = True


def main():
   try:
      inst.address = adrold
      inst.write_register(2, int(adrnew))
      time.sleep(1.0)
      val = inst.read_register(2)
      print(f"new adr: {val}")
   except Exception as e:
      print(e)


# - - - - - - - - - - - - - - - - - - - - - - - - -
if __name__ == "__main__":
    main()