from enum import StrEnum

from .base import *

"""
    Zarządzanie rozdzielniami
"""

__TITLE__: str = "Rozdzielnie"
__REDIRECT__: str = "manage_switch_boards"

@login_required
def manage_switch_boards(request):
    # -- -- Weryfikacja Uprawnień -- -- #
    if not is_admin_or_operator(request):
        message_no_access(request, __TITLE__)
        return redirect('error_view')
    # -- -- Weryfikacja Żądania -- -- #
    if not is_method_get(request):
        message_bad_method(request, __TITLE__)
        return redirect(__REDIRECT__)
    switch_boards = SwitchBoard.objects.all()

    # -- -- -- filtrowanie -- -- --
    fname = request.GET.get('fname')
    flocation = request.GET.get('flocation')
    fdescription = request.GET.get('fdescription')

    if fname:
        switch_boards = switch_boards.filter(name__icontains=fname)
    if flocation:
        switch_boards = switch_boards.filter(location__icontains=flocation)
    if fdescription:
        switch_boards = switch_boards.filter(description__icontains=fdescription)


    form_vals = {
        'fname': fname or "",
        'flocation': flocation or "",
        'fdescription': fdescription or "",
    }
    # -- -- Paginacja -- -- #
    paginator = Paginator(switch_boards, 25)
    page_number = request.GET.get('page')
    page_object = paginator.get_page(page_number)
    params = request.GET.copy()
    if "page" in params:
        del params["page"]
    # , 'params': params.urlencode()
    # -- -- -- -- -- -- -- -- #
    return render(request, 'core/administrator/manage_switch_boards.html', {'switch_boards': page_object, 'form_vals': form_vals, 'params': params.urlencode()})

@login_required
def manage_add_switch_board(request):
    # -- -- Weryfikacja Uprawnień -- -- #
    if not is_admin_or_operator(request):
        message_no_access(request, __TITLE__)
        return redirect('error_view')
    # -- -- Weryfikacja Żądania -- -- #
    if not is_method_post(request):
        message_bad_method(request, __TITLE__)
        return redirect(__REDIRECT__)

    name = request.POST.get('name')
    location = request.POST.get('location')
    description = request.POST.get('description')

    if not name or not description or not location:
        message_error(request, __TITLE__,
                       f" Nazwa opis ani lokalizacja nie mogą być puste.")
        return redirect('manage_switch_boards')

    if not (len(name) in range(4, 65) and len(description) in range(4, 128) and len(location) in range(4, 65)):
        message_error(request, __TITLE__,
                       f" Nazwa, lokalizacja rozdzielni powinna mieć długość 4-64, a opis od 4 do 128 znaków.")
        return redirect('manage_switch_boards')

    tmp = SwitchBoard.objects.filter(name=name)
    if tmp:
        message_error(request, __TITLE__,
                       f" Nazwa jest już zajęta.")
        return redirect('manage_switch_boards')

    try:
        sb = SwitchBoard.objects.create(
            name=name,
            description=description,
            location=location,
            floor=0
        )
        if not sb:
            message_error(request, __TITLE__, MESSAGE_STATUS.ERROR_ADD)
        else:
            message_success(request, __TITLE__, MESSAGE_STATUS.SUCCESS_ADD)
        return redirect('manage_switch_boards')
    except Exception as ex:
        message_error(request, __TITLE__,f"Nieznany {ex}")
        return redirect('manage_switch_boards')

@login_required
def manage_edit_switch_board(request, board_id):
    # -- -- Weryfikacja Uprawnień -- -- #
    if not is_admin_or_operator(request):
        message_no_access(request, __TITLE__)
        return redirect('error_view')
    # -- -- Weryfikacja Żądania -- -- #
    if not is_method_post(request):
        message_bad_method(request, __TITLE__)
        return redirect(__REDIRECT__)
    name = request.POST.get('name')
    location = request.POST.get('location')
    description = request.POST.get('description')

    if not name or not description or not location:
        message_error(request, __TITLE__,
                       f" Nazwa opis ani lokalizacja nie mogą być puste.")
        return redirect('manage_switch_boards')

    if not (len(name) in range(4, 65) and len(description) in range(4, 128) and len(location) in range(4, 65)):
        message_error(request, __TITLE__,
                       f" Nazwa, lokalizacja rozdzielni powinna mieć długość 4-64, a opis od 4 do 128 znaków.")
        return redirect('manage_switch_boards')
    board = SwitchBoard.objects.filter(id=board_id).first()
    if not board:
        message_error(request, __TITLE__, MESSAGE_STATUS.NOT_FOUND)
        return redirect('manage_switch_boards')
    try:

        board.name = request.POST.get('name')
        board.description = request.POST.get('description')
        board.location = request.POST.get('location')
        board.save()
        message_success(request, __TITLE__, MESSAGE_STATUS.SUCCESS_EDIT)
        return redirect('manage_switch_boards')
    except Exception as ex:
        message_error(request, __TITLE__,f"Nieznany błąd {ex}")
        return redirect('manage_switch_boards')

@login_required
def manage_delete_switch_board(request, board_id):
    # -- -- Weryfikacja Uprawnień -- -- #
    if not is_admin(request):
        message_no_access(request, __TITLE__)
        return redirect('error_view')
    # -- -- Weryfikacja Żądania -- -- #
    if not is_method_post(request):
        message_bad_method(request, __TITLE__)
        return redirect(__REDIRECT__)
    board = SwitchBoard.objects.filter(id=board_id).first()

    if board:
        board.delete()
        message_success(request, __TITLE__, MESSAGE_STATUS.SUCCESS_DEL)
    else:
        message_error(request, __TITLE__, MESSAGE_STATUS.NOT_FOUND)

    return redirect('manage_switch_boards')