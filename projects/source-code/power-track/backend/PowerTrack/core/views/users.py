from .base import *


"""
    Zarządzanie użytkownikami
"""

__TITLE__: str = "Użytkownicy i Uprawnienia"
__REDIRECT__: str = "manage_users"

@login_required
def manage_users(request):
    # -- -- Weryfikacja Uprawnień -- -- #
    if not is_admin_only(request):
        message_no_access(request, __TITLE__)
        return redirect('error_view')
    # -- -- Weryfikacja Żądania -- -- #
    if not is_method_get(request):
        message_bad_method(request, __TITLE__)
        return redirect(__REDIRECT__)
    users = User.objects.all()

    # -- -- filtrowanie -- --
    username = request.GET.get('fusername')
    email = request.GET.get('femail')
    first_name = request.GET.get('ffirst_name')
    last_name = request.GET.get('flast_name')
    is_staff = request.GET.get('fis_staff')
    user_role = request.GET.get('frole')
    client = request.GET.get('fclient')
    fis_admin = request.GET.get('fis_admin')

    if username:
        users = users.filter(username__icontains=username)
    if email:
        users = users.filter(email__icontains=email)
    if first_name:
        users = users.filter(first_name__icontains=first_name)
    if last_name:
        users = users.filter(last_name__icontains=last_name)
    if is_staff:
        users = users.filter(is_staff__icontains=bool(is_staff))
    if user_role:
        users = users.filter(userrole__role__icontains=user_role)
    if client:
        users = users.filter(userrole__client__id=client)
    if fis_admin:
        users = users.filter(is_staff=(fis_admin == "Tak"))

    form_vals = {
        'fusername': username or "",
        'femail': email or "",
        'ffirst_name': first_name or "",
        'flast_name': last_name or "",
        'frole': user_role or "",
        'fclient': client or "",
        'fis_admin': fis_admin or ""
    }

    users_count = User.objects.filter(userrole__role='user').count()
    admins_count = User.objects.filter(userrole__role='admin').count()
    operators_count = User.objects.filter(userrole__role='operator').count()

    clients = RoomGroup.objects.all()

    # -- -- Paginacja -- -- #
    paginator = Paginator(users, 25)
    page_number = request.GET.get('page')
    page_object = paginator.get_page(page_number)
    params = request.GET.copy()
    if "page" in params:
        del params["page"]
    # , 'params': params.urlencode()
    # -- -- -- -- -- -- -- -- #

    return render(request, 'core/administrator/manage_users.html', {'users': page_object,
        'form_vals': form_vals, 'users_count': users_count, 'admins_count': admins_count, 'operators_count': operators_count, 'clients': clients, 'params': params.urlencode()})

@login_required
def manage_add_user(request):
    # -- -- Weryfikacja Uprawnień -- -- #
    if not is_admin_only(request):
        message_no_access(request, __TITLE__)
        return redirect('error_view')
    # -- -- Weryfikacja Żądania -- -- #
    if not is_method_post(request):
        message_bad_method(request, __TITLE__)
        return redirect(__REDIRECT__)
    client = None
    try:

        role = request.POST.get('role')
        if not role in ["admin", "user", "operator"]:
            role = "user"

        client_id = request.POST.get('client')
        if client_id:
            client = RoomGroup.objects.filter(id=client_id).first()
            message_error(request, __TITLE__, MESSAGE_STATUS.NOT_FOUND)
        username=request.POST.get('username')
        email=request.POST.get('email')
        fname=request.POST.get('name')
        lname=request.POST.get('surname')
        passwd=request.POST.get('passwd')

        if not fname or not lname or not email:
            message_error(request, __TITLE__,
                           f"Imię, nazwisko oraz email nie mogą być puste.")
            return redirect(__REDIRECT__)

        if not (len(fname) in range(4,65) and len(lname) in range(4,65) and len(email) in range(4,65)):
            message_error(request, __TITLE__,
                           f"Imię, nazwisko oraz email mają dozwoloną długość 4-64.")
            return redirect(__REDIRECT__)

        REGEX = r'^[a-zA-Z0-9@/\.+-_\s]+$'
        if not username or not re.match(REGEX, username):
            message_error(request, __TITLE__, f"Nazwa użytkownika może zawierać tylko litery, cyfry i znaki  @/./+/-/_..")
            return redirect(__REDIRECT__)
        tmp = User.objects.filter(username=username)
        if tmp or len(username) >= 150:
            message_error(request, __TITLE__, f"Niepoprawna nazwa użytkownika lub zajęta.")
            return redirect(__REDIRECT__)

        user = User.objects.create_user(
            username=username,
            email=email,
            first_name=fname,
            last_name=lname,
            password=passwd,
            is_staff=bool(request.POST.get('is_staff'))
        )
        if user:
            userrole = UserRole.objects.create(
                user=user,
                role=role,
            )
            if client:
                userrole.client = client
                userrole.save()
        else:
            message_error(request, __TITLE__, MESSAGE_STATUS.ERROR_ADD)
            return redirect(__REDIRECT__)
        return redirect('manage_users')
    except Exception as ex:
        message_error(request, __TITLE__, f"Nieznany błąd {ex}.")
        return redirect(__REDIRECT__)

