from django.db.models.signals import post_save, post_delete
from django.dispatch import receiver
from .models import Device, ElectricMeter, AlertProfile, Channel

"""
    Zbiór metod odpowiada za wysyłanie sygnałów - w bazie danych, celem aktualizacji pola (czasu) w klasie Device w momencie
    gdy nastąpi zmiana w klasach posiadających referencję do tej instancji celem dynamicznego przeładowania konfiguracji na urządzeniu
"""

@receiver(post_save, sender=ElectricMeter)
@receiver(post_delete, sender=ElectricMeter)
def update_device_on_meter_change(sender, instance, **kwargs):
    """
        Metoda realizuje działanie, gdy zmienia się licznik, odszukuje powiązane urządzenie i je zapisuje co odświeża Device->last_edit_time
    :param sender:
    :param instance:
    :param kwargs:
    :return:
    """
    try:
        if instance.channel and instance.channel.device:
            device = instance.channel.device
            device.save(update_fields=['last_edit_time'])
    except Exception as ex:
           print(f"[Signals] Wystąpił błąd podczas aktualizacji czasu urządzenia: {ex}")

@receiver(post_save, sender=Channel)
@receiver(post_delete, sender=Channel)
def update_device_on_channel_change(sender, instance, **kwargs):
    """
        Metoda realizuje mechanizm aktualizacji czasu urządzenia przy zmianie w kanale
    """
    try:
        if instance.device:
            instance.device.save(update_fields=['last_edit_time'])
    except Exception as ex:
        pass

@receiver(post_save, sender=AlertProfile)
def update_device_on_alert_profile_change(sender, instance, **kwargs):
    """
        Metoda realizuje mechanizm: zmiana w profilu alarmowym wpływa na wszystkie liczniki, które z niego korzystają.
        W związku z tym "namierzono" wszystkie urządzenia, które mają te liczniki w swoich kanałach
    :param sender:
    :param instance:
    :param kwargs:
    :return:
    """
    meters = instance.electricmeter_set.all()

    devices_to_update = set()

    for meter in meters:
        if meter.channel and meter.channel.device:
            devices_to_update.add(meter.channel.device)

    for device in devices_to_update:
        device.save(update_fields=['last_edit_time'])