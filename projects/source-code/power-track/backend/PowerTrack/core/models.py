import os.path
from datetime import timezone, datetime
from django.utils import timezone as tz
from tkinter.constants import CASCADE
from django.contrib.auth.models import AbstractBaseUser, BaseUserManager, PermissionsMixin, Group, Permission, User
from django.db import models
from django.db.models import SET_NULL


# Create your models here.

# class Building(models.Model):
#     name = models.CharField(max_length=64, unique=True)
#     country = models.CharField(max_length=32)
#     city = models.CharField(max_length=32)
#     street = models.CharField(max_length=32)
#     number = models.CharField(max_length=16)
#     administrator_contact = models.CharField(max_length=64)
#     administrator_email = models.CharField(max_length=64)
#     administrator_phone = models.CharField(max_length=32)
#     description = models.CharField(max_length=256)



class Chat(models.Model):
    title = models.CharField(max_length=64, null=False, blank=False, default="Zgłoszenie")
    created_by = models.ForeignKey(User, on_delete=models.SET_NULL, null=True, blank=True, related_name="chat_owner")
    created_on = models.DateTimeField(auto_now_add=True)

    assigned_operator = models.ForeignKey(User, on_delete=models.SET_NULL, null=True, blank=True, related_name="chat_operator")
    assigned_admin = models.ForeignKey(User, on_delete=models.SET_NULL, null=True, blank=True, related_name="chat_admin")
    assigned_user = models.ForeignKey(User, on_delete=models.SET_NULL, null=True, blank=True, related_name="chat_user")

    last_msg_date = models.DateTimeField(default=tz.now) # default=tz.now

    status_choices = [
        ("new", "Nowy"),
        ("in_progress", "W trakcie"),
        ("ended", "Zakończono")
    ]

    last_seen_user = models.DateTimeField(null=True, blank=True)
    last_seen_admin = models.DateTimeField(null=True, blank=True)
    last_seen_operator = models.DateTimeField(null=True, blank=True)

    status = models.CharField(max_length=20, choices=status_choices, default="new")

    def __str__(self):
        return f"Zgłoszenie #{self.id}"

    class Meta:
        ordering = ['-last_msg_date']

class ChatMessage(models.Model):
    chat = models.ForeignKey(Chat, on_delete=models.CASCADE)
    created_by = models.ForeignKey(User, on_delete=models.SET_NULL, null=True, related_name="message_owner")

    created_on = models.DateTimeField(auto_now_add=True)
    is_internal = models.BooleanField(default=False)

    message = models.TextField(max_length=1024)

    def __str__(self):
        return f"Wiadomość #{self.id} w #{self.chat}"


class RoomGroup(models.Model):
    name = models.CharField(max_length=64, unique=True)
    description = models.CharField(max_length=256)

    def __str__(self):
        return f"Klient @{self.name}"
    class Meta:
        ordering = ['name']

class UserRole(models.Model):
    role_choices = [
        ('admin', "Administrator"),
        ('operator', "Operator"),
        ('user', "Użytkownik")
    ]

    user = models.OneToOneField(User, on_delete=models.CASCADE, primary_key=True)
    role = models.CharField(max_length=20, default="user", choices=role_choices)
    client = models.ForeignKey(RoomGroup, on_delete=models.SET_NULL, null=True, blank=True)

    def __str__(self):
        return f"{self.user.username} ({self.role})"

class Room(models.Model):
    name = models.CharField(max_length=32)
    number = models.CharField(max_length=16)
    start_lease_date = models.DateField(blank=True, null=True)
    end_lease_date = models.DateField(blank=True, null=True)
    room_group = models.ForeignKey(RoomGroup, on_delete=models.CASCADE, null=True, blank=True)

    def __str__(self):
        return f"{self.name} ({self.number})"

    class Meta:
        ordering = ['name']

# -- -- Rozdzielnia -- --
class SwitchBoard(models.Model):
    name = models.CharField(max_length=64, unique=True)
    # -- -- -- -- -- #
    floor = models.IntegerField(blank=True, null=True)
    location = models.CharField(max_length=32, blank=True, null=True)
    description = models.CharField(max_length=256, blank=True, null=True)
    # -- -- -- -- -- #
    def __str__(self):
        return self.name

    class Meta:
        ordering = ['name']

class DeviceUserManager(BaseUserManager):
    def create_user(self, uuid, password=None):
        if not uuid:
            raise ValueError("Device musi mieć pole uuid")
        device = self.model(uuid=uuid)
        device.set_password(password)
        device.save(using=self._db)
        return device
    def create_superuser(self, uuid, password):
        device = self.create_user(uuid, password)
        device.is_staff = True
        device.is_superuser = True
        device.save(using=self._db)
        return device

