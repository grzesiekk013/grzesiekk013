from .base import *

"""
    Zarządzanie pomieszczeniami
"""

__TITLE__: str = "Urządzenia"
__REDIRECT__: str = "manage_devices"

@login_required
def manage_rooms(request):
    # -- -- Weryfikacja Uprawnień -- -- #
    if not is_admin_or_operator(request):
        message_no_access(request, __TITLE__)
        return redirect('error_view')
    # -- -- Weryfikacja Żądania -- -- #
    if not is_method_get(request):
        message_bad_method(request, __TITLE__)
        return redirect(__REDIRECT__)
    user_role = request.user.userrole

    rooms = Room.objects.all()
    clients = RoomGroup.objects.all()

    fname = request.GET.get('fname')
    fnumber = request.GET.get('fnumber')
    fclient = request.GET.get('fclient')

    alert_profiles = AlertProfile.objects.all()

    if fname:
        rooms = rooms.filter(name__icontains=fname)
    if fnumber:
        rooms = rooms.filter(number=fnumber)
    if fclient:
        rooms = rooms.filter(room_group__id=fclient)

    form_vals = {
        'fname': fname or "",
        'fnumber': fnumber or "",
        'fclient': fclient or "",
    }
    # -- -- Paginacja -- -- #
    paginator = Paginator(rooms, 25)
    page_number = request.GET.get('page')
    page_object = paginator.get_page(page_number)
    params = request.GET.copy()
    if "page" in params:
        del params["page"]
    # , 'params': params.urlencode()
    # -- -- -- -- -- -- -- -- #
    return render(request, 'core/administrator/manage_rooms.html', {'rooms': page_object, 'clients': clients, 'form_vals': form_vals, 'params': params.urlencode()})

@login_required
def manage_add_room(request):
    # -- -- Weryfikacja Uprawnień -- -- #
    if not is_admin_or_operator(request):
        message_no_access(request, __TITLE__)
        return redirect('error_view')
    # -- -- Weryfikacja Żądania -- -- #
    if not is_method_post(request):
        message_bad_method(request, __TITLE__)
        return redirect(__REDIRECT__)
    fname = request.POST.get('name')
    fnumber = request.POST.get('number')
    fclient = request.POST.get('client_id')
    fstart_lease_date = request.POST.get('start_lease_date')
    if fstart_lease_date == '':
        fstart_lease_date = None
    fend_lease_date = request.POST.get('end_lease_date')
    if fend_lease_date == '':
        fend_lease_date = None
    if not (len(fname) in range (2, 33) and len(fnumber) in range(2,17)):
        message_error(request, __TITLE__, f" Nazwa pomieszczenia powinna mieścić się od 2 do 32 znaków, a numer od 2 do 16 znaków.")
        return redirect('error_view')
    client = RoomGroup.objects.filter(id=fclient).first() if fclient else None
    room = Room.objects.create(
        name=fname,
        number=fnumber,
    )
    if room and client:
        room.room_group = client
    if fstart_lease_date:
        room.start_lease_date = fstart_lease_date
    if fend_lease_date:
        room.end_lease_date = fend_lease_date
    room.save()
    message_success(request, __TITLE__, MESSAGE_STATUS.SUCCESS_ADD)
    return redirect('manage_rooms')

@login_required
def manage_edit_room(request, room_id):
    # -- -- Weryfikacja Uprawnień -- -- #
    if not is_admin_or_operator(request):
        message_no_access(request, __TITLE__)
        return redirect('error_view')
    # -- -- Weryfikacja Żądania -- -- #
    if not is_method_post(request):
        message_bad_method(request, __TITLE__)
        return redirect(__REDIRECT__)
    room = Room.objects.filter(id=room_id).first()
    if not room:
        message_error(request, __TITLE__, MESSAGE_STATUS.NOT_FOUND)
        return redirect('error_view')

    fname = request.POST.get('name')
    fnumber = request.POST.get('number')
    fclient = request.POST.get('client_id')
    client = None
    if fclient:
        client = RoomGroup.objects.filter(id=fclient).first()
    fstart_lease_date = request.POST.get('start_lease_date')
    if fstart_lease_date == '':
        fstart_lease_date = None
    fend_lease_date = request.POST.get('end_lease_date')
    if fend_lease_date == '':
        fend_lease_date = None
    if not (len(fname) in range (2, 33) and len(fnumber) in range(2,17)):
        message_error(request, __TITLE__, f" Nazwa pomieszczenia powinna mieścić się od 2 do 32 znaków, a numer od 2 do 16 znaków.")
        return redirect('error_view')

    room.name=fname
    room.room_group = client
    room.number = fnumber
    room.start_lease_date = fstart_lease_date
    room.end_lease_date = fend_lease_date
    room.save()

    return redirect('manage_rooms')

@login_required
def manage_delete_room(request, room_id):
    # -- -- Weryfikacja Uprawnień -- -- #
    if not is_admin(request):
        message_no_access(request, __TITLE__)
        return redirect('error_view')
    # -- -- Weryfikacja Żądania -- -- #
    if not is_method_post(request):
        message_bad_method(request, __TITLE__)
        return redirect(__REDIRECT__)
    room = Room.objects.filter(id=room_id).first()
    if room:
        room.delete()
    return redirect('manage_rooms')