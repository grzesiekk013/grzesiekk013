import os, threading
from core.classes.Datatypes import *
from core.classes.Config import *
from collections import deque

__SIZE_LIMIT__: int = 1024*16

class RamdiskQueue:
   """
      Klasa odpowiadająca za kolejkowanie danych w ramdysku (FIFO)
   """
   def __init__(self, CONFIG: Config):
      # -- -- Konfiguracja -- --
      self.__cfg = CONFIG
      self.__queue: deque = deque()
      self.__queue_type = self.__cfg.read_str(SECTION=Sections.DEVICE, KEY=Keys.QUEUE_TYPE)
      # -- -- Przechowywanie na Ramdysku -- --
      self.__ramdisk_path: str | None = self.__cfg.read_str(SECTION=Sections.DEVICE, KEY=Keys.RAMDISK_PATH)
      # -- -- Przechowywanie w zmiennej -- --
      self.__variable: dict[str, str] = {}
      # -- -- Mutex -- --
      self.__lock: threading.Lock = threading.Lock()

   def init(self) -> bool:
      """
         Metoda odpowiada za sprawdzenie ścieżki, w której znajduje się ram-dysk
         Również jej zadaniem jest sprawdzenie dostępu do pisania i czytania
      :return: Zwraca True jeśli ścieżka istnieje i można w niej pisać/czytać
      """
      if self.__queue_type == "RAMDISK":
         if not os.path.isdir(self.__ramdisk_path):
            print(f"[RamdiskQueue] Ścieżka do ram-dysku {self.__ramdisk_path} nie istnieje")
            return False
         if not os.access(self.__ramdisk_path, os.W_OK):
            print(f"[RamdiskQueue] Brak uprawnień do pisania w {self.__ramdisk_path}")
            return False
         if not os.access(self.__ramdisk_path, os.R_OK):
            print(f"[RamdiskQueue] Brak uprawnień do czytania z {self.__ramdisk_path}")
            return False
         return True
      elif self.__queue_type == "VARIABLE":
         return True
      else:
         print(f"[RamdiskQueue] Nieznany typ kolejki: {self.__queue_type}")
         return False

   def append(self, FILE_NAME: str, CONTENT: str):
      """
         Metoda odpowiada za dodanie nazwy pliku do kolejki oraz zapisanie
         zawartości do pliku
      :param FILE_NAME: Nazwa pliku
      :param CONTENT: Zawartość pliku
      :return:
      """
      with self.__lock:
         if self.__queue_type == "RAMDISK":
            self.__queue.append(FILE_NAME)
            with open(f"{self.__ramdisk_path}/{FILE_NAME}", "w") as f:
               f.write(CONTENT)
         else:
            self.__queue.append(FILE_NAME)
            self.__variable[FILE_NAME] = CONTENT

   def get(self) -> (str | None, str | None):
      """
         Metoda pobiera pierwszy element kolejki
        :return: Zwraca zawartość pliku lub None
     """
      with self.__lock:
         if not self.__queue:
            return (None, None)

         FILE_NAME: str = self.__queue[0]
         if not FILE_NAME:
            return (None, None)
         # -- -- -- --
         if self.__queue_type == "RAMDISK":
            with open(f"{self.__ramdisk_path}/{FILE_NAME}") as f:
               _tmp = f.read()
               return (_tmp, FILE_NAME)
         else:
            _tmp = self.__variable[FILE_NAME]
            return (_tmp, FILE_NAME)

      # return None
   def delete(self, FILE_NAME: str):
      """
         Metoda kasuje zadany element
      :return:
      """
      with self.__lock:
         if not self.__queue or self.__queue[0] != FILE_NAME:
            return
         if self.__queue_type == "RAMDISK":
            self.__queue.remove(FILE_NAME)
            os.remove(f"{self.__ramdisk_path}/{FILE_NAME}")
         else:
            self.__queue.remove(FILE_NAME)
            del self.__variable[FILE_NAME]

   def size(self) -> int:
      """
         Metoda zwraca wielkość kolejki
      :return: Wielkość Kolejki
      """
      return len(self.__queue)


