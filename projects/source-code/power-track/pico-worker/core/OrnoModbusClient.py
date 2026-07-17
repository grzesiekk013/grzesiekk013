import ctypes
import sys
from pymodbus.client import ModbusSerialClient
from pymodbus.client.mixin import T


class OrnoModbusClient(ModbusSerialClient):
    """
       Klasa dziedziczy po ModbusSerialClient,
       celem stworzenia tej klasy jest umożliwienie odczytywania licznika OR-WE-516, który
       jest niezgodny z protokołem ModBus - zwraca niepoprawny kod funkcji 01 na zapytanie 03
    """
    def __init__(self, *args, **kwargs):
        super().__init__(*args, **kwargs)

    def calculate_crc(self, data: bytes) -> bytes:
        """
            Metoda liczy kod CRC 16 na podstawie parametru we.
            :param data: dane wejściowe
            :return: 2 bajty kodu crc
        """
        crc = 0xFFFF
        for _pnt in data:
            crc ^= _pnt
            for i in range(8):
                if (crc & 1) != 0:
                    crc >>= 1
                    crc ^= 0xA001
                else:
                    crc >>= 1
        return crc.to_bytes(2, byteorder='little')

    def _bytes_to_registers(self, bytes_data):
        """
            Metoda zamienia surowe bajty na listę rejestrów 16-bit
        :param bytes_data:
        :return:
        """
        registers = []
        for i in range(0, len(bytes_data), 2):
            part = bytes_data[i:i+2]
            if len(part) == 2:
                val = int.from_bytes(part, byteorder='big')
                registers.append(val)
        return registers

    def _read_buggy_raw(self, address:int, count: int=1, device_id: int=1):
        try:
            frame_left = (
                device_id.to_bytes(1, 'big') +
                b'\x03' +
                address.to_bytes(2, 'big') +
                count.to_bytes(2, 'big')
            )
            frame = frame_left + self.calculate_crc(frame_left)

            if not self.socket:
                self.connect()
            self.socket.reset_input_buffer()
            self.socket.write(frame)

            expected_length = 3 + (count * 2) + 2
            response = self.socket.read(expected_length)

            if len(response) < expected_length:
                return None

            raw_data = response[3 : 3 + (count*2)]

            registers = self._bytes_to_registers(raw_data)

            # -- -- Oszukana odpowiedź -- -- #
            class Response:
                def __init__(self, registers):
                    self.registers = registers
                    self.isError = lambda: False
            return Response(registers)

        except Exception as ex:
            print(f"[OrnoModbusClient] _read_buggy_raw() - Błąd {ex}")

    def modded_read_holding_registers(self, address: int,count: int = 1,
        device_id: int = 1, buggy_device: bool = False) -> T:
        if buggy_device:
            result = self._read_buggy_raw(address, count, device_id)
            if result:
                return result
        return super().read_holding_registers(address=address, count=count, device_id=device_id)

    def decode_float(self, registers) -> float:
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

    def decode_int32(self, registers, divider: int=1) -> int:
        """
            Metoda zmienia dwa rejestry na int32
            :param registers:
            :param divider:
            :return:
        """
        if not registers or len(registers) < 2:
            return 0
        value = (registers[0] << 16) | registers[1]
        return value / divider