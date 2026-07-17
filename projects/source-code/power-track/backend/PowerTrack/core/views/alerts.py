from idlelib.configdialog import is_int

from django.contrib.auth.decorators import login_required

from .base import *

"""
    Zarządzanie alertami
"""

__TITLE__: str = "Zarządzanie Zdarzeniami"
__REDIRECT__: str = "manage_alerts"

@login_required
def manage_alerts(request):
    """
      Widok odpowiada za wyświetlenie ekranu - Zarządzanie Zdarzeniami - listy Profilów Zdarzeń - wraz z wbudowanym mechanizmem filtrowania
    :param request:
    :return:
    """
    # -- -- Weryfikacja Uprawnień -- -- #
    if not is_admin_or_operator(request):
      message_no_access(request, __TITLE__)
      return redirect('error_view')
    # -- -- Weryfikacja Żądania -- -- #
    if not is_method_get(request):
      message_bad_method(request, __TITLE__)
      return redirect(__REDIRECT__)
    # -- -- #
    meters = ElectricMeter.objects.all()
    # -- -- -- filtrowanie -- -- --
    fname = request.GET.get('fname')
    fis_active = request.GET.get('fis_active')
    fphases = request.GET.get('fphases')
    alert_profiles = AlertProfile.objects.all()
    # -- -- #
    if fname:
        alert_profiles = alert_profiles.filter(name__icontains=fname)
    if fis_active:
        alert_profiles = alert_profiles.filter(is_active= ( fis_active == "Tak" ) )
    if fphases:
        if "1" in fphases:
            alert_profiles = alert_profiles.filter(l1_enable=True)
        if "2" in fphases:
            alert_profiles = alert_profiles.filter(l2_enable=True)
        if "3" in fphases:
            alert_profiles = alert_profiles.filter(l3_enable=True)
    # -- -- #
    form_vals = {
     'fname': fname or "",
     'fis_active': fis_active or "",
     'fphases': fphases or ""
    }
    # -- -- Paginacja -- -- #
    paginator = Paginator(alert_profiles, 25)
    page_number = request.GET.get('page')
    page_object = paginator.get_page(page_number)
    params = request.GET.copy()
    if "page" in params:
        del params["page"]
    # , 'params': params.urlencode()
    # -- -- -- -- -- -- -- -- #
    return render(request, 'core/administrator/manage_alerts.html',{'meters': meters, 'alert_profiles': page_object, 'form_vals': form_vals, 'params': params.urlencode()})

