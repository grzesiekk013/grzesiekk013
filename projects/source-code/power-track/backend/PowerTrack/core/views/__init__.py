from logging import exception
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
from ..models import *
import json, re

from .base import *
from .switch_boards import *
from .devices import *
from .energy_meters import *
from .rooms import *
from .alerts import *
from .users import *
from .messages import *
from .groups import *
from .alerts_center import *
from .account import *
from .device_api import *
from .my_meters import *
from .reports import *

@login_required
def home(request):
    if not is_method_get(request):
        message_error(request, "Strona Główna", "Niepoprawne żądanie.")
        return redirect('home')

    alerts = Alert.objects.all()

    if is_user(request):
        try:
            user_group = request.user.userrole.client
        except:
            user_group = None
        finally:
            pass

        if user_group:
            alerts = alerts.filter(energy_meter__room__room_group=user_group)
        else:
            alerts = alerts.none()


    if not is_user(request):
        not_confirmed = alerts.filter(confirmed_admin_operator=False)
    else:
        not_confirmed = alerts.filter(confirmed_user=False)

    if is_admin(request):
        unread_filter = Q(last_seen_admin__isnull=True) | Q(last_msg_date__gt=F('last_seen_admin'))
    elif is_operator(request):
        unread_filter = Q(last_seen_operator__isnull=True) | Q(last_msg_date__gt=F('last_seen_operator'))
    else:
        unread_filter = Q(last_seen_user__isnull=True) | Q(last_msg_date__gt=F('last_seen_user'))

    chat_filter = Q(created_by=request.user) | Q(assigned_operator=request.user) | Q(assigned_admin=request.user) | Q(assigned_user=request.user)

    messages = Chat.objects.filter(
            Q(created_by=request.user) | Q(assigned_operator=request.user) | Q(assigned_admin=request.user) | Q(assigned_user=request.user)
        )
    unread_messages = Chat.objects.filter(chat_filter).filter(unread_filter).distinct()

    stats_vals = {
        'cnt_alerts_all': len(alerts) if alerts else 0,
        'cnt_alerts_not_confirmed': not_confirmed.count() if not_confirmed else 0,
        'cnt_messages': messages.count() if messages else 0,
        'cnt_unread_messages': unread_messages.count() if unread_messages else 0
    }

    return render(request, 'core/home_page.html', {'stats_vals': stats_vals})

def reports(request):
    return render(request, 'core/reports_home.html', {})

def manage_access(request):
    return render(request, 'core/administrator/manage_access.html', {})

def manage_events(request):
    return render(request, 'core/administrator/manage_events.html', {})

def error_view(request):
    role = get_role_name(request)
    return render(request, 'core/administrator/error_view.html', {'role': role})

