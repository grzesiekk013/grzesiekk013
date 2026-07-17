from Crypto.Util.Padding import unpad
from django.contrib.auth.models import User
from django.views.decorators.csrf import csrf_exempt
from django.contrib.auth.decorators import login_required, user_passes_test
from django.contrib.auth.forms import UserCreationForm, PasswordChangeForm
from django.contrib.auth import logout, update_session_auth_hash
from django.views import View
from django.shortcuts import render, get_object_or_404, redirect
from datetime import datetime, time as dtime, timedelta, timezone
from django.utils import timezone
import time as pytime
import threading
from time import strftime, localtime
import eye_app.models
from .models import Room, SensorDevice, Measurement, ens160, gy906, sen22396, ParameterVisibility, Access, RoomQualities, RoomActions
from django.http import HttpResponse
from .forms import AccessForm, AddAccessForm, RoomForm, RoomActionForm, RoomQualitiesForm, DeviceAssignForm, CreateDeviceForm
from django.http import JsonResponse
from Crypto.Cipher import AES
import json
import pytz
from django.db.models import Min, Max
from django.utils.dateparse import parse_datetime
import requests
from django.views.decorators.cache import never_cache
from django.db import transaction
# - - permissions

# hex_key = "abcdabcdabcdabcdabcdabcdabcdabcdabcdabcdabcdabcdabcdabcdabcdabcd"

def check_view_permission(request, room_id):
    room = get_object_or_404(Room, pk=room_id)
    try:
        user_access = Access.objects.get(room=room, user=request.user)
    except Access.DoesNotExist:
        user_access = None
    access = Access.objects.filter(room_id=room, user=request.user).first()

    if (access and request.user.is_staff) or (access and access.has_read_access):
        return True
    else:
        source_url = request.build_absolute_uri()
        return render(request, 'eye_app/no_permission.html', {'source': source_url, 'room':room, 'user_access': user_access})

@login_required
def check_view_edit_permission(request, room_id):
    room = get_object_or_404(Room, pk=room_id)
    try:
        user_access = Access.objects.get(room=room, user=request.user)
    except Access.DoesNotExist:
        user_access = None

    access = Access.objects.filter(room_id=room, user=request.user).first()
    if (access and access.has_read_write_access) or request.user.is_staff:
        return True
    else:
        source_url = request.build_absolute_uri()
        return render(request, 'eye_app/no_permission.html', {'source': source_url, 'room':room, 'user_access': user_access})

# - - -