class Device(AbstractBaseUser, PermissionsMixin):
    uuid = models.CharField(max_length=32, unique=True)
    password = models.CharField(max_length=64)
    # -- -- -- -- -- #
    is_active = models.BooleanField(default=True)
    is_staff = models.BooleanField(default=False)
    USERNAME_FIELD = "uuid"
    objects = DeviceUserManager()
    # -- -- -- -- -- #
    groups = models.ManyToManyField(
        Group,
        related_name="devices",
        blank=True,
        help_text="",
        verbose_name="device_groups"
    )
    user_permissions = models.ManyToManyField(
        Permission,
        related_name="device_set",
        blank=True,
        help_text="",
        verbose_name="device_permissions"
    )
    # -- -- -- -- -- #
    ip_address = models.CharField(max_length=32, blank=True, null=True)
    type = models.CharField(max_length=32, blank=True, null=True)
    # -- -- -- -- -- #
    status = models.CharField(max_length=32, blank=True, null=True)
    installed_date = models.DateField()
    last_seen = models.DateField(blank=True, null=True)
    description = models.CharField(max_length=128, blank=True, null=True)
    # -- -- -- -- -- #
    switch_board = models.ForeignKey(SwitchBoard, on_delete=models.CASCADE)
    # -- -- -- -- -- #
    polling_interval = models.IntegerField(default=1800)
    flushing_interval = models.IntegerField(default=1800)
    # -- -- -- -- -- #
    last_edit_time = models.DateTimeField(auto_now=True, null=True)

    @classmethod
    def authenticate(cls, uuid, password):
        try:
            device = cls.objects.get(uuid=uuid)
            if device.password == password:
                return device
        except Exception:
            return None

    def __str__(self):
        return f"{self.switch_board}/{self.uuid}"

    class Meta:
        ordering = ['switch_board__name', 'uuid']

class Channel(models.Model):
    channel_name = models.CharField(max_length=8)
    device = models.ForeignKey(Device, on_delete=models.CASCADE)
    uart = models.CharField(max_length=16, default="/dev/null")
    baud_rate = models.IntegerField(default=9600)
    parity = models.CharField(max_length=1, default="N")
    stop_bits = models.IntegerField(default=1)
    data_len = models.IntegerField(default=8)

    def __str__(self):
        return f"{self.device}/{self.channel_name}"

    class Meta:
        ordering = ['device', 'channel_name']

class AlertProfile(models.Model):
    """
        Tabela przechowuje dane o przekroczeniach wartości odczytanych z liczników
    """
    # -- -- -- Limity są ustalane na cały licznik i dotyczą każdej z faz licznika -- -- -- #
    name = models.CharField(max_length=64)
    is_active = models.BooleanField()

    # -- -- Wybór pól do monitorowania -- -- #
    monitor_voltage = models.BooleanField(default=True)
    monitor_current = models.BooleanField(default=True)
    monitor_frequency = models.BooleanField(default=True)
    monitor_active_power = models.BooleanField(default=True)
    monitor_reactive_power = models.BooleanField(default=True)
    monitor_power_factor = models.BooleanField(default=True)

    # -- -- Wybór faz do monitorowania -- -- #
    l1_enable = models.BooleanField(default=True)
    l2_enable = models.BooleanField(default=True)
    l3_enable = models.BooleanField(default=True)

    # -- -- Zakres dopuszczalnego napięcia na każdą z faz -- -- #
    # PN-EN 50160
    min_voltage = models.IntegerField(default=207)
    max_voltage = models.IntegerField(default=253)

    # -- -- Zakres dopuszczalnej częstotliwości na każdą z faz -- -- #
    # PN-EN 50160 (zaokralone)
    min_frequency = models.FloatField(default=49.5)
    max_frequency = models.FloatField(default=50.5)

    # -- -- Zakres dopuszczalnego prądu na każdą z faz -- -- #
    min_current = models.IntegerField(default=0)
    max_current = models.IntegerField(default=80)

    # -- -- Zakres dopuszczalnej mocy czynnej na każdą z faz -- -- #
    min_active_power = models.IntegerField(default=-18400)
    max_active_power = models.IntegerField(default=18400)

    # -- -- Zakres dopuszczalnej mocy biernej na każdą z faz -- -- #
    min_reactive_power = models.IntegerField(default=-18400)
    max_reactive_power = models.IntegerField(default=18400)

    # -- -- Zakres dopuszczalnej jakości prądu na każdą z faz -- -- #
    min_power_factor = models.FloatField(default=0.2)
    max_power_factor = models.FloatField(default=1.0)

    class Meta:
        ordering = ['name']


