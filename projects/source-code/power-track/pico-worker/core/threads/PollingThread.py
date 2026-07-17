from core.classes.RamdiskQueue import RamdiskQueue
from core.threads.BaseThread import *
from core.classes.energy_meters.OR_WE_504 import *
import time, json
from core.classes.AlertProfiles import *
from django.utils.timezone import datetime
__THREAD_NAME__: str = "POLLING_THREAD"

class PollingThread(BaseThread):
   """
      Klasa odpowiada za "próbkowanie" czyli będzie w określonym czasie odczytywała liczniki
      i zapisywała dane w kolejce
   """
   def __init__(self, _QUEUE: RamdiskQueue):
      super().__init__()
      self._INTERVAL: float = 60
      self._ENERGY_METERS: list[Energy_Meter_Base] = []
      self._ALERT_PROFILES: list[AlertProfile] = []
      self._WAITING: bool = False
      self._QUEUE = _QUEUE

   def __proccess_loop(self):
      """
         Główna pętla wątku, tutaj dzieje się cała logika odczytania liczników oraz dokonania analizy względem
         profili zdarzeń
      :return:
      """
      print(f"[{__THREAD_NAME__}] STARTED")
      next_time = time.time()
      while self._run_thread:
         # -- -- Pauza wątku -- -- #
         if self._is_paused:
            self._WAITING = True
            time.sleep(1)
            continue
         # -- -- -- #
         self._WAITING = False
         print(f"[{__THREAD_NAME__}] id:{self.get_thread_id()} tick")
         # -- -- Obliczenie czasu uśpienia wątku -- -- #
         next_time += self._INTERVAL
         _tmp: list[json] = []
         # -- -- Pętla dla każdego licznika -- -- #
         for meter in self._ENERGY_METERS:
            _channel_name = meter.get_channel_name()
            _channel_id = meter.get_channel_id()
            _address = meter.SLAVE_ADDRESS
            # -- -- Odczytanie stanu licznika -- -- #
            _success, _reading = meter.read_all()
            if _success:
               print(f"[PollingThread] Odczytano licznik z kanału {_channel_name}(id:{_channel_id} adres {_address}) - {meter.ALERT_PROFILE_ID}_ap")
               _data = {"type": "reading", "success": True, "channel_id": _channel_id, "meter_model": meter.MODEL,
                            "meter_id": meter.METER_ID ,"slave_address": _address, "data": _reading, "timestamp": f"{datetime.isoformat(datetime.now())}"}
               _tmp.append(_data)
               # -- -- Odnalezienie profilu alertu oraz dokonanie analizy -- -- #
               # _alert_profile = next(
               #    (ap for ap in self._ALERT_PROFILES if ap.id == meter.ALERT_PROFILE_ID), None
               # )
               _alert_profile = None
               for ap in self._ALERT_PROFILES:
                  if ap.id == meter.ALERT_PROFILE_ID:
                     _alert_profile = ap

               if _alert_profile:
                  _error, _message = AlertReadingAnalyzer.analyze(_alert_profile, _data)
                  if _error:
                     _tmp.append({"type": "alert", "channel_id": _channel_id, "meter_id": meter.METER_ID, "data": _message})
               else:
                  print(f'[PollingThread] nie odnaleziono profilu alertów dla licznika z kanału {_channel_name}(id:{_channel_id} adres {_address})' )
            else:
               print(f"[PollingThread] Wystąpił błąd odczytu licznika o id: {meter.METER_ID} ")
               _data = {"type": "reading", "success": False, "channel_id": _channel_id, "meter_model": meter.MODEL,
                            "meter_id": meter.METER_ID ,"slave_address": _address}
               _tmp.append(_data)
         # -- -- Wpisanie odczytów i alertów do kolejki wysyłania -- -- #
         if len(_tmp) > 0:
           self._QUEUE.append(FILE_NAME=str(time.time() * 1000), CONTENT=json.dumps(_tmp))

         sleep_time = next_time - time.time()
         if sleep_time > 0:
            time.sleep(sleep_time)
         else:
            next_time = time.time()
      # -- -- -- #
      self.IS_RUNNING = False


   def init(self):
      """
         Woła metodę inicjalizującą w klasie bazowej
      :return:
      """
      self._init(self.__proccess_loop)
      return True

   def start(self):
      """
         Woła metodę startującą wątek
      :return:
      """
      self._start()

   def stop(self):
      """
         Woła metodę zatrzymującą pracę wątku
      :return:
      """
      self._stop()

   def is_waiting(self) -> bool:
      """
         Metoda zwraca informacje czy wątek oczekuje w trakcie pauzy
      :return:
      """
      return self._WAITING

   def set_interval(self, SECS: int):
      """
         Metoda zmienia czas interwału pracy wątku
      :param SECS: Sekundy
      :return:
      """
      self._INTERVAL = SECS if SECS < 60 else 60

   def set_energy_meters(self, ENERGY_METERS: list[Energy_Meter_Base]):
      """
         Metoda odpowiada za zaaktualizowanie listy liczników
      :param ENERGY_METERS: lista liczników dziedziczących z Energy_Meter_Base
      :return:
      """
      self._ENERGY_METERS = ENERGY_METERS

   def set_alert_profiles(self, ALERT_PROFILES: list[AlertProfile]):
      """
         Metoda odpowiada za zapisanie listy profili zdarzeń
      :param ALERT_PROFILES:
      :return:
      """
      self._ALERT_PROFILES = ALERT_PROFILES
      print('[PollingThread] Dodano nowe profile:')
      for p in self._ALERT_PROFILES:
         print(f'> {p.name} {p.id}')