@login_required
def main(request):
    rooms = Room.objects.all()
    _rooms = []

    if request.user.is_staff:
        form = RoomForm()
    else:
        form = None

    for room in rooms:
        if not request.user.is_staff:
            try:
                user_access = Access.objects.get(room=room, user=request.user)
                if not user_access.has_read_access:
                    continue
            except Access.DoesNotExist:
                continue

        measurement = (
            Measurement.objects.filter(dev_id=room.device_id)
            .order_by('-timestamp')
            .select_related('ens160', 'gy906', 'sen22396')
            .first()
        )

        values = {}
        if measurement:
            if measurement.sen22396:
                values['temperature_ambient'] = measurement.sen22396.temperature
                values['humidity'] = measurement.sen22396.humidity
                values['co2'] = measurement.sen22396.co2
            if measurement.gy906:
                values['temperature_close'] = measurement.gy906.temperature
            if measurement.ens160:
                values['tvoc'] = measurement.ens160.tvoc
                values['eco2'] = measurement.ens160.eco2
                values['aqi'] = measurement.ens160.aqi

        try:
            rq = RoomQualities.objects.get(room=room)
        except RoomQualities.DoesNotExist:
            rq = None

        qualities = {}
        if rq:
            def quality_or_none(value, thresholds, fn):
                return fn(value, thresholds) if value is not None else None

            qualities["temperature_ambient"] = quality_or_none(values.get("temperature_ambient"), {
                "max_bad": rq.max_bad_temp_otocz,
                "max_mid": rq.max_mid_temp_otocz,
                "max_good": rq.max_good_temp_otocz,
                "max_mid2": rq.max_mid2_temp_otocz,
                "min_bad2": rq.min_bad2_temp_otocz,
            }, get_long_quality)

            qualities["temperature_close"] = quality_or_none(values.get("temperature_close"), {
                "max_bad": rq.max_bad_temp_zbliz,
                "max_mid": rq.max_mid_temp_zbliz,
                "max_good": rq.max_good_temp_zbliz,
                "max_mid2": rq.max_mid2_temp_zbliz,
                "min_bad2": rq.min_bad2_temp_zbliz,
            }, get_long_quality)

            qualities["humidity"] = quality_or_none(values.get("humidity"), {
                "max_bad": rq.max_bad_wiglotnosc,
                "max_mid": rq.max_mid_wiglotnosc,
                "max_good": rq.max_good_wiglotnosc,
                "max_mid2": rq.max_mid2_wiglotnosc,
                "min_bad2": rq.min_bad2_wiglotnosc,
            }, get_long_quality)

            qualities["co2"] = quality_or_none(values.get("co2"), {
                "max_good": rq.max_good_co2,
                "min_mid": rq.min_mid_co2,
                "max_mid": rq.max_mid_co2,
                "min_bad": rq.min_bad_co2,
            }, get_short_quality)

            qualities["tvoc"] = quality_or_none(values.get("tvoc"), {
                "max_good": rq.max_good_tvoc,
                "min_mid": rq.min_mid_tvoc,
                "max_mid": rq.max_mid_tvoc,
                "min_bad": rq.min_bad_tvoc,
            }, get_short_quality)

            qualities["eco2"] = quality_or_none(values.get("eco2"), {
                "max_good": rq.max_good_eco2,
                "min_mid": rq.min_mid_eco2,
                "max_mid": rq.max_mid_eco2,
                "min_bad": rq.min_bad_eco2,
            }, get_short_quality)

            qualities["aqi"] = quality_or_none(values.get("aqi"), {
                "max_good": rq.max_good_aqi,
                "min_mid": rq.min_mid_aqi,
                "max_mid": rq.max_mid_aqi,
                "min_bad": rq.min_bad_aqi,
            }, get_short_quality)

        quality_display = []
        score = 0

        for key, label, unit in [
            ("temperature_ambient", "Temp. otoczenia", "°C"),
            ("temperature_close", "Temp. zbliżeniowa", "°C"),
            ("humidity", "Wilgotność", "%"),
            ("co2", "CO₂", "ppm"),
            ("tvoc", "TVOC", "ppb"),
            ("eco2", "eCO₂", "ppm"),
            ("aqi", "AQI", "")
        ]:
            q = qualities.get(key)
            if q:
                quality_display.append({
                    "label": label,
                    "text": q[0],
                    "color": q[1],
                    "icon": q[2]
                })
                match q[0]:
                    case "Dobry":
                        score += 3
                    case "Średni":
                        score += 2
                    case "Zły":
                        score += 1

        if quality_display:
            if score >= 12:
                room.overall_quality = ("Dobry", "text-success", "bi-emoji-smile")
            elif score >= 8:
                room.overall_quality = ("Średni", "text-warning", "bi-emoji-neutral")
            else:
                room.overall_quality = ("Zły", "text-danger", "bi-emoji-frown")
        else:
            room.overall_quality = ("Brak danych", "text-muted", "")

        room.quality_display = quality_display
        room.last_seen = room.device_id.last_seen if room.device_id else None  # <- dodane
        _rooms.append(room)

    if request.user.is_staff and request.method == 'POST':
        form = RoomForm(request.POST)
        if form.is_valid():
            form.save()
            return redirect('/')

    return render(request, 'eye_app/main.html', {
        'rooms': _rooms,
        'form': form,
    })

@login_required
def create_room(request):
    pass


@login_required
def account_profile(request):
    if request.method == 'POST':
        form = PasswordChangeForm(request.user, request.POST)
        if form.is_valid():
            user = form.save()
            update_session_auth_hash(request, user)  # ← by nie wylogowało użytkownika
            return redirect('account_profile')
    else:
        form = PasswordChangeForm(request.user)

    return render(request, 'eye_app/account_profile.html', {
        'user': request.user,
        'form': form,
    })

@login_required
def room_actions(request, room_id):
    room = get_object_or_404(Room, pk=room_id)

    permission_result = check_view_edit_permission(request, room_id)
    if permission_result is not True:
        return permission_result

    actions = RoomActions.objects.filter(room=room)
    form = RoomActionForm()

    if request.method == "POST":
        form = RoomActionForm(request.POST)
        if form.is_valid():
            new_action = form.save(commit=False)
            new_action.room = room
            new_action.save()
            return redirect('room_actions', room_id=room.id)

    try:
        user_access = Access.objects.get(room=room, user=request.user)
    except Access.DoesNotExist:
        user_access = None

    return render(request, 'eye_app/room-actions.html', {
        'room': room,
        'actions': actions,
        'form': form,
        'user_access': user_access,
        'last_seen': room.device_id.last_seen if room.device_id else None  # ⬅️ dodane
    })

@login_required
def delete_action(request, room_id, action_id):
    room = get_object_or_404(Room, pk=room_id)
    permission_result = check_view_edit_permission(request, room_id)
    if permission_result is not True:
        return permission_result

    action = get_object_or_404(RoomActions, id=action_id, room=room)
    action.delete()
    return redirect('room_actions', room_id=room.id)

@login_required
def edit_action(request, room_id, action_id):
    room = get_object_or_404(Room, pk=room_id)
    action = get_object_or_404(RoomActions, id=action_id, room=room)

    permission_result = check_view_edit_permission(request, room_id)
    if permission_result is not True:
        return permission_result

    if request.method == "POST":
        form = RoomActionForm(request.POST, instance=action)
        if form.is_valid():
            form.save()
            return redirect('room_actions', room_id=room.id)
    else:
        form = RoomActionForm(instance=action)

    return render(request, 'eye_app/edit-action.html', {
        'room': room,
        'form': form,
        'action': action
    })

