from .base import *
from django.utils import timezone
"""
    System zgłoszeń
"""

__TITLE__: str = "Centrum Wiadomości"
__REDIRECT__: str = "message_center"

#
# def tickets_view(request):
#     role = get_role_name(request)
#
#     tickets_in_progress = None
#     tickets_ended = None
#
#     if role == "admin" and request.user.is_staff:
#         tickets_in_progress = Chat.objects.filter(
#             Q(status__in=["new","in_progress"])
#         )
#         tickets_ended = Chat.objects.filter(status__icontains="ended")
#     elif role == "admin" :
#         tickets_in_progress = Chat.objects.filter(
#             Q(status__in=["new","in_progress"]) &
#             Q(assigned_admin=request.user)
#         )
#         tickets_ended = Chat.objects.filter(
#             Q(status__icontains="ended") &
#             Q(assigned_admin=request.user)
#         )
#     elif role == "operator":
#         tickets_in_progress = Chat.objects.filter(
#             Q(status__in=["new", "in_progress"]) &
#             Q(assigned_operator=request.user)
#         )
#         tickets_ended = Chat.objects.filter(
#             Q(status__icontains="new") &
#             Q(assigned_operator=request.user)
#         )
#     elif role == "user":
#         tickets_in_progress = Chat.objects.filter(
#             Q(status__in=["new", "in_progress"]) &
#             Q(assigned_user=request.user)
#         )
#         tickets_ended = Chat.objects.filter(
#             Q(status__icontains="new") &
#             Q(assigned_user=request.user)
#         )
#         # -- -- filtrowanie -- --
#     fid = request.GET.get('fid')
#     fauthor = request.GET.get('fauthor')
#     ftitle = request.GET.get('ftitle')
#     fadmin = request.GET.get('fadmin')
#     foperator = request.GET.get('foperator')
#     fuser = request.GET.get('fuser')
#     fstatus = request.GET.get('fstatus')
#     fstatus = fstatus if fstatus in ["new", 'in_progress'] else None
#
#
#     if fid:
#         tickets_in_progress = tickets_in_progress.filter(id=fid)
#     if fauthor:
#         fauthor = fauthor.split(" ")
#         if len(fauthor) == 1:
#             tickets_in_progress = tickets_in_progress.filter(
#                 Q(assigned_author__first_name__icontains=fauthor[0]) |
#                 Q(assigned_author__last_name__icontains=fauthor[0])
#             )
#         if len(fauthor) == 2:
#             tickets_in_progress = tickets_in_progress.filter(
#                 Q(assigned_author__first_name__icontains=fauthor[0]) &
#                 Q(assigned_author__last_name__icontains=fauthor[1])
#             )
#         else:
#             pass
#
#     if ftitle:
#         tickets_in_progress = tickets_in_progress.filter(title__icontains=ftitle)
#     if fadmin:
#         fadmin = fadmin.split(" ")
#         if len(fadmin) == 1:
#             tickets_in_progress = tickets_in_progress.filter(
#                 Q(assigned_admin__first_name__icontains=fadmin[0]) |
#                 Q(assigned_admin__last_name__icontains=fadmin[0])
#             )
#         if len(fadmin) == 2:
#             tickets_in_progress = tickets_in_progress.filter(
#                 Q(assigned_admin__first_name__icontains=fadmin[0]) &
#                 Q(assigned_admin__last_name__icontains=fadmin[1])
#             )
#         else:
#             pass
#     if foperator:
#         foperator = foperator.split(" ")
#         if len(foperator) == 1:
#             tickets_in_progress = tickets_in_progress.filter(
#                 Q(assigned_operator__first_name__icontains=foperator[0]) |
#                 Q(assigned_operator__last_name__icontains=foperator[0])
#             )
#         if len(foperator) == 2:
#             tickets_in_progress = tickets_in_progress.filter(
#                 Q(assigned_operator__first_name__icontains=foperator[0]) &
#                 Q(assigned_operator__last_name__icontains=foperator[1])
#             )
#         else:
#             pass
#     if fuser:
#         fuser = foperator.split(" ")
#         if len(fuser) == 1:
#             tickets_in_progress = tickets_in_progress.filter(
#                 Q(assigned_user__first_name__icontains=fuser[0]) |
#                 Q(assigned_user__last_name__icontains=fuser[0])
#             )
#         if len(fauthor) == 2:
#             tickets_in_progress = tickets_in_progress.filter(
#                 Q(assigned_user__first_name__icontains=fuser[0]) &
#                 Q(assigned_user__last_name__icontains=fuser[1])
#             )
#         else:
#             pass
#     if fstatus:
#         tickets_in_progress = tickets_in_progress.filter(status__icontains=tickets_in_progress)
#
#     form_vals = {
#         'fid': fid or "",
#         'fauthor': fauthor or "",
#         'ftitle': ftitle or "",
#         'fadmin': fadmin or "",
#         'foperator': foperator or "",
#         'fuser': fuser or "",
#     }
#
#     return render(request, 'core/administrator/tickets_view.html',{'role': role, 'tickets_in_progress': tickets_in_progress, 'tickets_ended': tickets_ended, 'form_vals': form_vals})


