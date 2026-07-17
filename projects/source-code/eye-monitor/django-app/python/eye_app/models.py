from django.db import models
from django.core.validators import MinValueValidator, MaxValueValidator
from django.utils import timezone
from django.conf import settings
from django.db.models.signals import post_save
from django.dispatch import receiver
from datetime import datetime



class SensorDevice(models.Model):
    device_uuid = models.CharField(max_length=64, unique=True)
    ip = models.CharField(max_length=50, unique=False, default="-", blank=True, null=True)
    key = models.CharField(max_length=64)
    last_seen = models.DateTimeField(default=timezone.now)
    description = models.CharField(max_length=255, blank=True, null=True)
    room = models.ForeignKey('Room', on_delete=models.SET_NULL, null=True, blank=True)

    def __str__(self):
        return self.device_uuid


class Room(models.Model):
    device_id = models.ForeignKey(SensorDevice, on_delete=models.CASCADE, related_name='Device_ID', null=True, blank=True)
    room_number = models.CharField(max_length=10, verbose_name="Room number")
    floor = models.IntegerField(verbose_name="Floor")
    capacity = models.IntegerField(verbose_name="Max People Inside")
    description = models.CharField(max_length=100, verbose_name="Description of Room")

    def __str__(self):
        return self.room_number


class ens160(models.Model):
    tvoc = models.FloatField(verbose_name="tvoc")
    eco2 = models.FloatField(verbose_name="eco2")
    aqi = models.FloatField(verbose_name="aqi")


class gy906(models.Model):
    temperature = models.FloatField(verbose_name="temperature")


class sen22396(models.Model):
    co2 = models.FloatField(verbose_name="co2")
    humidity = models.FloatField(verbose_name="humidity")
    temperature = models.FloatField(verbose_name="temperature")


class dht11(models.Model):
    humidity = models.FloatField(verbose_name="humidity")
    temperature = models.FloatField(verbose_name="temperature")


class Measurement(models.Model):
    dev_id = models.ForeignKey(SensorDevice, on_delete=models.CASCADE, related_name="Dev_ID")
    timestamp = models.DateTimeField(default=datetime.now)
    ens160 = models.ForeignKey(ens160, on_delete=models.SET_NULL, null=True)
    gy906 = models.ForeignKey(gy906, on_delete=models.SET_NULL, null=True)
    sen22396 = models.ForeignKey(sen22396, on_delete=models.SET_NULL, null=True)
    dht11 = models.ForeignKey(dht11, on_delete=models.SET_NULL, null=True)

class ParameterRange(models.Model):
    PARAMETER_CHOICES = [
        ('temperature', 'Temperature'),
        ('humidity', 'Humidity'),
    ]

    QUALITY_CHOICES = [
        ('bad', 'Zły'),
        ('medium', 'Średni'),
        ('good', 'Dobry'),
    ]

    room = models.ForeignKey(Room, on_delete=models.CASCADE)
    parameter = models.CharField(max_length=20, choices=PARAMETER_CHOICES)
    quality = models.CharField(max_length=10, choices=QUALITY_CHOICES)
    min_value = models.FloatField(null=True, blank=True)
    max_value = models.FloatField(null=True, blank=True)

    class Meta:
        unique_together = ('room', 'parameter', 'quality')

class ParameterVisibility(models.Model):
    room = models.ForeignKey(Room, on_delete=models.CASCADE)
    parameter = models.CharField(max_length=50)
    visible = models.BooleanField(default=True)

    class Meta:
        unique_together = ('room', 'parameter')

class Access(models.Model):
    room = models.ForeignKey(Room, on_delete=models.CASCADE)
    user = models.ForeignKey(settings.AUTH_USER_MODEL, on_delete=models.CASCADE)
    has_read_access = models.BooleanField(default=False)
    has_read_write_access = models.BooleanField(default=False)

    def __str__(self):
        return f"Access for {self.user} to {self.room}"