def parse_date_range(start_str, end_str):
    try:
        start_date = datetime.strptime(start_str, "%Y-%m-%d")
        end_date = datetime.strptime(end_str, "%Y-%m-%d")
        start = datetime.combine(start_date, dtime.min)  # 00:00:00
        end = datetime.combine(end_date, dtime.max)      # 23:59:59.999999
        return start, end
    except:
        return None, None

@login_required
def room_details(request, room_id):
    room = get_object_or_404(Room, pk=room_id)

    permission_result = check_view_edit_permission(request, room_id)
    if permission_result is not True:
        return permission_result

    try:
        user_access = Access.objects.get(room=room, user=request.user)
    except Access.DoesNotExist:
        user_access = None

    if request.method == 'POST':
        form = RoomForm(request.POST, instance=room)
        if form.is_valid():
            form.save()
            return redirect('room_details', room_id=room.id)
    else:
        form = RoomForm(instance=room)

    return render(request, 'eye_app/room-details.html', {
        'room': room,
        'form': form,
        'user_access': user_access,
        'last_seen': room.device_id.last_seen if room.device_id else None  # ⬅️ dodane
    })

@login_required
def room_device(request, room_id):
    room = get_object_or_404(Room, pk=room_id)
    permission_result = check_view_edit_permission(request, room_id)
    if permission_result is not True:
        return permission_result

    try:
        current_device = room.device_id
    except SensorDevice.DoesNotExist:
        current_device = None

    if request.method == 'POST':
        form = DeviceAssignForm(request.POST)
        if form.is_valid():
            device = form.cleaned_data['device']
            room.device_id = device
            room.save()
            return redirect('room_device', room_id=room.id)
    else:
        form = DeviceAssignForm(initial={'device': room.device_id})

    try:
        user_access = Access.objects.get(room=room, user=request.user)
    except Access.DoesNotExist:
        user_access = None

    return render(request, 'eye_app/room-device.html', {
        'room': room,
        'form': form,
        'device': current_device,
        'user_access': user_access,
        'last_seen': current_device.last_seen if current_device else None,  # ⬅️ dodane
    })

@login_required
def room_edit(request, room_id):
    room = get_object_or_404(Room, pk=room_id)
    try:
        user_access = Access.objects.get(room=room, user=request.user)
    except Access.DoesNotExist:
        user_access = None
    return render(request, 'eye_app/room-edit.html', {'room': room, 'user_access': user_access})  # unused

@login_required
def room_delete(request, room_id):
    room = get_object_or_404(Room, pk=room_id)

    permission_result = check_view_edit_permission(request, room_id)
    if permission_result is not True:
        return permission_result

    room.delete()
    return redirect('main')

@login_required
def room_delete_access(request, room_id, access_id):
    room = get_object_or_404(Room, pk=room_id)
    try:
        user_access = Access.objects.get(room=room, user=request.user)
    except Access.DoesNotExist:
        user_access = None
    access = get_object_or_404(Access, id=access_id, room=room)
    permission_result = check_view_edit_permission(request, room_id)
    if permission_result is not True:
        return redirect('room_manage_access', room_id=room.id)
    access.delete()

    return redirect('room_manage_access', room_id=room.id)

@login_required
def room_manage_access(request, room_id):
    permission_result = check_view_edit_permission(request, room_id)

    if permission_result is not True:
        return permission_result

    room = get_object_or_404(Room, pk=room_id)
    access_list = Access.objects.filter(room=room)

    if request.method == 'POST':
        if 'add_access' in request.POST:
            add_form = AddAccessForm(request.POST, room=room)
            if add_form.is_valid():
                new_access = add_form.save(commit=False)
                new_access.room = room
                new_access.save()
                return redirect('room_manage_access', room_id=room.id)
        else:
            access_id = request.POST.get('access_id')
            if access_id:
                access_instance = get_object_or_404(Access, id=access_id, room=room)
                form = AccessForm(request.POST, instance=access_instance)
                if form.is_valid():
                    form.save()
                    return redirect('room_manage_access', room_id=room.id)

    access_forms = []
    for access in access_list:
        form = AccessForm(instance=access)
        access_forms.append({
            'form': form,
            'instance': access
        })

    add_form = AddAccessForm(room=room)

    try:
        user_access = Access.objects.get(room=room, user=request.user)
    except Access.DoesNotExist:
        user_access = None

    context = {
        'room': room,
        'access_forms': access_forms,
        'add_form': add_form,
        'user_access': user_access,
        'last_seen': room.device_id.last_seen if room.device_id else None,  # ⬅️ dodane
    }

    return render(request, 'eye_app/room-manage-access.html', context)


@login_required
def room_names(request, room_id):
    room = get_object_or_404(Room, pk=room_id)
    permission_result = check_view_edit_permission(request, room_id)
    if permission_result is not True:
        return permission_result

    rq, _ = RoomQualities.objects.get_or_create(room=room)

    if request.method == 'POST':
        form = RoomQualitiesForm(request.POST, instance=rq)
        if form.is_valid():
            form.save()
            return redirect('room_names', room_id=room.id)
    else:
        form = RoomQualitiesForm(instance=rq)

    user_access = Access.objects.filter(room=room, user=request.user).first()

    return render(request, 'eye_app/room-names.html', {
        'room': room,
        'form': form,
        'user_access': user_access,
        'last_seen': room.device_id.last_seen if room.device_id else None  # ⬅️ dodane
    })


