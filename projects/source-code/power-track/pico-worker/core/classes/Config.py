import configparser
import os

class Config:
   """
      Klasa odpowiada za uproszczenie odczytywania konfiguracji z plików .ini
   """
   def __init__(self, CONFS_PATH: str, CONF_NAME: str):
      self.__CONFS_PATH: str = CONFS_PATH
      self.__CONF_NAME: str = CONF_NAME
      # -- -- config -- --
      self.__cp: configparser.ConfigParser = configparser.ConfigParser()
      self.__cp_read = None
      # -- -- -- --

   def init(self) -> bool:
      """
         Metoda odpowiada za ustawienie klasy ConfigParser oraz sprawdza czy pliki istnieją
         :return: Jeżeli konfiguracja się powiedzie, metoda zwróci pozytywny rezultat
      """
      if not os.path.isdir(self.__CONFS_PATH):
         print(f"[CONFIG] Folder ({self.__CONFS_PATH}) nie istnieje.")
         return False
      # -- -- -- --
      if not os.path.isfile(f"{self.__CONFS_PATH}/{self.__CONF_NAME}"):
         print(f"[CONFIG] Plik ({self.__CONF_NAME}) w folderze ({self.__CONFS_PATH}) nie istnieje.")
         return False
      # Logic hidden for security and copyright protection reasons (engineering thesis).
      # Full source code is available to recruiters upon request.
      pass
      # -- -- -- --
      return True

   def read_str(self, SECTION: str, KEY: str) -> str | None:
      """
         Metoda odpowiada za odczytanie ciągu znaków z pliku konfiguracyjnego
         :param SECTION: Sekcja bez znaków []
         :param KEY: Klucz, po którym będzie szukana wartość
         :return: Zwraca ciąg znaków lub None
      """
      if not self.__cp_read:
         return None
      return self.__cp[f"{SECTION}"][KEY]

   def read_int(self, SECTION: str, KEY: str) -> int | None:
      """
         Metoda odpowiada za odczytanie wartości całkowitej-liczbowej z pliku konfiguracyjnego
         :param SECTION: Sekcja bez znaków []
         :param KEY: Klucz, po którym będzie szukana wartość
         :return: Zwraca liczbę lub None
      """
      if not self.__cp_read:
         return None
      tmp = self.__cp[f"{SECTION}"][KEY]
      if tmp.isdigit():
         return int(tmp)
      else:
         return None

   def read_float(self, SECTION: str, KEY: str) -> float | None:
      """
         Metoda odpowiada za odczytanie wartości zmiennoprzecinkowej z pliku konfiguracyjnego
         :param SECTION: Sekcja bez znaków []
         :param KEY: Klucz, po którym będzie szukana wartość
         :return: Zwraca liczbę lub None
      """
      # Logic hidden for security and copyright protection reasons (engineering thesis).
      # Full source code is available to recruiters upon request.
      pass
