import threading

class BaseThread:
   """
      Klasa - wrapper do threading, jest klasą bazową do PollingThread i FlushThread
   """
   def __init__(self):
      self.__thread_id: int = -1
      self.__target = None
      self.__thread: threading.Thread | None = None
      self._run_thread = True
      self._is_paused = False

   def _init(self, f):
      """
      Inicjuje wątek z metodą docelową
      :param f: metoda docelowwa
      :return:
      """
      self.__target = f
      self.__thread = threading.Thread(target=self.__target, daemon=True)

   def _start(self):
      """
      Uruchamia wątek jeżeli ten został zainicjalizowany
      :return:
      """
      self._run_thread = True
      if self.__thread is None:
         raise RuntimeError("Wątek nie został zainicjalizowany")

      self.__thread.start()
      self.__thread_id = self.__thread.ident


   def _join(self, timeout=None):
      """
      Czeka za zakończenie wątku
      :return:
      """
      if self.__thread is not None:
         self.__thread.join(timeout=timeout)

   def _stop(self):
      """
      Metoda odpowiada za zatrzymanie wątku
      :return:
      """
      self._run_thread = False
      self._join()

   def pause(self, pause: bool):
      self._is_paused = pause

   def get_pause(self) -> bool:
      return self._is_paused

   def status(self) -> str:
      """
      Metoda odpowiada za sprawdzenie czy wątek działa
      :return:
      """
      if self.__thread:
         return f'[{ "Pracuje" if self.__thread.is_alive() else "Nie pracuje"}, {"Pauza" if self._is_paused else "Brak pauzy"}]'
      else:
         return f"Brak Wątku"

   def get_thread_id(self) -> int:
      """
      Metoda zwraca indent wątku
      :return:
      """
      return self.__thread_id