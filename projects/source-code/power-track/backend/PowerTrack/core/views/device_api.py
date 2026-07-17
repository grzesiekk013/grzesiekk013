from datetime import datetime
from ipaddress import ip_address
from logging import exception, raiseExceptions
from django.contrib.auth.models import User
from django.contrib import messages
from django.shortcuts import render, redirect, get_object_or_404
from django.contrib.auth import authenticate, login
from django.http import JsonResponse
from django.views.decorators.csrf import csrf_exempt
from django.db.models import Q, F
from django.db import transaction
from unicodedata import numeric
from urllib3 import request
from django.utils import timezone as t

from . import is_method_post, is_method_get
from ..models import *
import json, re

"""
    Plik jest zbiorem metod, zapytań API służących do komunikacji urządzenia z systemem webowym
"""

@csrf_exempt
def api_ping(request):
    if not is_method_post(request):
        return JsonResponse({"success":False, "message": "Niepoprawna metoda"}, status=403)
    return JsonResponse({"success":True, "message": "pong"}, status=200)

@csrf_exempt
def api_logged(request):
    if not is_method_post(request):
        return JsonResponse({"success":False, "message":"Niepoprawna metoda"}, status=403)
    if request.device:
        return JsonResponse({"success": True, "message": "Zalogowany"}, status=200)
    else:
        return JsonResponse({"success": False, "message": "Niepoprawny login lub hasło"}, status=200)

@csrf_exempt
def api_get_device_last_edit(request):
    if not is_method_get(request):
        return JsonResponse({"success": False, "message": "Niepoprawna metoda"}, status=403)
    if request.device:
        return JsonResponse({"success": True, "message": "OK", "system_time": f"{t.now().isoformat()}" ,"last_edit":f"{request.device.last_edit_time.isoformat() if request.device.last_edit_time else None}"}, status=200)
    else:
        return JsonResponse({"success": False, "message": "Brak dostępu"}, status=200)

@csrf_exempt
def api_get_device_full_config(request):
    # Logic hidden for security and copyright protection reasons (engineering thesis).
    # Full source code is available to recruiters upon request.
    pass

@csrf_exempt
def api_send_alert(request):
    # Logic hidden for security and copyright protection reasons (engineering thesis).
    # Full source code is available to recruiters upon request.
    pass
    
@csrf_exempt
def api_send_measurements(request):
    if not is_method_post(request):
        return JsonResponse({"success": False, "message": "Niepoprawna metoda"}, status=403)
    if not request.device:
        return JsonResponse({"success": False, "message": "Brak dostępu"}, status=200)

    try:
        measurements = json.loads(request.body)
        if not measurements:
            return JsonResponse({"success": False, "message": "Brak danych"}, status=200)
        print(measurements)
        with transaction.atomic():
            for measurement in measurements:
                timestamp = measurement.get("timestamp")
                meter_id = measurement.get("meter_id")
                model = measurement.get("meter_model")

                measurement_reading = measurement.get("data")

                total_active_energy = int(measurement_reading.get("tae"))
                total_reactive_energy = int(measurement_reading.get("tre"))

                frequency = measurement_reading.get("freq")

                l1_voltage = measurement_reading.get("l1v")
                l2_voltage = 0
                l3_voltage = 0
                if not model == "OR_WE_504":
                    l2_voltage = measurement_reading.get("l2v")
                    l3_voltage = measurement_reading.get("l3v")

                l1_current = measurement_reading.get("l1a")
                l2_current = 0
                l3_current = 0
                if not model == "OR_WE_504":
                    l2_current = measurement_reading.get("l2a")
                    l3_current = measurement_reading.get("l3a")

                total_active_power = measurement_reading.get("tap")
                l1_ap = measurement_reading.get('l1ap')
                l2_ap = 0
                l3_ap = 0
                if not model == "OR_WE_504":
                    l2_ap = measurement_reading.get('l2ap')
                    l3_ap = measurement_reading.get('l3ap')

                total_reactive_power = measurement_reading.get("trp")
                l1_rp = measurement_reading.get('l1rp')
                l2_rp = 0
                l3_rp = 0
                # Logic hidden for security and copyright protection reasons (engineering thesis).
                # Full source code is available to recruiters upon request.
                pass

                total_apparent_power = measurement_reading.get("tapp")

                power_factor = measurement_reading.get("pf")

                meter = ElectricMeter.objects.filter(id=meter_id).first() if meter_id else None

                MeterReading.objects.create(
                    electric_meter = meter,
                    reading_time = datetime.fromisoformat(timestamp),
                    total_active_energy = total_active_energy,
                    total_reactive_energy = total_reactive_energy,
                    frequency = frequency,
                    l1_voltage = l1_voltage,
                    l2_voltage = l2_voltage,
                    l3_voltage = l3_voltage,
                    l1_current = l1_current,
                    # Logic hidden for security and copyright protection reasons (engineering thesis).
                    # Full source code is available to recruiters upon request.
                    
                    )

                # -- -- Aktualizacja device -- -- #
                _ip_address = request.META['REMOTE_ADDR']
                Device.objects.filter(pk=request.device.pk).update(ip_address = _ip_address if _ip_address else "-.-.-.-", last_seen = t.now())


        return JsonResponse(
            {
                "success": True,
                "message": "Zapisano pomiary",
                "status": 200
            }
        )
    except Exception as ex:
        return JsonResponse({"success": False, "message": f"Błąd zapisu {ex}"}, status=400)