@login_required
def manage_add_alert(request):
    """
        Metoda odpowiada za dodanie Profilu Zdarzeń, w tym pobranie danych z formularza, weryfikacje oraz utworzenie profilu.
        :param request:
        :return:
    """
    # -- -- Weryfikacja Uprawnień -- -- #
    if not is_admin_or_operator(request):
      message_no_access(request, __TITLE__)
      return redirect('error_view')
    # -- -- Weryfikacja Żądania -- -- #
    if not is_method_post(request):
      message_bad_method(request, __TITLE__)
      return redirect(__REDIRECT__)
    # -- -- Nazwa Alertu -- -- #
    fname = request.POST.get('name')
    is_active = request.POST.get('is_active') == 'on'
    # -- -- Znaczniki -- -- #
    fl1 = request.POST.get('fl1_active') == 'on'
    fl2 = request.POST.get('fl2_active') == 'on'
    fl3 = request.POST.get('fl3_active') == 'on'
    fvoltage = request.POST.get('fvoltage_active') == 'on'
    fcurrent = request.POST.get('fcurrent_active') == 'on'
    ffrequency = request.POST.get('ffrequency_active') == 'on'
    factive_power = request.POST.get('factive_power_active') == 'on'
    freactive_power = request.POST.get('freactive_power_active') == 'on'
    fpower_factor = request.POST.get('fpower_factor_active') == 'on'
    # -- -- Zakresy -- -- #
    fmin_voltage = to_int(request.POST.get('fvoltage_min'))
    fmax_voltage = to_int(request.POST.get('fvoltage_max'))
    # -- -- -- -- #
    fmin_current = to_int(request.POST.get('fcurrent_min'))
    fmax_current = to_int(request.POST.get('fcurrent_max'))
    # -- -- -- -- #
    fmin_frequency = to_float(request.POST.get('ffrequency_min'))
    fmax_frequency = to_float(request.POST.get('ffrequency_max'))
    # -- -- -- -- #
    fmin_active_power = to_int(request.POST.get('factive_power_min'))
    fmax_active_power = to_int(request.POST.get('factive_power_max'))
    # -- -- -- -- #
    fmin_reactive_power = to_int(request.POST.get('freactive_power_min'))
    fmax_reactive_power = to_int(request.POST.get('freactive_power_max'))
    # -- -- -- -- #
    fmin_power_factor = to_float(request.POST.get('fpower_factor_min'))
    fmax_power_factor = to_float(request.POST.get('fpower_factor_max'))
    # -- -- Weryfikacja Pól-- -- #
    required_fields = [
        fmin_voltage, fmax_voltage, fmin_current, fmax_current, fmin_frequency, fmax_frequency,
        fmin_active_power, fmax_active_power, fmin_reactive_power, fmax_reactive_power, fmin_power_factor,
        fmax_power_factor
    ]
    if any(field is None for field in required_fields):
        message_error(request, __TITLE__, "Nie udało się poprawnie wczytać pól formularza.")
        return redirect(__REDIRECT__)
    # -- -- Weryfikacja Formularza -- -- #
    if not (len(fname) in range(4,65) and
            fmin_voltage in range(-400, 401) and fmax_voltage in range(-400, 401) and
            fmin_current in range(-255, 256) and fmax_current in range(-255, 256) and
            int(fmin_frequency) in range(-500, 501) and int(fmax_frequency) in range(-500, 501) and
            fmin_active_power in range(-100000, 100000) and fmax_active_power in range(-100000, 100000) and
            fmin_reactive_power in range(-100000, 100000) and fmax_reactive_power in range(-100000, 100000) and
            int(fmin_power_factor) in range(0, 2) and int(fmax_power_factor) in range(0,2)):
        message_error(request, __TITLE__, "Nieprawidłowe wartości w formularzu.")
        return redirect(__REDIRECT__)
    # -- -- -- -- #
    if (fmin_voltage>fmax_voltage or fmin_current>fmax_current or fmin_frequency>fmax_frequency or
        fmin_active_power>fmax_active_power or fmin_reactive_power>fmax_reactive_power or fmin_power_factor>fmax_power_factor):
        message_error(request, __TITLE__, "Minimum nie może być większe od maksimum.")
        return redirect(__REDIRECT__)
    # -- -- Weryfikacja unikalności -- -- #
    if AlertProfile.objects.filter(name=fname).exists():
        message_error(request, __TITLE__, "Nazwa profilu jest już zajęta.")
        return redirect(__REDIRECT__)
    # -- -- Utworzenie Profilu -- -- #
    alert_profile = AlertProfile.objects.create(
        name = fname,
        is_active = is_active,
        monitor_voltage = fvoltage,
        monitor_current = fcurrent,
        monitor_frequency = ffrequency,
        monitor_active_power = factive_power,
        monitor_reactive_power = freactive_power,
        monitor_power_factor = fpower_factor,
        l1_enable = fl1,
        l2_enable = fl2,
        l3_enable = fl3,
        min_voltage = fmin_voltage,
        max_voltage = fmax_voltage,
        min_frequency = fmin_frequency,
        max_frequency = fmax_frequency,
        min_current = fmin_current,
        max_current = fmax_current,
        min_active_power = fmin_active_power,
        max_active_power = fmax_active_power,
        min_reactive_power = fmin_reactive_power,
        max_reactive_power = fmax_reactive_power,
        min_power_factor = fmin_power_factor,
        max_power_factor = fmax_power_factor
    )

    if alert_profile:
        message_success(request, __TITLE__, MESSAGE_STATUS.SUCCESS_ADD)
    else:
        message_error(request, __TITLE__, MESSAGE_STATUS.ERROR_ADD)
    return redirect(__REDIRECT__)

