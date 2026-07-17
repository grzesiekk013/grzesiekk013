from .base import *

"""
    Zarządzanie urzadzeniami
"""

__TITLE__: str = "Urządzenia"
__REDIRECT__: str = "manage_devices"

@login_required
def manage_devices(request):
    # -- -- Weryfikacja Uprawnień -- -- #
    if not is_admin_or_operator(request):
        message_no_access(request, __TITLE__)
        return redirect('error_view')
    # -- -- Weryfikacja Żądania -- -- #
    if not is_method_get(request):
        message_bad_method(request, __TITLE__)
        return redirect(__REDIRECT__)
    role = get_role_name(request)
    devices = Device.objects.all()
    switch_boards = SwitchBoard.objects.all()

    # -- -- -- filtrowanie -- -- --
    fuuid = request.GET.get('fuuid')
    fswitch_board = request.GET.get('fswitch_board')
    fip = request.GET.get('fip')
    ftype = request.GET.get('ftype')
    fstatus = request.GET.get('fstatus')
    fdescription = request.GET.get('fdescription')
    fis_active = request.GET.get('fis_active')

    if fuuid:
        devices = devices.filter(uuid__icontains=fuuid)
    if fswitch_board:
        devices = devices.filter(switch_board__id=fswitch_board)
    if fip:
        devices = devices.filter(ip_address__icontains=fip)
    if ftype:
        devices = devices.filter(type__icontains=ftype)
    if fstatus:
        devices = devices.filter(status__icontains=fstatus)
    if fdescription:
        devices = devices.filter(description__icontains=fdescription)
    if fis_active:
        devices = devices.filter(is_active=(fis_active=="Tak"))
    form_vals = {
        'fuuid': fuuid or "",
        'fswitch_board': fswitch_board or "",
        'fip': fip or "",
        'ftype': ftype or "",
        'fstatus': fstatus or "",
        'fdescription': fdescription or "",
        'fis_active': fis_active or ""
    }
    # -- -- Paginacja -- -- #
    paginator = Paginator(devices, 25)
    page_number = request.GET.get('page')
    page_object = paginator.get_page(page_number)
    params = request.GET.copy()
    if "page" in params:
        del params["page"]
    # , 'params': params.urlencode()
    # -- -- -- -- -- -- -- -- #
    return render(request, 'core/administrator/manage_devices.html', {'devices': page_object, 'switch_boards': switch_boards, 'form_vals':form_vals, 'params': params.urlencode()})