def get_long_quality(value, thresholds):
    if value is None:
        return "Brak danych", "text-secondary", "bi-question-circle"
    if value <= thresholds["max_bad"]:
        return "Zły", "text-danger", "bi-emoji-frown"
    elif value <= thresholds["max_mid"]:
        return "Średni", "text-warning", "bi-emoji-neutral"
    elif value <= thresholds["max_good"]:
        return "Dobry", "text-success", "bi-emoji-smile"
    elif value <= thresholds["max_mid2"]:
        return "Średni", "text-warning", "bi-emoji-neutral"
    else:
        return "Zły", "text-danger", "bi-emoji-frown"


def get_short_quality(value, thresholds):
    if value is None:
        return "Brak danych", "text-secondary", "bi-question-circle"
    if value <= thresholds["max_good"]:
        return "Dobry", "text-success", "bi-emoji-smile"
    elif value <= thresholds["max_mid"]:
        return "Średni", "text-warning", "bi-emoji-neutral"
    else:
        return "Zły", "text-danger", "bi-emoji-frown"

@login_required
def room_overall(request, room_id):
    room = get_object_or_404(Room, pk=room_id)

    try:
        user_access = Access.objects.get(room=room, user=request.user)
    except Access.DoesNotExist:
        user_access = None

    access = Access.objects.filter(room=room, user=request.user).first()
    if not (((access or request.user.is_staff) or (access and access.has_read_access))):
        source_url = request.build_absolute_uri()
        return render(request, 'eye_app/no_permission.html', {
            'source': source_url,
            'room': room,
            'user_access': user_access
        })

    visibilities = {
        v.parameter: v.visible for v in ParameterVisibility.objects.filter(room=room)
    }

    measurement = (
        Measurement.objects.filter(dev_id=room.device_id)
        .order_by('-timestamp')
        .select_related('ens160', 'gy906', 'sen22396')
        .first()
    )

    values = {}
    if measurement:
        if measurement.sen22396:
            values['temperature_ambient'] = measurement.sen22396.temperature
            values['humidity'] = measurement.sen22396.humidity
            values['co2'] = measurement.sen22396.co2
        if measurement.gy906:
            values['temperature_close'] = measurement.gy906.temperature
        if measurement.ens160:
            values['tvoc'] = measurement.ens160.tvoc
            values['eco2'] = measurement.ens160.eco2
            values['aqi'] = measurement.ens160.aqi

    rq, _ = RoomQualities.objects.get_or_create(room=room)

    qualities = {
        "temperature_ambient": get_long_quality(values.get("temperature_ambient"), {
            "max_bad": rq.max_bad_temp_otocz,
            "max_mid": rq.max_mid_temp_otocz,
            "max_good": rq.max_good_temp_otocz,
            "max_mid2": rq.max_mid2_temp_otocz,
            "min_bad2": rq.min_bad2_temp_otocz,
        }),
        "temperature_close": get_long_quality(values.get("temperature_close"), {
            "max_bad": rq.max_bad_temp_zbliz,
            "max_mid": rq.max_mid_temp_zbliz,
            "max_good": rq.max_good_temp_zbliz,
            "max_mid2": rq.max_mid2_temp_zbliz,
            "min_bad2": rq.min_bad2_temp_zbliz,
        }),
        "humidity": get_long_quality(values.get("humidity"), {
            "max_bad": rq.max_bad_wiglotnosc,
            "max_mid": rq.max_mid_wiglotnosc,
            "max_good": rq.max_good_wiglotnosc,
            "max_mid2": rq.max_mid2_wiglotnosc,
            "min_bad2": rq.min_bad2_wiglotnosc,
        }),
        "co2": get_short_quality(values.get("co2"), {
            "max_good": rq.max_good_co2,
            "min_mid": rq.min_mid_co2,
            "max_mid": rq.max_mid_co2,
            "min_bad": rq.min_bad_co2,
        }),
        "tvoc": get_short_quality(values.get("tvoc"), {
            "max_good": rq.max_good_tvoc,
            "min_mid": rq.min_mid_tvoc,
            "max_mid": rq.max_mid_tvoc,
            "min_bad": rq.min_bad_tvoc,
        }),
        "eco2": get_short_quality(values.get("eco2"), {
            "max_good": rq.max_good_eco2,
            "min_mid": rq.min_mid_eco2,
            "max_mid": rq.max_mid_eco2,
            "min_bad": rq.min_bad_eco2,
        }),
        "aqi": get_short_quality(values.get("aqi"), {
            "max_good": rq.max_good_aqi,
            "min_mid": rq.min_mid_aqi,
            "max_mid": rq.max_mid_aqi,
            "min_bad": rq.min_bad_aqi,
        }),
    }

    parameters = [
        {"key": "temperature_ambient", "label": "Temperatura (otoczenie)", "unit": "°C"},
        {"key": "temperature_close", "label": "Temperatura (zbliżenie)", "unit": "°C"},
        {"key": "humidity", "label": "Wilgotność", "unit": "%"},
        {"key": "co2", "label": "CO₂", "unit": "ppm"},
        {"key": "tvoc", "label": "TVOC", "unit": "ppb"},
        {"key": "eco2", "label": "eCO₂", "unit": "ppm"},
        {"key": "aqi", "label": "Jakość Powietrza (AQI)", "unit": ""},
    ]

    return render(request, 'eye_app/room-overall.html', {
        'room': room,
        'visibilities': visibilities,
        'parameters': parameters,
        'values': values,
        'qualities': qualities,
        'user_access': user_access,
        'last_seen': room.device_id.last_seen if room.device_id else None  # ⬅️ dodane
    })

