from django.contrib import admin
from .models import *

# Register your models here.
admin.site.register(SwitchBoard)
admin.site.register(Device)
admin.site.register(Channel)
admin.site.register(ElectricMeter)
admin.site.register(RoomGroup)
admin.site.register(Room)
admin.site.register(UserRole)
admin.site.register(Chat)
admin.site.register(ChatMessage)
admin.site.register(MeterReading)
admin.site.register(AlertProfile)
admin.site.register(Alert)
admin.site.register(MeterReport)