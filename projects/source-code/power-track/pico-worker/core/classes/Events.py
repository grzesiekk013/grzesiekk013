from core.data.Datatypes import *
import json

class Event:
   """
      Klasa odpowiada za przechowywanie zdarzeń przesyłanych na serwer
   """
   def __init__(self, EVENT_NAME: EventName=None | None, CONTENT: json=None | None):
      self.__event_name: EventName = EVENT_NAME
      self.__content: json = CONTENT
      print(f"[EVENT] {EVENT_NAME} {CONTENT}")

   def toJson(self) -> json:
      """
         Metoda odpowiada za zapisane obiektu klasy do JSON
      :return: Klasa zapisana w postaci JSON
      """
      pass

   def toStr(self) -> str:
      """
         Metoda odpowiada za zapisanie obiektu klasy do typu STR
      :return: Klasa zapisana w postaci Str
      """
      pass

   def fromStr(self, DATA: str):
      """
         Metoda odpowiada za wczytanie pól klasy ze STR
      :param DATA: Pola klasy zapisane za pomocą STR
      :return:
      """
      pass

   def fromJson(self, DATA: json):
      """
         Metoda odpowiada za wczytanie pól klasy z JSON
      :return: 
      """
      pass