@login_required
def room_advanced(request, room_id):
    room = get_object_or_404(Room, pk=room_id)

    access = Access.objects.filter(room=room, user=request.user).first()
    if not (request.user.is_staff or (access and access.has_read_access)):
        return render(request, 'eye_app/no_permission.html', {'room': room})

    visibilities = {
        pv.parameter: pv.visible
        for pv in ParameterVisibility.objects.filter(room=room)
    }

    timestamp_range = Measurement.objects.filter(dev_id=room.device_id).aggregate(
        earliest=Min("timestamp"),
        latest=Max("timestamp")
    )
    fallback_now = datetime.now()
    earliest = timestamp_range["earliest"] or fallback_now
    latest = timestamp_range["latest"] or fallback_now

    start_str = request.GET.get("start")
    end_str = request.GET.get("end")

    if start_str and end_str:
        parsed = parse_date_range(start_str, end_str)
        if parsed[0] and parsed[1]:
            start, end = parsed
        else:
            start = datetime.combine(earliest.date(), dtime.min)
            end = datetime.combine(latest.date(), dtime.max)
    else:
        start = datetime.combine(earliest.date(), dtime.min)
        end = datetime.combine(latest.date(), dtime.max)

    measurements = (
        Measurement.objects
        .filter(dev_id=room.device_id, timestamp__range=(start, end))
        .order_by('timestamp')
        .select_related('sen22396', 'ens160', 'gy906', 'dht11')
    )

    def extract_series(ms, accessor):
        result = []
        for m in ms:
            try:
                val = accessor(m)
                if val is not None:
                    result.append({
                        "x": m.timestamp.strftime('%Y-%m-%d %H:%M:%S'),
                        "y": val
                    })
            except:
                continue
        return result

    chart_data = {
        "temperature_ambient": extract_series(measurements, lambda m: m.sen22396.temperature if m.sen22396 else None),
        "humidity": extract_series(measurements, lambda m: m.sen22396.humidity if m.sen22396 else None),
        "co2": extract_series(measurements, lambda m: m.sen22396.co2 if m.sen22396 else None),
        "temperature_close": extract_series(measurements, lambda m: m.gy906.temperature if m.gy906 else None),
        "tvoc": extract_series(measurements, lambda m: m.ens160.tvoc if m.ens160 else None),
        "eco2": extract_series(measurements, lambda m: m.ens160.eco2 if m.ens160 else None),
        "aqi": extract_series(measurements, lambda m: m.ens160.aqi if m.ens160 else None),
    }

    rq, _ = RoomQualities.objects.get_or_create(room=room)

    qualities = {
        "temperature_ambient": {
            "max_bad": rq.max_bad_temp_otocz,
            "max_mid": rq.max_mid_temp_otocz,
            "max_good": rq.max_good_temp_otocz,
            "max_mid2": rq.max_mid2_temp_otocz,
            "min_bad2": rq.min_bad2_temp_otocz,
        },
        "temperature_close": {
            "max_bad": rq.max_bad_temp_zbliz,
            "max_mid": rq.max_mid_temp_zbliz,
            "max_good": rq.max_good_temp_zbliz,
            "max_mid2": rq.max_mid2_temp_zbliz,
            "min_bad2": rq.min_bad2_temp_zbliz,
        },
        "humidity": {
            "max_bad": rq.max_bad_wiglotnosc,
            "max_mid": rq.max_mid_wiglotnosc,
            "max_good": rq.max_good_wiglotnosc,
            "max_mid2": rq.max_mid2_wiglotnosc,
            "min_bad2": rq.min_bad2_wiglotnosc,
        },
        "co2": {
            "max_good": rq.max_good_co2,
            "min_mid": rq.min_mid_co2,
            "max_mid": rq.max_mid_co2,
            "min_bad": rq.min_bad_co2,
        },
        "tvoc": {
            "max_good": rq.max_good_tvoc,
            "min_mid": rq.min_mid_tvoc,
            "max_mid": rq.max_mid_tvoc,
            "min_bad": rq.min_bad_tvoc,
        },
        "eco2": {
            "max_good": rq.max_good_eco2,
            "min_mid": rq.min_mid_eco2,
            "max_mid": rq.max_mid_eco2,
            "min_bad": rq.min_bad_eco2,
        },
        "aqi": {
            "max_good": rq.max_good_aqi,
            "min_mid": rq.min_mid_aqi,
            "max_mid": rq.max_mid_aqi,
            "min_bad": rq.min_bad_aqi,
        },
    }

    quality_functions = {
        "temperature_ambient": get_long_quality,
        "temperature_close": get_long_quality,
        "humidity": get_long_quality,
        "co2": get_short_quality,
        "tvoc": get_short_quality,
        "eco2": get_short_quality,
        "aqi": get_short_quality,
    }

    chart_stats = {}

    for key in chart_data:
        values = chart_data[key]

        if not values:
            chart_stats[key] = {
                "latest": None,
                "min": None,
                "max": None,
            }
            continue

        latest_point = max(values, key=lambda d: d["x"])
        min_point = min(values, key=lambda d: d["y"])
        max_point = max(values, key=lambda d: d["y"])
        threshold = qualities.get(key)
        quality_fn = quality_functions.get(key)

        chart_stats[key] = {
            "latest": {
                "value": latest_point["y"],
                "timestamp": latest_point["x"],
                "quality": quality_fn(latest_point["y"], threshold) if threshold and quality_fn else None
            },
            "min": {
                "value": min_point["y"],
                "timestamp": min_point["x"],
                "quality": quality_fn(min_point["y"], threshold) if threshold and quality_fn else None
            },
            "max": {
                "value": max_point["y"],
                "timestamp": max_point["x"],
                "quality": quality_fn(max_point["y"], threshold) if threshold and quality_fn else None
            },
        }

    parameters = [
        {"key": "temperature_ambient", "label": "Temperatura (otoczenie)", "unit": "°C"},
        {"key": "temperature_close", "label": "Temperatura (zbliżenie)", "unit": "°C"},
        {"key": "humidity", "label": "Wilgotność", "unit": "%"},
        {"key": "co2", "label": "CO₂", "unit": "ppm"},
        {"key": "tvoc", "label": "TVOC", "unit": "ppb"},
        {"key": "eco2", "label": "eCO₂", "unit": "ppm"},
        {"key": "aqi", "label": "Jakość powietrza (AQI)", "unit": ""},
    ]

    return render(request, 'eye_app/room-advanced.html', {
        "room": room,
        "visibilities": visibilities,
        "user_access": access,
        "chart_data": chart_data,
        "parameters": parameters,
        "chart_stats": chart_stats,
        "start": start.strftime('%Y-%m-%d'),
        "end": end.strftime('%Y-%m-%d'),
        "start_iso": start.strftime('%Y-%m-%d %H:%M:%S'),
        "end_iso": end.strftime('%Y-%m-%d %H:%M:%S'),
        "full_range_start": earliest.strftime('%Y-%m-%d'),
        "full_range_end": latest.strftime('%Y-%m-%d'),
        "last_seen": room.device_id.last_seen if room.device_id else None  # ⬅️ dodane
    })

