from .base import *

"""
    Zarządzanie licznikami
"""

__TITLE__: str = "Liczniki"
__REDIRECT__: str = "manage_meters"

@login_required
def manage_meters(request):
    # -- -- Weryfikacja Uprawnień -- -- #
    if not is_admin_or_operator(request):
        message_no_access(request, __TITLE__)
        return redirect('error_view')
    # -- -- Weryfikacja Żądania -- -- #
    if not is_method_get(request):
        message_bad_method(request, __TITLE__)
        return redirect(__REDIRECT__)

    meters = ElectricMeter.objects.all()
    channels = Channel.objects.all()
    rooms = Room.objects.filter(electricmeter__isnull=True)
    allrooms = Room.objects.filter()

    # -- -- -- filtrowanie -- -- --
    fchannel = request.GET.get('fchannel')
    faddress = request.GET.get('faddress')
    faddress = to_int(faddress)
    froom = request.GET.get('froom')
    fmodel = request.GET.get('fmodel')
    falert_profile = request.GET.get('fstatus')
    fis_active = request.GET.get('fis_active')

    alert_profiles = AlertProfile.objects.all()

    channel = None
    if fchannel:
        channel = Channel.objects.filter(id=fchannel)
    if fchannel:
        meters = meters.filter(channel=channel)
    if faddress:
        meters = meters.filter(slave_id=faddress)
    if froom:
        meters = meters.filter(room__id=froom)
    if fmodel:
        meters = meters.filter(model__icontains=fmodel)
    if falert_profile:
        meters = meters.filter(alert_profile__name__icontains=falert_profile)
    if fis_active:
        meters = meters.filter(is_active=(fis_active=="Tak"))
    form_vals = {
        'fchannel': fchannel or "",
        'faddress': faddress or "",
        'froom': froom or "",
        'fmodel': fmodel or "",
        'falert_profile': falert_profile or "",
        'fis_active': fis_active or "",
    }
    # -- -- Paginacja -- -- #
    paginator = Paginator(meters, 25)
    page_number = request.GET.get('page')
    page_object = paginator.get_page(page_number)
    params = request.GET.copy()
    if "page" in params:
        del params["page"]
    # , 'params': params.urlencode()
    # -- -- -- -- -- -- -- -- #
    return render(request, 'core/administrator/manage_meters.html',
                  {'meters': page_object, 'channels': channels, 'rooms': rooms, 'allrooms':allrooms, 'form_vals': form_vals, 'alert_profiles': alert_profiles, 'params': params.urlencode()})

@login_required
def manage_add_meter(request):
    # -- -- Weryfikacja Uprawnień -- -- #
    if not is_admin_or_operator(request):
        message_no_access(request, __TITLE__)
        return redirect('error_view')
    # -- -- Weryfikacja Żądania -- -- #
    if not is_method_post(request):
        message_bad_method(request, __TITLE__)
        return redirect(__REDIRECT__)

    channel = None
    room = None
    alert_profile = None
    fis_active = request.POST.get('is_active')
    fslave_id =request.POST.get('slave_id')
    froom = request.POST.get('room')
    fchannel = request.POST.get('channel')
    fmodel = request.POST.get('model')
    falert_profile = request.POST.get('falert_profile')
    if falert_profile:
        alert_profile = AlertProfile.objects.filter(id=falert_profile).first()
    if not fmodel in ["OR-WE-504", "OR-WE-516"]:
        message_error(request, __TITLE__, f"Nieznany model licznika.")
        return redirect('manage_meters')
    if fchannel:
        channel = Channel.objects.filter(id=fchannel).first()
    if not channel:
        message_error(request, __TITLE__, f"Nie wybrano kanału.")
        return redirect('manage_meters')
    if froom:
        room = Room.objects.filter(id=froom).first()
    if not re.fullmatch(r'\d+', fslave_id):
        if not (fslave_id in range(1, 248)):
            message_error(request, __TITLE__, f"Nieprawidłowy adres Modbus.")
            return redirect('manage_meters')
    slave_id_int = int(fslave_id)

    occupied = ElectricMeter.objects.filter(channel=channel, slave_id=slave_id_int).exists()
    if occupied:
        message_error(request, __TITLE__, f"Adres Modbus {slave_id_int} jest już zajęty w tym kanale")
        return redirect('manage_meters')

    em = ElectricMeter.objects.create(
        slave_id=fslave_id,
        model=fmodel,
        channel=channel,
        room = room,
        is_active = fis_active == 'on',
        alert_profile = alert_profile,
        three_phases = True if fmodel in ['OR-WE-516'] else False
    )
    if not em:
        message_error(request, __TITLE__, MESSAGE_STATUS.ERROR_ADD)
        return redirect('manage_meters')

    message_success(request, __TITLE__, MESSAGE_STATUS.SUCCESS_ADD)
    return redirect('manage_meters')