"""
    Centrum Wiadomości
"""

@login_required
def message_center(request):
    # -- -- Weryfikacja Uprawnień -- -- #
    if not (is_admin_or_operator(request) or is_user(request)):
        message_no_access(request, __TITLE__)
        return redirect('error_view')
    # -- -- Weryfikacja Żądania -- -- #
    if not is_method_get(request):
        message_bad_method(request, __TITLE__)
        return redirect(__REDIRECT__)

    chats = Chat.objects.filter(
        Q(created_by = request.user) |
        Q(assigned_admin = request.user) |
        Q(assigned_operator = request.user) |
        Q(assigned_user = request.user)
    ).order_by("-last_msg_date")

    users = User.objects.all()
    fauthor = request.GET.get('fauthor')
    ftitle = request.GET.get('ftitle')
    fadmin = request.GET.get('fadmin')
    foperator = request.GET.get('foperator')
    fuser = request.GET.get('fuser')
    fstatus = request.GET.get('fstatus')
    fstatus = fstatus if fstatus in ["new", 'in_progress', 'ended'] else None

    admins = users.filter(userrole__role="admin")
    operators = users.filter(userrole__role="operator")
    authors = users
    standard_users = users.filter(userrole__role="user")

    if is_user(request):
        try:
            user_group = request.user.userrole.client
        except:
            user_group = None
        finally:
            pass

        if user_group:
            authors = authors.filter(userrole__client=user_group) | admins | operators | users.filter(id=request.user.id)
            standard_users = standard_users.filter(userrole__client=user_group)
        else:
            authors = admins | operators



    if fauthor:
        chats = chats.filter(assigned_author__id=int(fauthor))
    if ftitle:
        chats = chats.filter(title__icontains=ftitle)
    if fadmin:
        chats = chats.filter(assigned_admin__id=int(fadmin))
    if foperator:
        chats = chats.filter(assigned_operator__id=int(foperator))
    if fuser:
        chats = chats.filter(aassigned_user__id=int(fuser))
    if fstatus:
        chats = chats.filter(status__icontains=fstatus)

    form_vals = {
        'fauthor': fauthor or "",
        'ftitle': ftitle or "",
        'fadmin': fadmin or "",
        'foperator': foperator or "",
        'fuser': fuser or "",
        'fstatus': fstatus or ""
    }
    # -- -- Paginacja -- -- #
    paginator = Paginator(chats, 25)
    page_number = request.GET.get('page')
    page_object = paginator.get_page(page_number)
    params = request.GET.copy()
    if "page" in params:
        del params["page"]
    # , 'params': params.urlencode()
    # -- -- -- -- -- -- -- -- #

    return render(request, 'core/administrator/message_center.html', {'users': users, 'chats': page_object, 'form_vals': form_vals,
                                                          'admins': admins, 'authors': authors, 'operators': operators, 'standard_users': standard_users})