@login_required
def room_variables_visibility(request, room_id):
    room = get_object_or_404(Room, pk=room_id)
    try:
        user_access = Access.objects.get(room=room, user=request.user)
    except Access.DoesNotExist:
        user_access = None

    permission_result = check_view_edit_permission(request, room_id)
    if permission_result is not True:
        return permission_result

    parameters = [
        {"key": "temperature_ambient", "label": "Temperatura (otoczenie)"},
        {"key": "temperature_close", "label": "Temperatura (zbliżenie)"},
        {"key": "humidity", "label": "Wilgotność"},
        {"key": "co2", "label": "CO₂"},
        {"key": "tvoc", "label": "TVOC"},
        {"key": "eco2", "label": "eCO₂"},
        {"key": "aqi", "label": "Jakość powietrza (AQI)"},
    ]

    if request.method == "POST":
        for param in parameters:
            visible = request.POST.get(param["key"]) == "on"
            obj, _ = ParameterVisibility.objects.get_or_create(room=room, parameter=param["key"])
            obj.visible = visible
            obj.save()
        return redirect('room_variables_visibility', room_id=room.id)

    visibilities = {
        pv.parameter: pv.visible for pv in ParameterVisibility.objects.filter(room=room)
    }

    for param in parameters:
        param["visible"] = visibilities.get(param["key"], True)

    return render(request, 'eye_app/room-variables-visibility.html', {
        'room': room,
        'parameters': parameters,
        'user_access': user_access,
        'last_seen': room.device_id.last_seen if room.device_id else None,  # ⬅️ dodane
    })

# @user_passes_test(lambda u: u.is_staff)
@login_required
def manage_devices(request):
    if not request.user.is_staff:
        return redirect('main')

    if request.method == 'POST':
        form = CreateDeviceForm(request.POST)
        if form.is_valid():
            device = form.save(commit=False)
            device.ip = "0.0.0.0"
            device.save()
            return redirect('manage_devices')
    else:
        form = CreateDeviceForm()

    devices = SensorDevice.objects.all()
    return render(request, 'eye_app/manage-devices.html', {
        'devices': devices,
        'form': form,
    })

