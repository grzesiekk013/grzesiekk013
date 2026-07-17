from datetime import timedelta
from turtledemo.penrose import start

from django.db.models.fields import return_None

from .base import *
from django.db.models import Count, Q
"""
    Moje liczniki
"""

__TITLE__: str = "Moje liczniki"
__REDIRECT__: str = "my_meters_home"

@login_required()
def my_meters_home(request):
    # -- -- Weryfikacja Uprawnień -- -- #
    if not (is_admin_or_operator(request) or is_user(request)):
        message_no_access(request, __TITLE__)
        return redirect('error_view')
    # -- -- Weryfikacja Żądania -- -- #
    if not is_method_get(request):
        message_bad_method(request, __TITLE__)
        return redirect(__REDIRECT__)

    meters = ElectricMeter.objects.all()
    meters_list = meters
    rooms = Room.objects.all()

    fmeter = request.GET.get("fmeter")
    froom = request.GET.get("froom")

    form_vals = {
        'fmeter': fmeter if fmeter else "",
        'froom': froom if froom else ""
    }

    if is_user(request):
        try:
            # user_group = request.user.userprofile.room_group
            user_group = request.user.userrole.client
        except:
            user_group = None
        finally:
            pass

        if user_group:
            meters = meters.filter(room__room_group = user_group)
            meters_list = meters
            rooms = rooms.filter(room_group = user_group)

        else:
            meters = meters.none()

    if fmeter:
        meters = meters.filter(id = int(fmeter))
    if froom:
        meters = meters.filter(room__id = int(froom))

    meters = meters.select_related('room', 'room__room_group')

    # -- -- Paginacja -- -- #
    paginator = Paginator(meters, 6)
    page_number = request.GET.get('page')
    page_object = paginator.get_page(page_number)
    params = request.GET.copy()
    if "page" in params:
        del params["page"]


    return render(request, 'core/all/my_meters.html', {'meters': page_object, 'params': params.urlencode(),
                                                       'meters_list': meters_list, 'rooms_list': rooms, 'form_vals': form_vals})

@login_required
def my_meters_details(request, meter_id):
    # -- -- Weryfikacja Uprawnień -- -- #
    # Logic hidden for security and copyright protection reasons (engineering thesis).
    # Full source code is available to recruiters upon request.
    pass
    # -- -- Weryfikacja Żądania -- -- #
    # Logic hidden for security and copyright protection reasons (engineering thesis).
    # Full source code is available to recruiters upon request.
    pass

def get_meter_chart_data(request, meter_id):
    # -- -- Weryfikacja Uprawnień -- -- #
    # Logic hidden for security and copyright protection reasons (engineering thesis).
    # Full source code is available to recruiters upon request.
    pass

