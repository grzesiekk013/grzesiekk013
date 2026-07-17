import json

import requests
from datetime import datetime

class ServerLink:
   def __init__(self, IP_ADDRESS: str, PORT: int, DEVICE_UUID: str, PASSWORD: str):
      self._DEVICE_UUID: str = DEVICE_UUID
      self._IP_ADDRESS: str = IP_ADDRESS
      self._PORT: int = PORT
      self.__PASSWORD: str = PASSWORD
      self._SESSION = requests.Session()
      self._SESSION.auth = (self._DEVICE_UUID, self.__PASSWORD)

   def init(self) -> bool:
      # try:
      #    r = self._SESSION.post(f"http://{self._IP_ADDRESS}:{self._PORT}/api/ping")
      #    if r.status_code == 200:
      #       content = r.json()
      #       success = content.get("success")
      #       print(f"[ServerLink] init(): {content.get('message')}")
      #       return success
      #    else:
      #       content = r.json()
      #       print(f"[ServerLink] init(): {content.get('message')}")
      #       return False
      # except Exception as ex:
      #    print(f"[ServerLink] init() Wyjątek {ex}")
      #    return False
      return True
   def ping(self) -> bool:
      try:
         r = self._SESSION.post(f"http://{self._IP_ADDRESS}:{self._PORT}/api/ping")
         return r.status_code == 200
      except:
         print('[ServerLink] Błąd Pingu')
         return False

   def get_device_last_config_change(self, last_time: datetime | None) -> (bool, bool, datetime or None):
      """
         Metoda odpowiada za pobranie czasu ostatniej zmiany Device w bazie danych i sprawdza czy ten czas się zmienił
      :param last_time: Znany czas do porównania
      :return: Czy się udało?, Czy czas się zmienił?, Nowy czas
      """
      # Logic hidden for security and copyright protection reasons (engineering thesis).
      # Full source code is available to recruiters upon request.
      pass

   def get_device_config(self) -> (bool, json or None):
      try:
         r = self._SESSION.get(f"http://{self._IP_ADDRESS}:{self._PORT}/api/device/config",
                               auth=(self._DEVICE_UUID, self.__PASSWORD), timeout=5)
         if not r.status_code == 200:
            return False, None

         return True, r.json()

      except Exception as ex:
         print(f"[ServerLink] {ex}")


   def send_readings(self, READINGS: json) -> bool:
      """
         Metoda odpowiada za wysłanie odczytów z liczników energii elektrycznej na zdalny serwer akwizycji
      :param READINGS: Odczyty w formacie json, który jest listą obiektów
      :return: Rezultat oznaczający czy wysyłanie się powiodło
      """

      try:
         print('[ServerLink] Wysyłam pomiary...')
         r = self._SESSION.post(f"http://{self._IP_ADDRESS}:{self._PORT}/api/device/send_measurements",
                                json=READINGS)
         if r.status_code == 200:
            print('[ServerLink] Wysłano pomiary.')
            return True
         else:
            print(f"[ServerLink] Nie udało się wysłać pomiarów: {r.status_code} {r.json()}")
            return False
      except Exception as ex:
         print(f"[ServerLink] Nie udało się wysłać pomiarów: {ex}")
         return False
      finally:
         pass

   def send_alert(self, DATA: json) -> bool:
      """
         Metoda odpowiada za wysłanie zdarzenia na serwer
      :return: Rezultat oznaczający czy wysyłanie się powiodło
      """
      r = self._SESSION.post(f"http://{self._IP_ADDRESS}:{self._PORT}/api/device/send_alerts",
                             json=DATA)
      # Logic hidden for security and copyright protection reasons (engineering thesis).
      # Full source code is available to recruiters upon request.
      pass