class RoomQualities(models.Model):
    # room = models.ForeignKey(Room, on_delete=models.CASCADE)
    room = models.OneToOneField(Room, on_delete=models.CASCADE, related_name='quality')

    #5 range zły średni dobry średni2 zły2
    max_bad_temp_otocz = models.FloatField(default=0.0)
    min_mid_temp_otocz = models.FloatField(default=0.0)
    max_mid_temp_otocz = models.FloatField(default=0.0)
    min_good_temp_otocz = models.FloatField(default=0.0)
    max_good_temp_otocz = models.FloatField(default=0.0)
    min_mid2_temp_otocz = models.FloatField(default=0.0)
    max_mid2_temp_otocz = models.FloatField(default=0.0)
    min_bad2_temp_otocz = models.FloatField(default=0.0)
    # -inf maxbad minmid maxmid mingood maxgood minmid2 maxmid2 minbad2 +inf

    #5 range zły średni dobry średni2 zły2
    max_bad_temp_zbliz = models.FloatField(default=0.0)
    min_mid_temp_zbliz = models.FloatField(default=0.0)
    max_mid_temp_zbliz = models.FloatField(default=0.0)
    min_good_temp_zbliz = models.FloatField(default=0.0)
    max_good_temp_zbliz = models.FloatField(default=0.0)
    min_mid2_temp_zbliz = models.FloatField(default=0.0)
    max_mid2_temp_zbliz = models.FloatField(default=0.0)
    min_bad2_temp_zbliz = models.FloatField(default=0.0)

    #5 range zły średni dobry średni2 zły2
    max_bad_wiglotnosc = models.FloatField(default=0.0)
    min_mid_wiglotnosc = models.FloatField(default=0.0)
    max_mid_wiglotnosc = models.FloatField(default=0.0)
    min_good_wiglotnosc = models.FloatField(default=0.0)
    max_good_wiglotnosc = models.FloatField(default=0.0)
    min_mid2_wiglotnosc = models.FloatField(default=0.0)
    max_mid2_wiglotnosc = models.FloatField(default=0.0)
    min_bad2_wiglotnosc = models.FloatField(default=0.0)

    #3 lower is better
    max_good_co2 = models.FloatField(default=0.0)
    min_mid_co2 = models.FloatField(default=0.0)
    max_mid_co2 = models.FloatField(default=0.0)
    min_bad_co2 = models.FloatField(default=0.0)

    #3 lower is better
    max_good_tvoc = models.FloatField(default=0.0)
    min_mid_tvoc = models.FloatField(default=0.0)
    max_mid_tvoc = models.FloatField(default=0.0)
    min_bad_tvoc = models.FloatField(default=0.0)

    #3 lower is better
    max_good_eco2 = models.FloatField(default=0.0)
    min_mid_eco2 = models.FloatField(default=0.0)
    max_mid_eco2 = models.FloatField(default=0.0)
    min_bad_eco2 = models.FloatField(default=0.0)

    #3 lower is better
    max_good_aqi = models.FloatField(default=0.0)
    min_mid_aqi = models.FloatField(default=0.0)
    max_mid_aqi = models.FloatField(default=0.0)
    min_bad_aqi = models.FloatField(default=0.0)

class RoomActions(models.Model):
    PARAMETER_CHOICES = [
        ("temperature_ambient", "Temperatura (otoczenie)"),
        ("temperature_close", "Temperatura (zbliżeniowa)"),
        ("humidity", "Wilgotność"),
        ("co2", "CO₂"),
        ("tvoc", "TVOC"),
        ("eco2", "eCO₂"),
        ("aqi", "Jakość powietrza (AQI)"),
    ]

    room = models.ForeignKey(Room, on_delete=models.CASCADE)
    parameter = models.CharField(max_length=50, choices=PARAMETER_CHOICES, default="default./")
    custom_name = models.CharField(max_length=100, blank=True)
    min = models.FloatField()
    max = models.FloatField()
    url = models.URLField()
  

    def __str__(self):
        return self.custom_name or self.get_parameter_display()


# - - - signals
DEFAULT_PARAMETERS = [
    {"key": "temperature_ambient", "label": "Temperatura (otoczenie)"},
    {"key": "temperature_close", "label": "Temperatura (zbliżeniowa)"},
    {"key": "humidity", "label": "Wilgotność"},
    {"key": "co2", "label": "CO₂"},
    {"key": "tvoc", "label": "TVOC"},
    {"key": "eco2", "label": "eCO₂"},
    {"key": "aqi", "label": "Jakość powietrza (AQI)"},
]

@receiver(post_save, sender=Room)
def create_default_room_quality(sender, instance, created, **kwargs):
    if created:
        RoomQualities.objects.create(
            room=instance,
            max_good_aqi=1,
            max_good_co2=600,
            max_good_eco2=600,
            max_bad_temp_otocz=15,
            max_bad_temp_zbliz=15,
            max_good_temp_otocz=22,
            max_bad_wiglotnosc=20,
            max_mid_aqi=3,
            max_mid_co2=1000,
            max_mid_eco2=1000,
            max_mid_temp_otocz=18,
            max_mid_temp_zbliz=18,
            max_mid_tvoc=750,
            max_mid_wiglotnosc=39,
            max_good_temp_zbliz=22,
            max_good_tvoc=50,
            max_good_wiglotnosc=60,
            min_good_temp_otocz=19,
            min_good_temp_zbliz=19,
            max_mid2_temp_otocz=25,
            min_good_wiglotnosc=40,
            min_mid_aqi=2,
            min_mid_co2=601,
            min_mid_eco2=601,
            min_mid_temp_otocz=16,
            min_mid_temp_zbliz=16,
            min_mid_tvoc=51,
            min_mid_wiglotnosc=21,
            max_mid2_temp_zbliz=25,
            max_mid2_wiglotnosc=80,
            min_bad2_temp_otocz=26,
            min_bad2_temp_zbliz=26,
            min_bad2_wiglotnosc=81,
            min_bad_aqi=4,
            min_bad_co2=1001,
            min_bad_eco2=1001,
            min_bad_tvoc=751,
            min_mid2_temp_otocz=23,
            min_mid2_temp_zbliz=23,
            min_mid2_wiglotnosc=61
        )
        for param in DEFAULT_PARAMETERS:
            ParameterVisibility.objects.create(
                room=instance,
                parameter=param["key"],
                visible=True
            )
# - - -