@login_required
def manage_edit_alert(request, alert_id):
    """
      Metoda odpowiada za edytowaniu Profilu Zdarzeń.
      :param request:
      :param alert_id:
      :return:
    """
    # -- -- Weryfikacja Uprawnień -- -- #
   # Logic hidden for security and copyright protection reasons (engineering thesis).
    # Full source code is available to recruiters upon request.
    
    # -- -- Weryfikacja Żądania -- -- #
    # Logic hidden for security and copyright protection reasons (engineering thesis).
    # Full source code is available to recruiters upon request.
    

    # -- -- Nazwa Alertu -- -- #
    # Logic hidden for security and copyright protection reasons (engineering thesis).
    # Full source code is available to recruiters upon request.
    
    # -- -- Znaczniki -- -- #
    # Logic hidden for security and copyright protection reasons (engineering thesis).
    # Full source code is available to recruiters upon request.
    
    # -- -- Zakresy -- -- #
    fmin_voltage = to_int(request.POST.get('fvoltage_min'))
    fmax_voltage = to_int(request.POST.get('fvoltage_max'))
    # -- -- -- -- #
    fmin_current = to_int(request.POST.get('fcurrent_min'))
    fmax_current = to_int(request.POST.get('fcurrent_max'))
    # -- -- -- -- #
    fmin_frequency = to_float(request.POST.get('ffrequency_min'))
    fmax_frequency = to_float(request.POST.get('ffrequency_max'))
    # -- -- -- -- #
    fmin_active_power = to_int(request.POST.get('factive_power_min'))
    fmax_active_power = to_int(request.POST.get('factive_power_max'))
    # -- -- -- -- #
    fmin_reactive_power = to_int(request.POST.get('freactive_power_min'))
    fmax_reactive_power = to_int(request.POST.get('freactive_power_max'))
    # -- -- -- -- #
    fmin_power_factor = to_float(request.POST.get('fpower_factor_min'))
    fmax_power_factor = to_float(request.POST.get('fpower_factor_max'))
    # -- -- Weryfikacja Pól-- -- #
   # Logic hidden for security and copyright protection reasons (engineering thesis).
    # Full source code is available to recruiters upon request.
    
    # -- -- Weryfikacja Formularza -- -- #
   # Logic hidden for security and copyright protection reasons (engineering thesis).
    # Full source code is available to recruiters upon request.
    
    # -- -- -- -- #
    # Logic hidden for security and copyright protection reasons (engineering thesis).
    # Full source code is available to recruiters upon request.
    
    # -- -- Weryfikacja unikalności -- -- #
   # Logic hidden for security and copyright protection reasons (engineering thesis).
    # Full source code is available to recruiters upon request.
    
    # -- -- Utworzenie Profilu -- -- #
   # Logic hidden for security and copyright protection reasons (engineering thesis).
    # Full source code is available to recruiters upon request.
    

@login_required
def manage_delete_alert(request, alert_id):
    """
        Metoda odpowiada za mechanizm usuwania profilu zdarzeń
    :param request:
    :param alert_id:
    :return:
    """
    # -- -- Weryfikacja Uprawnień -- -- #
    if not is_admin(request):
      message_no_access(request, __TITLE__)
      return redirect('error_view')
    # -- -- Weryfikacja Żądania -- -- #
    if not is_method_post(request):
      message_bad_method(request, __TITLE__)
      return redirect(__REDIRECT__)
    alert_profile = AlertProfile.objects.filter(id=alert_id).first()
    if not alert_profile:
       message_error(request, __TITLE__, "Nie odnaleziono.")
       return redirect(__REDIRECT__)
    message_success(request, __TITLE__, MESSAGE_STATUS.SUCCESS_DEL)
    alert_profile.delete()
    return redirect(__REDIRECT__)