# manage_add_device manage_edit_device manage_delete_device
@login_required
def manage_add_device(request):
    # -- -- Weryfikacja Uprawnień -- -- #
    if not is_admin_or_operator(request):
        message_no_access(request, __TITLE__)
        return redirect('error_view')
    # -- -- Weryfikacja Żądania -- -- #
    if not is_method_post(request):
        message_bad_method(request, __TITLE__)
        return redirect(__REDIRECT__)

    names = request.POST.getlist('names[]')
    uart = request.POST.getlist('uart[]')
    baud = request.POST.getlist('baud[]')
    parity = request.POST.getlist('parity[]')
    stopbits = request.POST.getlist('stopbits[]')
    datalen = request.POST.getlist('datalen[]')

    # - - - Sprawdzenie czy nazwy kanałów są unikalne - - - #
    if len(names) >= 2:
        clean_names = [n.strip() for n in names if n.strip()]
        if len(clean_names) != len(set(clean_names)):
            message_error(request, __TITLE__, "Nazwy kanałów nie mogą się powtarzać.")
            return redirect('manage_devices')

    def create_channels(device):
        for i in range(0, len(names)):
            if not (len(names[i]) in range(1,9) and len(uart[i]) in range(4,17) and int(baud[i]) in [9600, 19200, 34800, 57600, 115200] and
                parity[i] in ["N", "E", "O"] and stopbits[i] in ["1", "2"] and datalen[i] in ["7", "8"]):
                message_error(request, __TITLE__, "Nie udało się dodać kanału - nieprawidłowe wartości.")
                return redirect('manage_devices')

            Channel.objects.create(
                channel_name=names[i],
                uart=uart[i],
                baud_rate=int(baud[i]),
                parity=parity[i],
                stop_bits=int(stopbits[i]),
                data_len=int(datalen[i]),
                device=device
            )

    fuuid = request.POST.get('uuid')
    fdescription = request.POST.get('description')
    fswitch_board = SwitchBoard.objects.get(id=request.POST.get('switch_board'))

    if not fswitch_board:
        message_error(request, __TITLE__, "Nie odnaleziono rozdzielni.")
        return redirect('manage_devices')

    ftype = request.POST.get('type')
    finstalled_date = request.POST.get('installed_date')
    fis_active = request.POST.get('is_active')
    fpassword = request.POST.get('password')

    if not (fuuid or fdescription or ftype):
        message_error(request, __TITLE__, "Te pola nie mogą być puste: UUID, Opis, Typ..")
        return redirect('manage_devices')

    if not (len(fuuid) in range (2,33) and len(fdescription) in range(4, 129)):
        message_error(request, __TITLE__, "UUID powinno mieć długość 2-32, Opis 4-128.")
        return redirect('manage_devices')

    if not ftype in ["LP_MAX", "LP_LYRA", "SIM"]:
        message_error(request, __TITLE__, "Niepoprawny typ urządzenia.")
        return redirect('manage_devices')

    try:
        device = Device.objects.create(
            uuid=fuuid,
            description=fdescription,
            switch_board=fswitch_board,
            type=ftype,
            installed_date=finstalled_date,
            is_active=bool(fis_active),
            password=fpassword
        )

        if not device:
            message_error(request, __TITLE__, MESSAGE_STATUS.ERROR_ADD)
            return redirect('manage_devices')

        create_channels(device)

        polling_interval = request.POST.get('polling_interval')
        flushing_interval = request.POST.get('flushing_interval')
        if polling_interval:
            tmp = int(polling_interval)
            if tmp in range(30, 43201):
                device.polling_interval = tmp
        if flushing_interval:
            tmp = int(flushing_interval)
            if tmp in range(30, 43201):
                device.flushing_interval = tmp
        device.save()

    except Exception as ex:
        message_error(request, __TITLE__, f"Nieznany błąd {ex}.")
        return redirect('manage_devices')

    message_success(request, __TITLE__, MESSAGE_STATUS.SUCCESS_ADD)
    return redirect('manage_devices')

@login_required
def manage_edit_device(request, device_id):
    # -- -- Weryfikacja Uprawnień -- -- #
    # Logic hidden for security and copyright protection reasons (engineering thesis).
    # Full source code is available to recruiters upon request.
    pass
    # -- -- Weryfikacja Żądania -- -- #
    # Logic hidden for security and copyright protection reasons (engineering thesis).
    # Full source code is available to recruiters upon request.
    pass

        # - - - Sprawdzenie czy nazwy kanałów są unikalne - - - #
        # Logic hidden for security and copyright protection reasons (engineering thesis).
        # Full source code is available to recruiters upon request.
      # Logic hidden for security and copyright protection reasons (engineering thesis).
    # Full source code is available to recruiters upon request.
    pass

@login_required
def manage_delete_device(request, device_id):
    # -- -- Weryfikacja Uprawnień -- -- #
    if not is_admin(request):
        message_no_access(request, __TITLE__)
        return redirect('error_view')
    # -- -- Weryfikacja Żądania -- -- #
    if not is_method_post(request):
        message_bad_method(request, __TITLE__)
        return redirect(__REDIRECT__)

    device = Device.objects.filter(id=device_id).first()
    if not device:
        message_error(request, __TITLE__, MESSAGE_STATUS.NOT_FOUND)
        return redirect('manage_devices')

    device.delete()

    message_success(request, __TITLE__, MESSAGE_STATUS.SUCCESS_DEL)
    return redirect('manage_devices')