@login_required
def messages_delete_chat(request, chat_id):
    # -- -- Weryfikacja Uprawnień -- -- #
    if not (is_admin(request) or is_user(request)):
        message_no_access(request, __TITLE__)
        return redirect('error_view')
    # -- -- Weryfikacja Żądania -- -- #
    if not is_method_post(request):
        message_bad_method(request, __TITLE__)
        return redirect(__REDIRECT__)
    chat = Chat.objects.filter(id=chat_id).first()
    if chat:
        if chat.created_by == request.user or is_admin_only(request):
            chat.delete()
        else:
            message_error(request, __TITLE__, "Nie masz uprawnień do usunięcia tego czatu lub nie jesteś jego autorem.")
            return redirect(__REDIRECT__)
    else:
        message_error(request, __TITLE__, MESSAGE_STATUS.NOT_FOUND)
        return redirect(__REDIRECT__)
    message_success(request, __TITLE__, MESSAGE_STATUS.SUCCESS_DEL)
    return redirect(__REDIRECT__)

@login_required
def messages_view_chat(request, chat_id):
    # -- -- Weryfikacja Uprawnień -- -- #
    # Logic hidden for security and copyright protection reasons (engineering thesis).
    # Full source code is available to recruiters upon request.
    pass
    # -- -- Weryfikacja Żądania -- -- #
    # Logic hidden for security and copyright protection reasons (engineering thesis).
    # Full source code is available to recruiters upon request.
    pass

@login_required
def message_add_message(request, chat_id):
    # -- -- Weryfikacja Uprawnień -- -- #
   # Logic hidden for security and copyright protection reasons (engineering thesis).
    # Full source code is available to recruiters upon request.
    pass
    # -- -- Weryfikacja Żądania -- -- #
    # Logic hidden for security and copyright protection reasons (engineering thesis).
    # Full source code is available to recruiters upon request.
    pass

@login_required
def message_delete(request, message_id):
    # -- -- Weryfikacja Uprawnień -- -- #
    if not (is_admin_or_operator(request) or is_user(request)):
        message_no_access(request, __TITLE__)
        return redirect('error_view')
    # -- -- Weryfikacja Żądania -- -- #
    if not is_method_post(request):
        message_bad_method(request, __TITLE__)
        return redirect(__REDIRECT__)
    message = ChatMessage.objects.filter(id=message_id).first()
    if not (is_admin(request) or (request.user == message.created_by and is_operator(request))):
        message_error(request, __TITLE__, f" Nie masz dostępu do tego żądania.")
        return redirect(__REDIRECT__)
    if not message:
        message_error(request, __TITLE__, MESSAGE_STATUS.NOT_FOUND)
        return redirect(__REDIRECT__)
    try:
        message.delete()
        message_success(request, __TITLE__, MESSAGE_STATUS.SUCCESS_DEL)
        return redirect('messages_view_chat', chat_id=message.chat.id)
    except Exception as ex:
        message_error(request, __TITLE__, MESSAGE_STATUS.ERROR_DEL)
        return redirect(__REDIRECT__)

