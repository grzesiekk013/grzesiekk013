from dataclasses import dataclass

@dataclass(slots=True)
class AlertProfile:
    """
        Klasa - dataclass - przechowuje konfiguracje alertu zdarzeń
        Dla klasy wykorzystano __slots__ celem zredukowania ilości zużytej pamięci RAM
    """

    id: int = 0
    name: str = ""
    is_active: bool = False
    monitor_voltage: bool = False
    monitor_current: bool = False
    monitor_frequency: bool = False
    monitor_active_power: bool = False
    monitor_reactive_power: bool = False
    monitor_power_factor: bool = False
    l1_enabled: bool = False
    l2_enabled: bool = False
    l3_enabled: bool = False
    min_voltage: int = 0
    max_voltage: int = 0
    min_current: int = 0
    max_current: int = 0
    min_frequency: float = 0
    max_frequency: float = 0
    min_active_power: int = 0
    max_active_power: int = 0
    min_reactive_power: int = 0
    max_reactive_power: int = 0
    min_power_factor: float = 0
    max_power_factor: float = 0

    @classmethod
    def get_id(cls) -> int:
        return cls.id

    @classmethod
    def from_json(cls, data: dict):
        """
            Metoda mapuje skrócone klucze z json do pól klasy
        :param data:
        :return:
        """
        return cls(
            id=data.get('id', -1),
            name = data.get('name', ""),
            is_active = data.get('active', False),
            monitor_voltage = data.get('mon_volt', False),
            monitor_current = data.get('mon_amp', False),
            monitor_frequency = data.get('mon_freq', False),
            monitor_active_power = data.get('mon_ap', False),
            monitor_reactive_power = data.get('mon_rp', False),
            monitor_power_factor = data.get('mon_pf', False),
            l1_enabled = data.get('l1', False),
            l2_enabled = data.get('l2', False),
            l3_enabled = data.get('l3', False),
            min_voltage = data.get('min_volt', 225),
            max_voltage = data.get('max_volt', 235),
            min_current = data.get('min_amp', 0),
            max_current = data.get('max_amp', 80),
            min_frequency = data.get('min_freq', 49.5),
            max_frequency = data.get('max_freq', 50.5),
            min_active_power = data.get('min_ap', -18400),
            max_active_power = data.get('max_ap', 18400),
            min_reactive_power = data.get('min_rp', -18400),
            max_reactive_power = data.get('max_rp', 18400),
            min_power_factor = data.get('min_pf', 0.2),
            max_power_factor = data.get('max_pf', 1.0),
        )