@login_required
def edit_device(request, sensordevice_id):
    if not request.user.is_staff:
        source_url = request.build_absolute_uri()
        return render(request, 'eye_app/no_permission_other.html', {'source': source_url})

    device = get_object_or_404(SensorDevice, id=sensordevice_id)

    if request.method == 'POST':
        device.device_uuid = request.POST.get('uuid', device.device_uuid)
        device.key = request.POST.get('key', device.key)
        device.description = request.POST.get('description', device.description)
        device.ip = request.POST.get('ip_address', device.ip)
        device.save()
        return redirect('manage_devices')

    return render(request, 'eye_app/edit-device.html', {'device': device})

@login_required
def delete_device(request, sensordevice_id):
    if not request.user.is_staff:
        source_url = request.build_absolute_uri()
        return render(request, 'eye_app/no_permission_other.html',
                      {'source': source_url})
    device = get_object_or_404(SensorDevice, id=sensordevice_id)
    device.delete()
    return redirect('manage_devices')

@user_passes_test(lambda u: u.is_staff)
@login_required
def manage_accounts(request):
    if not request.user.is_staff:
        source_url = request.build_absolute_uri()
        return render(request, 'eye_app/no_permission_other.html',
                      {'source': source_url})
    users = User.objects.all()
    return render(request, 'eye_app/manage-accounts.html', {'users': users})

@login_required
@csrf_exempt
def add_account(request):
    if not request.user.is_staff:
        source_url = request.build_absolute_uri()
        return render(request, 'eye_app/no_permission_other.html',
                      {'source': source_url})
    if request.method == 'POST':
        User.objects.create_user(
            username=request.POST['username'],
            email=request.POST.get('email', ''),
            first_name=request.POST.get('first_name', ''),
            last_name=request.POST.get('last_name', ''),
            password=request.POST['password'],
            is_staff=bool(request.POST.get('is_staff'))
        )
    return redirect('manage_accounts')

@login_required
def edit_account(request, user_id):
    if not request.user.is_staff:
        source_url = request.build_absolute_uri()
        return render(request, 'eye_app/no_permission_other.html',
                      {'source': source_url})
    user = get_object_or_404(User, id=user_id)
    if request.method == 'POST':
        user.username = request.POST['username']
        user.email = request.POST.get('email', '')
        user.first_name = request.POST.get('first_name', '')
        user.last_name = request.POST.get('last_name', '')
        user.is_staff = bool(request.POST.get('is_staff'))
        if request.POST.get('password'):
            user.set_password(request.POST['password'])
        user.save()
        return redirect('manage_accounts')
    return render(request, 'eye_app/edit-account.html', {'user_obj': user})

@login_required
def delete_account(request, user_id):
    if not request.user.is_staff:
        source_url = request.build_absolute_uri()
        return render(request, 'eye_app/no_permission_other.html',
                      {'source': source_url})
    user = get_object_or_404(User, id=user_id)
    user.delete()
    return redirect('manage_accounts')

def register(request):
    if request.method == 'POST':
        form = UserCreationForm(request.POST)
        if form.is_valid():
            form.save()
            return redirect('login')
    else:
        form = UserCreationForm()
    return render(request, 'eye_app/register.html', {'form': form})

class CustomLogoutView(View):
    def get(self, request):
        logout(request)
        return render(request, 'eye_app/logged_out.html')

def make_json(body_bytes):
    try:
        body_str = unpad(body_bytes).decode('utf-8')
        return json.loads(body_str)
    except (json.JSONDecodeError, UnicodeDecodeError, IndexError):
        return ""

def unpad(padded_data: bytes) -> bytes:
    pad_len = padded_data[-1]
    return padded_data[:-pad_len]

def check_if_dev_is_in_db(data,time_epoch):
    dev_uuid = data.get("dev_uuid")
    dev_addr = data.get("dev_addr")
    timestamp_seconds = time_epoch / 1000.0

    naive_datetime = datetime.fromtimestamp(timestamp_seconds)
    timestamp = pytz.utc.localize(naive_datetime)
    try:
        device = SensorDevice.objects.get(device_uuid=dev_uuid)
        device.ip = dev_addr
        device.last_seen = timestamp
        device.save()
        return device
    except SensorDevice.DoesNotExist:
        return JsonResponse({"error": "There is no such a device in db"}, status=400)

