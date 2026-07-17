from core.classes.RamdiskQueue import RamdiskQueue
from core.classes.energy_meters.OR_WE_516 import *
from core.threads.PollingThread import *
from core.threads.FlushThread import *
import time

__THREAD_NAME__: str = "MASTER_THREAD"

class MasterThread(BaseThread):
    """
      Klasa odpowiada za nadzorowanie procesów w aplikacji
    """
    def __init__(self, _SERVER_LINK: ServerLink, _QUEUE: RamdiskQueue, _POLLING_THREAD, _FLUSHING_THREAD):
        super().__init__()
        self.SERVER_LINK = _SERVER_LINK
        self._QUEUE = _QUEUE
        self._POLLING_THREAD = _POLLING_THREAD
        self._FLUSHING_THREAD = _FLUSHING_THREAD
        self._LAST_CFG_CHANGE = None
        self._MODBUS_CHANNELS: list[ModbusInterfaces] = []
        self._MODBUS_METERS: list[Energy_Meter_Base] = []
        self._ALERT_PROFILES: list[AlertProfile] = []


    def __proccess_loop(self):
        """
         Główna pętla wątku, tutaj dzieje się cała logika, za którą ten wątek odpowiada
        :return:
        """
        print(f"[{__THREAD_NAME__}] STARTED")

        """
            Inicjalizacja wątków
        """
        print('[MASTER_THREAD] Inicjalizacja wątku próbkującego.')

        print(f"[POLLING_THREAD] init()")
        assert self._POLLING_THREAD.init(), "ERROR"
        time.sleep(1)
        self._POLLING_THREAD.start()

        print('[MASTER_THREAD] Inicjalizacja wątku wysyłającego.')

        print(f"[FLUSHING_THREAD] init()")
        assert self._FLUSHING_THREAD.init(), "ERROR"
        time.sleep(1)
        self._FLUSHING_THREAD.start()


        while self._run_thread:
            time.sleep(5)
            # -- -- -- Połączenie z serwerem -- -- -- #
            if not self.SERVER_LINK.ping():
                print("[MASTER_THREAD] Brak połączenia z serwerem.")
                time.sleep(5)
                continue
            # -- -- -- Sprawdzenie aktualności konfiguracji -- -- -- #
            _success, config_changed, new_time = self.SERVER_LINK.get_device_last_config_change(self._LAST_CFG_CHANGE)
            if _success:
                if config_changed:
                    print("[MASTER_THREAD] Wykryto zmianę konfiguracji urządzenia.")
                    _success, new_config = self.SERVER_LINK.get_device_config()
                    if _success:
                        _success = self._reload_config(new_config)
                        if _success:
                            self._LAST_CFG_CHANGE = new_time
                        else:
                            print('[MASTER_THREAD] Nie udało się zaaktualizować konfiguracji.')
                    else:
                        print("[MASTER_THREAD] Nie udało się odczytać konfiguracju urządzenia.")
            else:
                print("[MASTER_THREAD] Nie udało się pobrać ostatniej zmiany konfiguracji urządzenia.")
            print('[MASTER_THREAD] tick')

    def _reload_config(self, json_data) -> bool:
        """
            Metoda odpowiada za przeładowanie całej konfiguracji systemu w aplikacji
        :param json: json z konfiguracją urządzenia, kanałów oraz liczników
        :return: Czy przeładowanie konfiguracji się powiodło?
        """
        """
        {'success': True, 'message': 'OK', 'config': {'type': 'LP_MAX', 'polling_interval': 1800, 'flushing_interval': 1800, 
        'channels': [{'id': 1, 'name': 'A', 'uart': '/dev/null', 'baud': 9600, 'parity': 'N', 'stop': 1, 'data': 8, 
        'meters': [{'meter_id': 8, 'slave_id': 3, 'alert_profile_id': 1}]}], 
        'alert_profiles': [{'name': '50AMP', 'id': 1, 'active': False, 'mon_volt': True, 'mon_amp': True, 'mon_freq': True, 'mon_ap': True, 'mon_rp': False, 'mon_pf': True, 'l1': False, 'l2': True, 'l3': True, 'min_volt': 207, 'max_volt': 253, 'min_amp': 0, 'max_amp': 80, 'min_freq': 49.5, 'max_freq': 50.5, 'min_ap': -18400, 'max_ap': 18400, 'min_rp': -18400, 'max_rp': 18400, 'min_pf': 0.2, 'max_pf': 1.0}, {'name': 'dsad', 'id': 14, 'active': True, 'mon_volt': True, 'mon_amp': True, 'mon_freq': True, 'mon_ap': True, 'mon_rp': True, 'mon_pf': True, 'l1': True, 'l2': True, 'l3': True, 'min_volt': 207, 'max_volt': 253, 'min_amp': 0, 'max_amp': 80, 'min_freq': 49.5, 'max_freq': 50.5, 'min_ap': -18400, 'max_ap': 18400, 'min_rp': -18400, 'max_rp': 18400, 'min_pf': 0.2, 'max_pf': 1.0}, {'name': 'dsada', 'id': 13, 'active': True, 'mon_volt': True, 'mon_amp': True, 'mon_freq': True, 'mon_ap': True, 'mon_rp': True, 'mon_pf': True, 'l1': True, 'l2': True, 'l3': True, 'min_volt': 207, 'max_volt': 253, 'min_amp': 0, 'max_amp': 80, 'min_freq': 49.5, 'max_freq': 50.5, 'min_ap': -18400, 'max_ap': 18400, 'min_rp': -18400, 'max_rp': 18400, 'min_pf': 0.2, 'max_pf': 1.0}, {'name': 'dsadas', 'id': 15, 'active': True, 'mon_volt': True, 'mon_amp': True, 'mon_freq': True, 'mon_ap': True, 'mon_rp': True, 'mon_pf': True, 'l1': True, 'l2': True, 'l3': True, 'min_volt': 207, 'max_volt': 253, 'min_amp': 0, 'max_amp': 80, 'min_freq': 49.5, 'max_freq': 50.5, 'min_ap': -18400, 'max_ap': 18400, 'min_rp': -18400, 'max_rp': 18400, 'min_pf': 0.2, 'max_pf': 1.0}]}}

        """
        # -- -- Zapauzowanie pozostałych wątków -- -- #
        # self._FLUSHING_THREAD.stop()
        self._POLLING_THREAD.pause(True)
        self._FLUSHING_THREAD.pause(True)
        print('[MASTER_THREAD] Oczekiwanie na zatrzymanie procesów.')
        while self._POLLING_THREAD.is_waiting() or self._FLUSHING_THREAD.is_waiting():
            time.sleep(1)
        print('[MASTER_THREAD] Procesy oczekują na wznowienie.')

        # -- -- Odczytanie wartości config -- -- #
        json_data = json_data['config']
        if not json_data:
            print('[MASTER_THREAD] Brak klucza config w konfiguracji urządzenia.')
            return False
        # -- -- Aktualizacja interwałów -- -- #
        polling_interval = json_data.get('polling_interval')
        flushing_interval = json_data.get('flushing_interval')
        if not (polling_interval in range(30, 43201) and flushing_interval in range(30, 43201)):
            print('[MASTER_THREAD] Interwał Próbkowania/Wypychania jest poza dozwolonym zakresem.')
            return False
        self._POLLING_THREAD.set_interval(polling_interval)
        self._FLUSHING_THREAD.set_interval(flushing_interval)

        # -- -- Aktualizacja kanałów Modbus -- -- #
        for channel in self._MODBUS_CHANNELS:
            channel.close()
        self._MODBUS_CHANNELS.clear()
        self._MODBUS_METERS.clear()
        self._ALERT_PROFILES.clear()

        channels = json_data.get('channels', [])
        print(f'[MASTER_THREAD] Nowa liczba kanałów: {len(channels)}.')
        print(channels)
        if not channels or len(channels) == 0:
            print('[MASTER_THREAD] Brak kanałów w konfiguracji')
            return False
        for channel in channels:
            channel_id = channel.get('id')
            channel_name = channel.get('name')
            channel_uart = channel.get('uart')
            channel_baud = channel.get('baud')
            channel_parity = channel.get('parity')
            channel_stop = channel.get('stop')
            channel_data = channel.get('data')
            channel_meters = channel.get('meters',[])
            # -- -- Weryfikacja poprawności konfiguracji -- -- #
            if not (channel_id > 0 and len(channel_name) in range(1,9) and len(channel_uart) in range(4,17) and
                    channel_baud in [9600, 19200, 38400, 57600, 115200] and
                    channel_parity in ["N", "E", "O"] and channel_stop in [1, 2] and channel_data in [7, 8]):
                print('[MASTER_THREAD] Niepoprawna konfiguracja kanału.')
                return False

            _channel = ModbusInterfaces(
                id = channel_id,
                name = channel_name,
                baud = channel_baud,
                uart = channel_uart,
                parity = channel_parity,
                stop_bits = channel_stop,
                data_length = channel_data
            )
            _channel.init()
            self._MODBUS_CHANNELS.append(_channel)
            print(f'[MASTER_THREAD] Nowa liczba liczników w kanale {channel_name}: {len(channel_meters)}.')
            for meters in channel_meters:
                meter_id = meters.get('meter_id')
                meter_slave_id = meters.get('slave_id')
                meter_alert_profile = meters.get('alert_profile_id', -1)
                meter_type = meters.get('type')
                # -- -- Weryfikacja konfiguracji -- -- #
                if not (meter_id > 0 and meter_type in ["OR-WE-504", "OR-WE-516"] and meter_slave_id in range(1, 248)):
                    print('[MASTER_THREAD] Niepoprawna konfiguracja licznika.')
                    return False
                _meter = None
                if meter_type == "OR-WE-504":
                    _meter = OR_WE_504(meter_id, _channel, meter_slave_id, meter_alert_profile)
                elif meter_type == "OR-WE-516":
                    _meter = OR_WE_516(meter_id, _channel, meter_slave_id, meter_alert_profile)
                else:
                    print('[MASTER_THREAD] Nieznany typ licznika.')
                    return False
                self._MODBUS_METERS.append(_meter)
        # -- -- Aktualizacja Profili -- -- #
        alert_profiles = json_data.get('alert_profiles', [])
        self._ALERT_PROFILES.clear()
        if not alert_profiles:
            print('[MASTER_THREAD] Nie odnaleziono klucza profili alarmowych w konfiguracji.')
            return False
        for alert_profile in alert_profiles:
            _alert_profile = AlertProfile.from_json(alert_profile)
            self._ALERT_PROFILES.append(_alert_profile)
        print(f'[MASTER_THREAD] Liczba profili zdarzeń wynosi: {len(self._ALERT_PROFILES)}')

        # -- -- Aktualizacja liczników w wątku próbkującym -- -- #
        self._POLLING_THREAD.set_energy_meters(self._MODBUS_METERS)
        self._POLLING_THREAD.set_alert_profiles(self._ALERT_PROFILES)
        # -- -- Odmrożenie wątków -- -- #
        self._POLLING_THREAD.pause(False)
        self._FLUSHING_THREAD.pause(False)

        print(f'[MASTER_THREAD] Konfiguracja urządzenia została zaaktualizowana. Kanały: {len(self._MODBUS_CHANNELS)}, Liczniki: {len(self._MODBUS_METERS)}')
        return True

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