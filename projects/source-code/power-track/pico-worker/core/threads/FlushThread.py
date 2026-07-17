import json

from core.classes import RamdiskQueue
from core.classes.networking.ServerLink import ServerLink
from core.threads.BaseThread import *
import time

__THREAD_NAME__: str = "FLUSH_THREAD"

class FlushThread(BaseThread):
   """
      Klasa odpowiada za przesyłanie odczytanych danych na serwer
   """
   def __init__(self, _SERVER_LINK: ServerLink, _QUEUE: RamdiskQueue):
      super().__init__()
      self._INTERVAL: float = 30
      self.SERVER_LINK: ServerLink = _SERVER_LINK
      self._QUEUE: RamdiskQueue = _QUEUE
      self._WAITING: bool = False

   def __proccess_loop(self):
      """
         Główna pętla, odpowiada za rozpoznanie danych w kolejce i przesłanie ich na serwer
      :return:
      """
      print(f"[{__THREAD_NAME__}] STARTED")
      next_time = time.time()
      while self._run_thread:
         # -- -- Pauza -- -- #
         if self._is_paused:
            self._WAITING = True
            continue
         self._WAITING = False
         # -- -- -- -- #
         print(f"[{__THREAD_NAME__}] id:{self.get_thread_id()} tick")
         next_time += self._INTERVAL
         # -- -- Kolejka -- --#
         if self._QUEUE.size() > 0:
            _data, _filename = self._QUEUE.get()
            _data = json.loads(_data)
            print(f"[FlushThread] Dane o długości {len(_data)}: {_data}")
            for _item in _data:
               if _item["type"] == "reading":
                  r = self.SERVER_LINK.send_readings(_data)
                  if r:
                     self._QUEUE.delete(_filename)
               elif _item["type"] == "alert":
                  r = self.SERVER_LINK.send_alert(_data)
                  if r:
                     self._QUEUE.delete(_filename)


         sleep_time = next_time - time.time()
         if sleep_time > 0:
            time.sleep(sleep_time)
         else:
            next_time = time.time()

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
      self._INTERVAL = SECS if SECS < 30 else 30