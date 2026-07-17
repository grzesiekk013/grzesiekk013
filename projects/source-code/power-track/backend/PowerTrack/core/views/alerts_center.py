from .base import *

"""
    Podgląd zdarzeń, umożliwia przeglądanie zdarzeń - przekroczeń zarówno dla operatorów jak i użytkowników systemu.
    Metody mają wbudowany mechanizm rozróżniania treści zgodnie z rolą użytkownika.
    Lista mechanizmów:
        - filtrowanie wyników
        - podgląd zdarzenia oraz wydruk
        - potwierdzanie zdarzeń przez operatorów i administratorów
        - usuwanie zdarzeń
"""

__TITLE__: str = "Podgląd zdarzeń"
__REDIRECT__: str = "alerts_center"

@login_required
def alerts_center(request):
    """
      Widok odpowiada za wyświetlenie ekranu - Podglądu Zdarzeń - listy zdarzeń wraz z powiązanymi atrybutami
    :param request:
    :return:
    """
    # -- -- Weryfikacja Uprawnień -- -- #
    if not (is_admin_or_operator(request) or is_user(request)):
      message_no_access(request, __TITLE__)
      return redirect('error_view')
    # -- -- Weryfikacja Żądania -- -- #
    if not is_method_get(request):
      message_bad_method(request, __TITLE__)
      return redirect(__REDIRECT__)
    # -- -- #
    meters = None
    alerts = None
    alert_profiles = None
    if is_admin_or_operator(request):
       meters = ElectricMeter.objects.all().select_related('channel__device__switch_board', 'room', 'alert_profile')
       alerts = Alert.objects.all().select_related('energy_meter', 'room', 'alert_profile')
       alert_profiles = AlertProfile.objects.all()
    else:
       client_group = None
       try:
           user_role = UserRole.objects.select_related('client').get(user=request.user)
           client_group = user_role.client
       except UserRole.DoesNotExist:
           message_error(request, __TITLE__, "Nie przypisano do żadnej grupy.")

       if client_group:
           meters = ElectricMeter.objects.filter(room__room_group=client_group).select_related('room', 'channel__device', 'alert_profile')
           alerts = Alert.objects.filter(energy_meter__room__room_group=client_group).select_related('energy_meter', 'room', 'alert_profile')
           alert_profiles = AlertProfile.objects.filter(electricmeter__room__room_group=client_group).distinct()
       else:
           meters = []
           alerts = []

    # -- -- -- filtrowanie -- -- --
    fname = request.GET.get('fname') # Message
    falert = request.GET.get('falert') # Profile
    fconfirmed = request.GET.get('fconfirmed')
    fmeter = request.GET.get('fmeter')
    # -- -- #
    if fname:
        alerts = alerts.filter(message__icontains=fname)
    if falert:
        alerts = alerts.filter(alert_profile__id=falert)
    if fconfirmed:
        if is_admin_or_operator(request):
            alerts = alerts.filter(confirmed_admin_operator= (fconfirmed == "Tak") )
        else:
            alerts = alerts.filter(confirmed_user = (fconfirmed == "Tak") )
    if fmeter:
        alerts = alerts.filter(energy_meter__id=fmeter)

    # -- -- #
    form_vals = {
     'fname': fname or "",
     'falert': falert or "",
     'fconfirmed': fconfirmed or "",
     'fmeter': fmeter or "",
    }
    # -- -- Paginacja -- -- #
    paginator = Paginator(alerts, 25)
    page_number = request.GET.get('page')
    page_object = paginator.get_page(page_number)
    params = request.GET.copy()
    if "page" in params:
        del params["page"]
    # , 'params': params.urlencode()
    # -- -- -- -- -- -- -- -- #
    return render(request, 'core/all/alerts_center.html',{'meters': meters, 'alert_profiles': alert_profiles, 'alerts':page_object,
                                                                    'form_vals': form_vals, 'params': params.urlencode()})

@login_required
def confirm_alert_center(request, alert_id):
    """
      Metoda odpowiada za potwierdzenie zdarzenia wraz z rozróżnieniem ról.
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
    
    # -- -- Odnalezienie alertu i edycja flagi -- -- #
    # Logic hidden for security and copyright protection reasons (engineering thesis).
    # Full source code is available to recruiters upon request.
    

@login_required
def show_alert_center(request, alert_id):
    """
      Metoda odpowiada za wyświetlenie strony do druku ze szczegółami zdarzenia.
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
    
    # -- -- Odnalezienie alertu i edycja flagi -- -- #
    # Logic hidden for security and copyright protection reasons (engineering thesis).
    # Full source code is available to recruiters upon request.
    
    return render(request, 'core/all/show_alert_center.html',{'alert': alert})

@login_required
def delete_alert_center(request, alert_id):
    """
        Metoda odpowiada za mechanizm usuwania zdarzenia
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
    