@login_required
def manage_edit_meter(request, meter_id):
    # -- -- Weryfikacja Uprawnień -- -- #
    if not is_admin_or_operator(request):
        message_no_access(request, __TITLE__)
        return redirect('error_view')
    # -- -- Weryfikacja Żądania -- -- #
    if not is_method_post(request):
        message_bad_method(request, __TITLE__)
        return redirect(__REDIRECT__)

    alert_profile = None
    channel = None
    room = None
    fis_active = request.POST.get('is_active')
    fslave_id = request.POST.get('slave_id')
    froom = request.POST.get('room')
    fchannel = request.POST.get('channel')
    fmodel = request.POST.get('model')
    falert_profile = request.POST.get('falert_profile')
    if falert_profile:
        alert_profile = AlertProfile.objects.filter(id=falert_profile).first()

    if not fmodel in ["OR-WE-504", "OR-WE-516"]:
        message_error(request, __TITLE__, f"Nieznany model licznika")
        return redirect('manage_meters')
    if fchannel:
        channel = Channel.objects.filter(id=fchannel).first()
    if not channel:
        message_error(request, __TITLE__, f"Nie wybrano kanału")
        return redirect('manage_meters')
    if froom:
        room = Room.objects.filter(id=froom).first()
    if not re.fullmatch(r'\d+', fslave_id):
        if not (fslave_id in range(1, 248)):
            message_error(request, __TITLE__, f"Nieprawidłowy adres Modbus.")
            return redirect('manage_meters')
    slave_id_int = int(fslave_id)

    occupied = ElectricMeter.objects.filter(channel=channel, slave_id=slave_id_int)
    for occ in occupied:
        if not occ.id == meter_id:
            message_error(request, __TITLE__, f"Adres Modbus {slave_id_int} jest już zajęty w tym kanale")
            return redirect('manage_meters')

    meter = ElectricMeter.objects.filter(id=meter_id).first()
    if not meter:
        message_error(request, __TITLE__, MESSAGE_STATUS.NOT_FOUND)
    meter.slave_id=fslave_id
    meter.model=fmodel
    meter.channel=channel
    meter.room=room
    meter.three_phases=True if fmodel in ['OR-WE-516'] else False
    meter.is_active = fis_active == 'on'
    meter.alert_profile = alert_profile
    meter.save()

    message_success(request, __TITLE__, MESSAGE_STATUS.SUCCESS_ADD)
    return redirect('manage_meters')

@login_required
def manage_delete_meter(request, meter_id):
    # -- -- Weryfikacja Uprawnień -- -- #
    if not is_admin(request):
        message_no_access(request, __TITLE__)
        return redirect('error_view')
    # -- -- Weryfikacja Żądania -- -- #
    if not is_method_post(request):
        message_bad_method(request, __TITLE__)
        return redirect(__REDIRECT__)
    meter = ElectricMeter.objects.filter(id=meter_id).first()
    if meter:
        meter.delete()
        message_success(request, __TITLE__, MESSAGE_STATUS.SUCCESS_DEL)
    else:
        message_error(request, __TITLE__, MESSAGE_STATUS.NOT_FOUND)
    return redirect('manage_meters')