
from .base import *
from django.contrib.auth import authenticate, update_session_auth_hash
from django.contrib.auth import login as auth_login
from django.contrib.auth import logout as auth_logout

"""
    Plik przechowuje metody, odpowiadające za mechanizm logowania się, zmiany hasła oraz wylogowania
"""

def user_login(request):
    """
        Metoda odpowiada za logowanie się użytkownika przy wykorzystaniu metody login z django.contrib.auth
        Posiada wbudowany mechanizm walidacji formularza - pustych, zbyt długich pól jak i sprawdzenie poprawności logowania
        W przypadku gdy w pasku wyszukiwanai znajduje się parametr get "next" to przekierowuje na ten zasób sieciowy.

        Metoda POST do logowania oraz GET do wyświetlenia zawartości strony.
    :param request:
    :return:
    """

    if request.user.is_authenticated:
        return redirect('home')

    if is_method_post(request):
        username = request.POST.get('username')
        passwd = request.POST.get('password')

        if not (username and passwd):
            message_error(request, "Autentykacja", "Brak danych logowania.")
            return redirect('user_login')

        if len(username) > 150 or len(passwd) > 150:
            message_error(request, "Autentykacja", "Brak danych logowania.")
            return redirect('user_login')

        if "@" in username:
            _usr = User.objects.filter(email=username).first()
            if not _usr:
                message_error(request, "Autentykacja", "Niepoprawne dane logowania.")
                return redirect('user_login')
            username = _usr.username

        user = authenticate(request, username=username, password=passwd)

        if user is not None:
            auth_login(request, user)
            next_page = request.GET.get('next')
            message_success(request, 'Autentykacja', "Zalogowano pomyślnie.")
            if next_page:
                return redirect(next_page)
            else:
                return redirect('home')
        else:
            message_error(request, "Autentykacja", "Niepoprawne dane logowania.")
            return redirect('user_login')
    return render(request, 'core/all/login.html')

@login_required
def logout(request):
    """
        Metoda odpowiada za wylogowanie użytkownika
    :param request:
    :return:
    """
    auth_logout(request)
    message_success(request, 'Autentykacja', "Wylogowano pomyślnie.")
    return redirect('user_login')

@login_required
def password_change(request):
    """
        Metoda odpowiada za zmianę hasła obecnego użytkownika, w tym walidacja formularza
    :param request:
    :return:
    """
    if not is_method_post(request):
        message_error(request, "Autentykacja", "Niepoprawne żądanie.")
        return redirect(request.META.get('HTTP_REFERER'), 'home')
    if not request.user.is_authenticated:
        message_error(request, "Autentykacja", "Brak autoryzacji.")
        return redirect('user_login')

    passwd = request.POST.get('passwd')
    passwd_confirm = request.POST.get('passwd_confirm')
    passwd_old = request.POST.get('passwd_old')

    if not request.user.check_password(passwd_old):
        message_error(request, "Autentykacja", "Podano nieprawidłowe hasło.")
        return redirect(request.META.get('HTTP_REFERER'), 'home')

    if not (passwd and passwd_confirm):
        message_error(request, "Autentykacja", "Hasło nie może być puste.")
        return redirect(request.META.get('HTTP_REFERER'), 'home')

    if passwd != passwd_confirm:
        message_error(request, "Autentykacja", "Hasła różnią się od siebie.")
        return redirect(request.META.get('HTTP_REFERER'), 'home')

    if len(passwd) < 8 or len(passwd) > 128:
        message_error(request, "Autentykacja", "Hasło nie może być za krótkie ani za długie.")
        return redirect(request.META.get('HTTP_REFERER'), 'home')

    request.user.set_password(passwd)
    request.user.save()
    update_session_auth_hash(request, request.user)

    message_success(request, "Autentykacja", "Pomyślnie zmieniono hasło.")
    return redirect(request.META.get('HTTP_REFERER'), 'home')

@login_required
def account_details(request):
    if not is_method_get(request):
        message_error(request, "Szczegóły konta", "Niepoprawne żądanie.")
        return redirect(request.META.get('HTTP_REFERER'), 'home')
    return render(request, 'core/all/details.html')