@login_required
def manage_edit_user(request, user_id):
    # -- -- Weryfikacja Uprawnień -- -- #
    if not is_admin_only(request):
        message_no_access(request, __TITLE__)
        return redirect('error_view')
    # -- -- Weryfikacja Żądania -- -- #
    if not is_method_post(request):
        message_bad_method(request, __TITLE__)
        return redirect(__REDIRECT__)
    user = User.objects.filter(id=user_id).first()
    if not user:
        message_error(request, __TITLE__, MESSAGE_STATUS.NOT_FOUND)
    user_role = UserRole.objects.filter(user=user).first()
    if not user_role:
        message_error(request, __TITLE__, MESSAGE_STATUS.NOT_FOUND)
    role = request.POST.get('role')
    client = None
    client_id = request.POST.get('client')
    if client_id:
        client = RoomGroup.objects.filter(id=client_id).first()
        if not client:
            message_error(request, __TITLE__, MESSAGE_STATUS.NOT_FOUND)

    if not role in ["admin", "user", "operator"]:
        role = "user"
    username = request.POST.get('username')
    email = request.POST.get('email')
    fname = request.POST.get('name')
    lname = request.POST.get('surname')
    passwd = request.POST.get('passwd')

    if not fname or not lname or not email:
        message_error(request, __TITLE__,
                       f"Imię, nazwisko oraz email nie mogą być puste.")
        return redirect(__REDIRECT__)

    if not (len(fname) in range(4,65) and len(lname) in range(4,65) and len(email) in range(4,65)):
        message_error(request, __TITLE__,
                       f"Imię, nazwisko oraz email mają dozwoloną długość 4-64.")
        return redirect(__REDIRECT__)

    REGEX = r'^[a-zA-Z0-9@/\.+-_\s]+$'
    if not username or not re.match(REGEX, username):
        message_error(request, __TITLE__,
                       f"Nazwa użytkownika może zawierać tylko litery, cyfry i znaki  @/./+/-/_..")
        return redirect(__REDIRECT__)
    tmp = User.objects.filter(username=username).first()
    if len(username) >= 150:
        message_error(request, __TITLE__, f"Zbyt długa nazwa użytkownika.")
        return redirect(__REDIRECT__)
    if tmp:
        # nazwa zajęta
        if tmp != user:
            message_error(request, __TITLE__, f"Nazwa użytkownika jest już zajęta.")
            return redirect(__REDIRECT__)

    user.username = username
    user.email = email
    user.first_name = fname
    user.last_name = lname
    user.is_staff = bool(request.POST.get('is_staff'))
    if passwd:
        user.set_password(passwd)
    user.save()
    user_role.role = role
    user_role.client = client
    user_role.save()
    message_success(request, __TITLE__, MESSAGE_STATUS.SUCCESS_EDIT)
    return redirect('manage_users')

@login_required
def manage_delete_user(request, user_id):
    # -- -- Weryfikacja Uprawnień -- -- #
    if not is_admin_only(request):
        message_no_access(request, __TITLE__)
        return redirect('error_view')
    # -- -- Weryfikacja Żądania -- -- #
    if not is_method_post(request):
        message_bad_method(request, __TITLE__)
        return redirect(__REDIRECT__)
    user = User.objects.filter(id=user_id).first()
    if not user:
        message_error(request, __TITLE__, MESSAGE_STATUS.NOT_FOUND)
    if user_id != request.user.id:
        user.delete()
        message_success(request, __TITLE__, MESSAGE_STATUS.SUCCESS_DEL)
    else:
        message_error(request, __TITLE__, MESSAGE_STATUS.ERROR_DEL)
    return redirect('manage_users')