@transaction.atomic
def add_sensors_data(device, sensors, time_epoch):
    ens160_instance = None
    gy906_instance = None
    sen22396_instance = None

    for sensor in sensors:
        name = sensor.get("name")
        readings = sensor.get("readings", {})

        if name == "ens160":
            ens160_instance = ens160.objects.create(
                aqi=float(readings.get("aqi", 0)),
                tvoc=float(readings.get("tvoc", 0)),
                eco2=float(readings.get("eco2", 0))
            )
        elif name == "sen22396":
            sen22396_instance = sen22396.objects.create(
                co2=float(readings.get("co2", 0)),
                temperature=float(readings.get("temperature", 0)),
                humidity=float(readings.get("humidity", 0))
            )
        elif name == "mlx90614":
            gy906_instance = gy906.objects.create(
                temperature=float(readings.get("object_temperature", 0))
            )

    timestamp_seconds = time_epoch / 1000.0
    timestamp = datetime.fromtimestamp(timestamp_seconds, tz=pytz.UTC)
    print(f"Original epoch_ms: {time_epoch}")
    print(f"Converted timestamp (naive): {timestamp}")

    Measurement.objects.create(
        dev_id=device,
        timestamp=timestamp,
        ens160=ens160_instance,
        gy906=gy906_instance,
        sen22396=sen22396_instance,
        dht11=None
    )

# -- Embedded_API
@csrf_exempt
def api_time(request):
    epoch_ms = int(pytime.time() * 1000)
    response_data = {
        "time_epoch_ms": epoch_ms
    }
    print(response_data)
    return JsonResponse(response_data)


def proccess_room_action(device, sensors):
    rooms = Room.objects.filter(device_id=device)

    def get_sens_val(sens_name, reading_name):
        for sensor in sensors:
            if sensor.get("name") == sens_name:
                return float(sensor.get("readings", {}).get(reading_name, 0))
        return -1.0

    def http_post_async(room, action, value):
        def task():
            json_body = {
                "room_number": room.room_number,
                "action_parameter": action.parameter,
                "action_name": action.custom_name,
                "action_value": value
            }
            try:
                response = requests.post(action.url, json=json_body, timeout=3)
                print(f"[HTTP POST] Sent to {action.url} | Status: {response.status_code}")
            except Exception as e:
                print(f"[HTTP POST ERROR] {e}")
        threading.Thread(target=task).start()

    for room in rooms:
        actions = RoomActions.objects.filter(room=room)
        for action in actions:
            value = None
            if action.parameter == "temperature_ambient":
                value = get_sens_val("sen22396", "temperature")
            elif action.parameter == "temperature_close":
                value = get_sens_val("mlx90614", "object_temperature")
            elif action.parameter == "humidity":
                value = get_sens_val("sen22396", "humidity")
            elif action.parameter == "co2":
                value = get_sens_val("sen22396", "co2")
            elif action.parameter == "tvoc":
                value = get_sens_val("ens160", "tvoc")
            elif action.parameter == "eco2":
                value = get_sens_val("ens160", "eco2")
            elif action.parameter == "aqi":
                value = get_sens_val("ens160", "aqi")
            else:
                continue

            if action.min <= value <= action.max:
                http_post_async(room, action, value)


@csrf_exempt
def api_post(request):
    body = request.body.decode('utf-8') if request.body else "{}"

    try:
        data = json.loads(body)

        time_epoch = data.get("time_epoch")
        device = check_if_dev_is_in_db(data,time_epoch)

        sensors = data.get("data",{}).get("sensors",[])
        add_sensors_data(device, sensors, time_epoch)
        proccess_room_action(device, sensors)
        print(f"Parsed JSON:\n{data}")
    except json.JSONDecodeError:
        return "Invalid JSON", 400
    return JsonResponse({}, status=200)
    
@csrf_exempt
def api_post_encrypted(request):
    body = request.body.decode('utf-8') if request.body else "{}"
    print(body)
    try:
        data = json.loads(body)
        dev_uuid = data.get("dev_uuid")
        if not dev_uuid:
            return JsonResponse({"error": "Brak dev_uuid"}, status=400)

        try:
            device_obj = SensorDevice.objects.get(device_uuid=dev_uuid)
        except SensorDevice.DoesNotExist:
            return JsonResponse({"error": "Nie znaleziono urządzenia"}, status=404)

        hex_key = device_obj.key
        key = bytes.fromhex(hex_key)

        time_epoch = data.get("time_epoch")
        device = check_if_dev_is_in_db(data,time_epoch)
        encrypted_hex = data.get("encrypted")

        encrypted_bytes = bytes.fromhex(encrypted_hex)


        cipher = AES.new(key,AES.MODE_ECB)
        decrypted = cipher.decrypt(encrypted_bytes)
        decrypted_json = make_json(decrypted)
        if not decrypted_json:
            return JsonResponse({"error": "Błąd podczas odszyfrowania"}, status=400)

        sensors = decrypted_json.get("data",{}).get("sensors",[])
        # time_epoch = decrypted_json.get("time_epoch")
        try:
            add_sensors_data(device,sensors,time_epoch)
        except Exception as e:
            return JsonResponse({"error": f"Błąd przy zapisie danych: {str(e)}"}, status=500)

        try:
            proccess_room_action(device, sensors)
        except Exception as e:
            print( f"Problem z post do zewn. serwera: {str(e)}")

    except json.JSONDecodeError:
        return JsonResponse({"error": "Invalid JSON"}, status=400)

    except Exception as e:
        return JsonResponse({"error": str(e)}, status=500)


    return JsonResponse({}, status=200)


@csrf_exempt
def api_ping(request):
    return JsonResponse({})