class ElectricMeter(models.Model):
    room = models.ForeignKey(Room, on_delete=models.CASCADE, null=True, blank=True)
    slave_id = models.IntegerField(null=False)
    # device = models.ForeignKey(Device, on_delete=models.CASCADE, blank=True, null=True)
    model = models.CharField(max_length=32)
    three_phases = models.BooleanField()
    channel = models.ForeignKey(Channel, on_delete=models.CASCADE)
    is_active = models.BooleanField(default=True)
    alert_profile = models.ForeignKey(AlertProfile, on_delete=models.SET_NULL, null=True, blank=True)
    # limit = models.ForeignKey(Limit, on_delete=models.CASCADE, blank=True, null=True)
    # last_measurement =
    def __str__(self):
        return f"{self.channel}/{self.slave_id}"

    class Meta:
        ordering = ['channel__device__switch_board__name', 'channel__device__uuid', 'channel__channel_name', 'slave_id']
    @property
    def last_measurement(self):
        return self.meterreading_set.order_by('-reading_time').first()

    @property
    def alert_unconfirmed_count_admin(self):
        return self.alert_set.filter(confirmed_admin_operator=False).count()

    @property
    def alert_unconfirmed_user(self):
        return self.alert_set.filter(confirmed_user=False).count()

    @property
    def is_timed_out(self):
        tmp = self.last_measurement
        if not tmp:
            return True
        tmp1 = self.channel.device.polling_interval
        tmp2 = self.channel.device.polling_interval
        tmp1 = max(tmp1, tmp2)*2

        delta = tz.now() - tmp.reading_time

        return delta.total_seconds() > tmp1


class Alert(models.Model):
    message = models.CharField(max_length=128, default="Przekroczenie")
    alert_profile = models.ForeignKey(AlertProfile, on_delete=SET_NULL, null=True, blank=True)
    datetime_from = models.DateTimeField(db_index=True) # indeksowanie dla zwiększenia wydajności
    # datetime_to = models.DateTimeField(null=True)
    confirmed_admin_operator = models.BooleanField(default=False)
    confirmed_user = models.BooleanField(default=False)
    energy_meter = models.ForeignKey(ElectricMeter, on_delete=SET_NULL, null=True, blank=True)
    room = models.ForeignKey(Room, on_delete=SET_NULL, null=True, blank=True)

    def __str__(self):
        return f"Zdarzenie #{self.id} w {self.energy_meter} ({self.room})"

    class Meta:
        ordering = ['-datetime_from']

class MeterReading(models.Model):
    electric_meter = models.ForeignKey(ElectricMeter, on_delete=models.CASCADE)
    reading_time = models.DateTimeField()

    # -- -- Parametry rozliczeniowe -- -- #
    total_active_energy = models.DecimalField(max_digits=15, decimal_places=2, default=0)
    total_reactive_energy = models.DecimalField(max_digits=15, decimal_places=2, default=0)

    # -- -- Parametry sieci -- -- #
    frequency = models.FloatField()

    l1_voltage = models.FloatField(null=True, blank=True)
    l2_voltage = models.FloatField(null=True, blank=True)
    l3_voltage = models.FloatField(null=True, blank=True)

    l1_current = models.FloatField(null=True, blank=True)
    l2_current = models.FloatField(null=True, blank=True)
    l3_current = models.FloatField(null=True, blank=True)

    # -- -- Moc chwilowa -- -- #
    total_active_power = models.FloatField(null=True, blank=True)
    l1_active_power = models.FloatField(null=True, blank=True)
    l2_active_power = models.FloatField(null=True, blank=True)
    l3_active_power = models.FloatField(null=True, blank=True)

    # -- -- Mob bierna -- -- #
    total_reactive_power = models.FloatField(null=True, blank=True)
    l1_reactive_power = models.FloatField(null=True, blank=True)
    l2_reactive_power = models.FloatField(null=True, blank=True)
    l3_reactive_power = models.FloatField(null=True, blank=True)

    # -- -- Moc pozorna -- -- #
    total_apparent_power = models.FloatField(null=True, blank=True)

    # -- -- Jakość energii  -- -- #
    total_power_factor = models.FloatField(null=True, blank=True)

    class Meta:
        indexes = [
            models.Index(fields=['electric_meter', 'reading_time'])
        ]
        ordering = ['-reading_time']

class MeterReport(models.Model):
    electric_meter = models.ForeignKey(ElectricMeter, on_delete=models.SET_NULL, null=True)
    generated_by = models.ForeignKey(User, on_delete=SET_NULL, null=True)

    report_file = models.FileField(upload_to='reports/%Y/%m')

    date_from = models.DateField()
    date_to = models.DateField()
    created_at = models.DateTimeField(auto_now_add=True)

    def filename(self):
        return os.path.basename(self.report_file.name)

    def __str__(self):
        return f"Raport {self.electric_meter} ({self.date_from}-{self.date_to})"

    class Meta:
        ordering = ['-created_at']