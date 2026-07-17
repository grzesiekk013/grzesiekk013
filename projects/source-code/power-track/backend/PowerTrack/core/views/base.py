from logging import exception
from django.contrib.auth.models import User
from django.contrib import messages
from django.core.paginator import Paginator
from django.contrib.auth.decorators import login_required
from django.shortcuts import render, redirect, get_object_or_404
from django.contrib.auth import authenticate, login
from django.http import JsonResponse
from django.views.decorators.csrf import csrf_exempt
from django.db.models import Q
from django.db import transaction
from unicodedata import numeric
from urllib3 import request
from ..models import *
import json, re
from enum import StrEnum

class MESSAGE_STATUS(StrEnum):
   NOT_FOUND = "Nie odnaleziono."
   SUCCESS_ADD = "Utworzono pomyślnie."
   SUCCESS_EDIT = "Pomyślnie edytowano."
   SUCCESS_DEL = "Pomyślnie usunięto."

   ERROR_ADD = "Nie udało się utworzyć z powodu błędu."
   ERROR_DEL = "Nie udało się usunąć z powodu błędu."
   ERROR_EDIT = "Nie udało się edytować z powodu błędu."

# -- -- Metody -- -- #
def is_method_post(request):
   return request.method == "POST"

def is_method_get(request):
   return request.method == "GET"

# -- -- Uprawnienia -- -- #

def is_admin_only(request):
   return get_role_name(request) == 'admin' or request.user.is_staff

def is_admin_or_operator(request):
   return get_role_name(request) == 'admin' or request.user.is_staff or get_role_name(request) == 'operator'

def is_user(request):
   return get_role_name(request) == 'user'


def is_user(request):
   user = request.user
   return (user.is_authenticated and
           hasattr(user, "userrole") and
           user.userrole.role == "user")


def is_operator(request):
   user = request.user
   return (user.is_authenticated and
           hasattr(user, "userrole") and
           user.userrole.role == "operator")


def is_admin(request):
   user = request.user
   return (user.is_authenticated and
           (user.is_staff or hasattr(user, "userrole")) and
           (user.is_staff or user.userrole.role == "admin"))


def get_role_name(request):
   role = ""
   if is_user(request):
      role = "user"
   if is_operator(request):
      role = "operator"
   elif is_admin(request):
      role = "admin"
   return role

# -- -- Powiadomienia -- -- #



def message_no_access(request, title):
   messages.error(request, f"[{title}] Nie masz dostępu do tego zasobu.")

def message_bad_method(request, title):
   messages.error(request, f"[{title}] Niepoprawne żądanie.")

def message_success(request, title, message):
   messages.success(request, f"[{title}] {message}.")

def message_error(request, title, message):
   messages.error(request, f"[{title}] {message}.")
# -- -- -  - --- - -- - #


def to_float(val):
   if not val:
      return None
   try:
      return float(val.replace(',', '.'))
   except:
      return None


def to_int(val):
   if not val:
      return None
   try:
      return int(val)
   except:
      return None