@login_required
def message_edit_chat(request, chat_id):
    # -- -- Weryfikacja Uprawnień -- -- #
    if not is_admin(request):
        message_no_access(request, __TITLE__)
        return redirect('error_view')
    # -- -- Weryfikacja Żądania -- -- #
    if not is_method_post(request):
        message_bad_method(request, __TITLE__)
        return redirect(__REDIRECT__)
    chat = Chat.objects.filter(id=chat_id).first()
    if not chat:
        message_error(request, __TITLE__, MESSAGE_STATUS.NOT_FOUND)
        return redirect(__REDIRECT__)

    fuser = request.POST.get('fuser')
    fadmin = request.POST.get('fadmin')
    foperator = request.POST.get('foperator')
    ftitle = request.POST.get('ftitle')
    fstatus = request.POST.get('fstatus')

    status = None
    user = None
    admin = None
    operator = None

    if fstatus and fstatus in ["new", "in_progress", "ended"]:
        status = fstatus

    if fuser:
        user = User.objects.filter(id=fuser).first()
    if fadmin:
        admin = User.objects.filter(id=fadmin).first()
    if foperator:
        operator = User.objects.filter(id=foperator).first()

    if not (user or admin or operator):
        message_error(request, __TITLE__, f" Nie wskazano odbiorcy wiadomości.")
        return redirect(__REDIRECT__)

    if not (ftitle):
        message_error(request, __TITLE__, f" Tytuł ani treść nie może być pusta.")
        return redirect(__REDIRECT__)

    if not len(ftitle) in range(8,65):
        message_error(request, __TITLE__, f" Tytuł musi zawierać od 8 do maks. 64 znaków.")
        return redirect(__REDIRECT__)
    # Logic hidden for security and copyright protection reasons (engineering thesis).
    # Full source code is available to recruiters upon request.
    pass

@login_required
def message_add_chat(request):
    # -- -- Weryfikacja Uprawnień -- -- #
    if not (is_admin_or_operator(request) or is_user(request)):
        message_no_access(request, __TITLE__)
        return redirect('error_view')
    # -- -- Weryfikacja Żądania -- -- #
    if not is_method_post(request):
        message_bad_method(request, __TITLE__)
        return redirect(__REDIRECT__)
    role = get_role_name(request)
    fuser = ""
    fadmin = ""
    foperator = ""
    fis_internal = ""

    ftitle = request.POST.get('ftitle')
    fdescription = request.POST.get('fdescription')

    user = None
    admin = None
    operator = None

    if role == "admin":
        fuser = request.POST.get('fuser')
        fadmin = request.POST.get('fadmin')
        foperator = request.POST.get('foperator')
        fis_internal = request.POST.get('fis_internal')
    elif role == "operator":
        fadmin = request.POST.get('fadmin')
        fis_internal = request.POST.get('fis_internal')
    elif role == "user":
        fadmin = request.POST.get('fadmin')
    else:
        message_error(request, __TITLE__, "Nieznane uprawnienia.")
        return redirect(__REDIRECT__)


    created_by = request.user

    if fuser:
        user = User.objects.filter(id=fuser).first()
    if fadmin:
        admin = User.objects.filter(id=fadmin).first()
    if foperator:
        operator = User.objects.filter(id=foperator).first()

    if not (user or admin or operator):
        message_error(request, __TITLE__, f" Nie wskazano odbiorcy wiadomości.")
        return redirect(__REDIRECT__)


    if not (ftitle and fdescription):
        message_error(request, __TITLE__, f" Tytuł ani treść nie może być pusta.")
        return redirect(__REDIRECT__)

    if not len(ftitle) in range(8,65):
        message_error(request, __TITLE__, f" Tytuł musi zawierać od 8 do maks. 64 znaków.")
        return redirect(__REDIRECT__)

    if not len(fdescription) in range(8,1024):
        message_error(request, __TITLE__, f" Tytuł musi zawierać od 8 do maks. 1024 znaków.")
        return redirect(__REDIRECT__)
    try:
        is_internal = bool(fis_internal)
    except:
        is_internal = False

    try:
        chat = Chat.objects.create(
            created_by=created_by,
            assigned_user=user,
            assigned_admin=admin,
            assigned_operator=operator,
            title=ftitle
        )
        if chat:
            ChatMessage.objects.create(
                chat=chat,
                created_by=created_by,
                message=fdescription,
                is_internal=is_internal
            )

        else:
            message_error(request, __TITLE__, MESSAGE_STATUS.ERROR_ADD)
            return redirect(__REDIRECT__)
        return redirect('messages_view_chat', chat_id = chat.id)
    except Exception as ex:
        message_error(request, __TITLE__,  f"Nieznany błąd {ex}")
        return redirect(__REDIRECT__)

