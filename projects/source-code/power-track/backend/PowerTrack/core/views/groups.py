from .base import *

"""
    Zarządzanie klientami
"""

__TITLE__: str = "Grupy"
__REDIRECT__: str = "manage_clients"

@login_required
def manage_clients(request):
    # -- -- Weryfikacja Uprawnień -- -- #
    if not is_admin_or_operator(request):
        message_no_access(request, __TITLE__)
        return redirect('error_view')
    # -- -- Weryfikacja Żądania -- -- #
    if not is_method_get(request):
        message_bad_method(request, __TITLE__)
        return redirect(__REDIRECT__)

    clients = RoomGroup.objects.all()

    # -- -- filtrowanie -- --
    fname = request.GET.get('fname')
    fdescription = request.GET.get('fdescription')


    if fname:
        clients = clients.filter(name__icontains=fname)
    if fdescription:
        clients = clients.filter(description__icontains=fdescription)

    form_vals = {
        'fname': fname or "",
        'fdescription': fdescription or "",
    }
    # -- -- Paginacja -- -- #
    paginator = Paginator(clients, 25)
    page_number = request.GET.get('page')
    page_object = paginator.get_page(page_number)
    params = request.GET.copy()
    if "page" in params:
        del params["page"]
    # , 'params': params.urlencode()
    # -- -- -- -- -- -- -- -- #
    return render(request, 'core/administrator/manage_clients.html', {'clients': page_object, 'form_vals': form_vals, 'params': params.urlencode()})

@login_required
def manage_add_client(request):
    # -- -- Weryfikacja Uprawnień -- -- #
    if not is_admin_or_operator(request):
        message_no_access(request, __TITLE__)
        return redirect('error_view')
    # -- -- Weryfikacja Żądania -- -- #
    if not is_method_post(request):
        message_bad_method(request, __TITLE__)
        return redirect(__REDIRECT__)

    name = request.POST.get('name')
    description = request.POST.get('description')

    if not name or not description:
        message_error(request, __TITLE__, "Nazwa ani opis grupy nie mogą być puste.")
        return redirect('error_view')

    if not (len(name) in range(4, 65) and len(description) in range(4, 128)):
        message_error(request, __TITLE__, "Nazwa grupy powinna mieć długość 4-64, a opis grupy od 4 do 128 znaków.")
        return redirect('error_view')

    tmp = RoomGroup.objects.filter(name=name)
    if tmp:
        message_error(request, __TITLE__ ,f"Nazwa grupy jest zajęta.")
        return redirect('error_view')


    group = RoomGroup.objects.create(
        name=name,
        description=description
    )
    if not group:
        message_error(request, __TITLE__, MESSAGE_STATUS.ERROR_ADD)
    message_success(request, __TITLE__, MESSAGE_STATUS.SUCCESS_ADD)
    return redirect('manage_clients')

@login_required
def manage_edit_client(request, client_id):
    # -- -- Weryfikacja Uprawnień -- -- #
    if not is_admin_or_operator(request):
        message_no_access(request, __TITLE__)
        return redirect('error_view')
    # -- -- Weryfikacja Żądania -- -- #
    if not is_method_post(request):
        message_bad_method(request, __TITLE__)
        return redirect(__REDIRECT__)

    name = request.POST.get('name')
    description = request.POST.get('description')

    if not name or not description:
        message_error(request, __TITLE__ ,"Nazwa ani opis grupy nie mogą być puste.")
        return redirect('error_view')

    if not (len(name) in range(4, 65) and len(description) in range(4, 128)):
        message_error(request, __TITLE__ ,"Nazwa grupy powinna mieć długość 4-64, a opis grupy od 4 do 128 znaków.")
        return redirect('error_view')

    tmp = RoomGroup.objects.filter(name=name)
    if tmp:
        message_error(request, __TITLE__ ,"Nazwa jest zajęta.")
        return redirect('error_view')

    client = RoomGroup.objects.filter(id=client_id).first()
    if not client:
        message_error(request, __TITLE__ ,MESSAGE_STATUS.NOT_FOUND)
        return redirect('error_view')


    client.name = name
    client.description = description
    client.save()
    message_success(request, __TITLE__, MESSAGE_STATUS.SUCCESS_EDIT)

    return redirect('manage_clients')

@login_required
def manage_delete_client(request, client_id):
    # -- -- Weryfikacja Uprawnień -- -- #
    if not is_admin(request):
        message_no_access(request, __TITLE__)
        return redirect('error_view')
    # -- -- Weryfikacja Żądania -- -- #
    if not is_method_post(request):
        message_bad_method(request, __TITLE__)
        return redirect(__REDIRECT__)
    client = RoomGroup.objects.filter(id=client_id).first()
    if client:
        client.delete()
        message_success(request, __TITLE__, MESSAGE_STATUS.SUCCESS_DEL)
    else:
        message_error(request, __TITLE__, MESSAGE_STATUS.NOT_FOUND)
    return redirect('manage_clients')