class AlertReadingAnalyzer:
    """
        Klasa odpowiada za analize odczytanych wyników oraz stworzenie treści alertu
    """
    def __init__(self):
        pass

    @staticmethod
    def analyze(ap: AlertProfile, reading: dict) -> (bool, dict):
        """
            Metoda analizuje czy odczytane dane nie są poza założonymi ramami w przypisanym profilu zdarzeń
        :param alert_profile:
        :param reading:
        :return:
        """
        print(reading)
        errors: list[str]= []
        if not ap:
            return False, {"errors": ""}
        if not ap.is_active:
            return False, {"errors": ""}
        if not reading['type']== "reading":
            print("[AlertReadingAnalyzer]Niepoprawny typ danych.")
            return False, {"errors": ""}
        _model = reading['meter_model']
        if not _model in ["OR_WE_504", "OR_WE_516"]:
            return False, {"errors": ""}
        # -- -- -- -- #
        if _model == "OR_WE_504":
            _data = reading["data"]
            if not ap.l1_enabled:
                return False, {"errors": ""}
            # -- -- Sprawdzenie napięcia -- -- #
            if ap.monitor_voltage:
                _tmp = _data["voltage"][0]
                if _tmp < ap.min_voltage:
                    errors.append(f"[V-L1]{_tmp}<min")
                elif _tmp > ap.max_voltage:
                    errors.append(f"[V-L1]{_tmp}>max")
            # -- -- Sprawdzenie prądu -- -- #
            if ap.monitor_current:
                _tmp = _data["current"][0]
                if _tmp < ap.min_current:
                    errors.append(f"[A-L1]{_tmp}<min")
                elif _tmp > ap.max_current:
                    errors.append(f"[A-L1]{_tmp}>max")
            # -- -- Sprawdzenie częstotliwości -- -- #
            if ap.monitor_frequency:
                _tmp = _data["frequency"][0]
                if _tmp < ap.min_frequency:
                    errors.append(f"[F]{_tmp}<min")
                elif _tmp > ap.max_frequency:
                    errors.append(f"[F]{_tmp}>max")
            # -- -- Sprawdzenie Mocy Czynnej -- -- #
            if ap.monitor_active_power:
                _tmp = _data["active_power"][0]
                if _tmp < ap.min_active_power:
                    errors.append(f"[APWR-L1]{_tmp}<min")
                elif _tmp > ap.max_active_power:
                    errors.append(f"[APWR-L1]{_tmp}>max")
            # -- -- Sprawdzenie Mocy Biernej -- -- #
            if ap.monitor_reactive_power:
                _tmp = _data["reactive_power"][0]
                if _tmp < ap.min_reactive_power:
                    errors.append(f"[RPWR-L1]{_tmp}<min")
                elif _tmp > ap.max_reactive_power:
                    errors.append(f"[RPWR-L1]{_tmp}>max")
            # -- -- Sprawdzenie Współczynnika Mocy -- -- #
            if ap.monitor_power_factor:
                _tmp = _data["power_factor"][0]
                if _tmp < ap.min_power_factor:
                    errors.append(f"[PF]{_tmp}<min")
                elif _tmp > ap.max_power_factor:
                    errors.append(f"[PF]{_tmp}>max")
            if len(errors) > 0:
                ret = " ".join(errors)
                return True, {"errors": ret}
            else:
                return False, {"errors": ""}
        elif _model == "OR_WE_516":
            _data = reading["data"]
            _phases_enabled = [ap.l1_enabled, ap.l2_enabled, ap.l3_enabled]

            for i in range(3):
                if not _phases_enabled[i]:
                    continue
                _idx = i + 1

                # -- -- Sprawdzenie napięcia -- -- #
                if ap.monitor_voltage:
                    _tmp = _data["voltage"][i]
                    if _tmp < ap.min_voltage:
                        errors.append(f"[V-L{_idx}]{_tmp}<min")
                    elif _tmp > ap.max_voltage:
                        errors.append(f"[V-L{_idx}]{_tmp}>max")
                # -- -- Sprawdzenie prądu -- -- #
                if ap.monitor_current:
                    _tmp = _data["current"][i]
                    if _tmp < ap.min_current:
                        errors.append(f"[A-L{_idx}]{_tmp}<min")
                    elif _tmp > ap.max_current:
                        errors.append(f"[A-L{_idx}]{_tmp}>max")
                # -- -- Moc czynna -- -- #
                if ap.monitor_active_power:
                    _tmp = _data["active_power"][i+1]
                    if _tmp < ap.min_active_power:
                        errors.append(f"[APWR-L{_idx}]{_tmp}<min")
                    elif _tmp > ap.max_active_power:
                        errors.append(f"[APRR-L{_idx}]{_tmp}>max")
                # -- -- Moc bierna -- -- #
                if ap.monitor_reactive_power:
                    _tmp = _data["reactive_power"][i+1]
                    if _tmp < ap.min_reactive_power:
                        errors.append(f"[RPWR-L{_idx}]{_tmp}<min")
                    elif _tmp > ap.max_reactive_power:
                        errors.append(f"[RPRR-L{_idx}]{_tmp}>max")
            # -- -- Sprawdzenie częstotliwości -- -- #
            if ap.monitor_frequency:
                _tmp = _data["frequency"][0]
                if _tmp < ap.min_frequency:
                    errors.append(f"[F]{_tmp}<min")
                elif _tmp > ap.max_frequency:
                    errors.append(f"[F]{_tmp}>max")
            # -- -- Sprawdzenie wspólczynnika mocy -- -- #
            if ap.monitor_power_factor:
                _tmp = _data["power_factor"][0]
                if _tmp < ap.min_power_factor:
                    errors.append(f"[PF]{_tmp}<min")
                elif _tmp > ap.max_power_factor:
                    errors.append(f"[PF]{_tmp}>max")
            if len(errors) > 0:
                ret = " ".join(errors)
                return True, {"errors": ret}
            else:
                return False, {"errors": ""}

        else:
            print('[AlertReadingAnalyzer] Nieznany typ licznika')
            return False, {"errors": ""}
