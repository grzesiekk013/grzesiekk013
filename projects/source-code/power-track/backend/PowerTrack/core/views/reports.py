import os.path
from django.http import FileResponse
from .base import *
import csv, io
from django.core.files.base import ContentFile
from ..models import MeterReport, ElectricMeter, Room, RoomGroup

"""
    Widoki odpowiadają za wyświetlanie listy,
    pobierania, tworzenia oraz usuwania raportów.
    Wbudowany mechanizm kontroli dostępu
"""

__TITLE__: str = "Raporty"
__REDIRECT__: str = "reports_list"

@login_required
def reports_list(request):
    # -- -- Weryfikacja Uprawnień -- -- #
    if not (is_admin_or_operator(request) or is_user(request)):
        message_no_access(request, __TITLE__)
        return redirect('error_view')
    # -- -- Weryfikacja Żądania -- -- #
    if not is_method_get(request):
        message_bad_method(request, __TITLE__)
        return redirect(__REDIRECT__)

    reports = MeterReport.objects.all().select_related('electric_meter', 'electric_meter__room')
    rooms = Room.objects.all()
    electric_meters = ElectricMeter.objects.all()
    clients = RoomGroup.objects.all()
    users = User.objects.filter(userrole__role__in=["admin", "operator"])
    if is_user(request):
        try:
            user_group = request.user.userrole.client
        except:
            user_group = None
        finally:
            pass
        if user_group:
            reports = reports.filter(electric_meter__room__room_group=user_group)
            rooms = rooms.filter(room_group=user_group)
            electric_meters = electric_meters.filter(room__room_group=user_group)
            clients = clients.filter(id=user_group.id)
        else:
            reports = []
            rooms = None
            electric_meters = None
            clients = None
    # -- -- -- filtrowanie - - -- --
    fmeter = request.GET.get('fmeter')
    froom = request.GET.get('froom')
    fdate_from = request.GET.get('fdate_from')
    fdate_to = request.GET.get('fdate_to')
    fclient = request.GET.get('fclient')
    fuser = request.GET.get('fuser')

    if fmeter:
        reports = reports.filter(electric_meter__id=fmeter)
    if froom:
        reports = reports.filter(electric_meter__room__id=froom)
    if fdate_from:
        reports = reports.filter(date_from=fdate_from)
    if fdate_to:
        reports = reports.filter(date_to=fdate_to)
    if fclient:
        reports = reports.filter(electric_meter__room__room_group__id=fclient)
    if fuser:
        reports = reports.filter(generated_by__id=fuser)
    form_vals = {
        'fmeter': fmeter or "",
        'froom': froom or "",
        'fdate_from': fdate_from or "",
        'fdate_to': fdate_to or "",
        'fclient': fclient or "",
    }
    # -- -- Paginacja -- -- #

    paginator = Paginator(reports, 25)
    page_number = request.GET.get('page')
    page_object = paginator.get_page(page_number)
    params = request.GET.copy()
    if "page" in params:
        del params["page"]
    # , 'params': params.urlencode()
    # -- -- -- -- -- -- -- -- #

    return render(request, 'core/all/reports.html', {'reports': page_object, 'form_vals': form_vals,
                                                     'clients': clients, 'rooms': rooms, 'meters': electric_meters, 'params': params.urlencode(),
                                                     'users': users})

"""
  path('reports/list', views.reports_list, name="reports_list"),
    path('reports/create', views.reports_create, name="reports_create"),
    path('reports/delete/<int:report_id>', views.reports_delete, name="reports_delete"),
    path('reports/download/<int:report_id>', views.reports_download, name="reports_download"),
"""
@login_required
def reports_create(request):
    # -- -- Weryfikacja Uprawnień -- -- #
    if not is_admin_or_operator(request):
        message_no_access(request, __TITLE__)
        return redirect(__REDIRECT__)
    # -- -- Weryfikacja Żądania -- -- #
    if not is_method_post(request):
        message_bad_method(request, __TITLE__)
        return redirect(__REDIRECT__)

    fmeter_id = request.POST.get('fmeter')
    fdate_from_s = request.POST.get('fdate_from')
    fdate_to_s = request.POST.get('fdate_to')

    if not all([fmeter_id, fdate_from_s, fdate_to_s]):
        message_error(request, __TITLE__, "Wszystkie pola są wymagane.")
        return redirect(__REDIRECT__)

    meter = ElectricMeter.objects.filter(id=int(fmeter_id)).first()
    if not meter:
        message_error(request, __TITLE__, "Nie odnaleziono licznika.")
        return redirect(__REDIRECT__)
    try:
        start_date = datetime.strptime(fdate_from_s, '%Y-%m-%d')
        end_date = datetime.strptime(fdate_to_s, '%Y-%m-%d').replace(hour=23, minute=59, second=59)
    except:
        message_error(request, __TITLE__, "Nieprawidłowy format daty")
        return redirect(__REDIRECT__)

    readings = MeterReading.objects.filter(
        electric_meter = meter,
        reading_time__range=(start_date, end_date)
    ). order_by('reading_time')

    if not readings.exists():
        message_error(request, __TITLE__, "Brak danych pomiarowych dla wybranego okresu")
        return redirect(__REDIRECT__)

    # -- -- Generowanie pliku CSV w pamięci -- -- #
    buff = io.StringIO()
    writer = csv.writer(buff, delimiter=';')

    writer.writerow(['Data i godzina', 'Moc czynna [W]', 'Energia czynna [kWh]', 'Energia bierna [WAR]'])

    for r in readings:
        writer.writerow(
            [
                r.reading_time.strftime('%Y-%m-%d %H:%M:%S'),
                str(r.total_active_power),
                str(r.total_active_energy),
                str(r.total_reactive_energy)
            ]
        )

    file_content = buff.getvalue().encode('utf-8-sig')
    file_name = f"Raport_{meter.id}_{fdate_from_s}_{fdate_to_s}.csv"

    report = MeterReport(
        electric_meter = meter,
        generated_by = request.user,
        date_from = start_date.date(),
        date_to = end_date.date()
    )

    report.report_file.save(file_name, ContentFile(file_content))
    report.save()
    message_success(request, __TITLE__, "Pomyślnie wykonano raport")
    return redirect(__REDIRECT__)
@login_required
def reports_delete(request, report_id):
    # -- -- Weryfikacja Uprawnień -- -- #
    if not is_admin(request):
        message_no_access(request, __TITLE__)
        return redirect(__REDIRECT__)
    # -- -- Weryfikacja Żądania -- -- #
    if not is_method_post(request):
        message_bad_method(request, __TITLE__)
        return redirect(__REDIRECT__)
    report = MeterReport.objects.filter(id=report_id).first()
    if not report:
        message_error(request, __TITLE__, "Nie odnaleziono")
        return redirect(__REDIRECT__)

    has_access = False
    if is_admin_or_operator(request):
        has_access = True
    if is_user(request):
        user_profile = getattr(request.user, 'userprofile', None)
        user_group = user_profile.room_group if user_profile else None
        if user_group and report.electric_meter.room and report.electric_meter.room.room_group == user_group:
            has_access = True

    if not has_access:
        message_error(request, __TITLE__, "Nie masz dostępu do tego zasobu")
        return redirect(__REDIRECT__)

    if report.report_file and os.path.isfile(report.report_file.path):
        os.remove(report.report_file.path)

    report.delete()
    message_success(request, __TITLE__, "Raport został usunięty.")
    return redirect(__REDIRECT__)
@login_required
def reports_download(request, report_id):
    # -- -- Weryfikacja Uprawnień -- -- #
    if not (is_admin_or_operator(request) or is_user(request)):
        message_no_access(request, __TITLE__)
        return redirect('error_view')

    has_access = False

    report = get_object_or_404(MeterReport, id=report_id)

    if is_admin_or_operator(request):
        has_access = True
    if is_user(request):
        user_role = getattr(request.user, 'userrole', None)
        user_group = user_role.client if user_role else None
        if user_group and report.electric_meter.room and report.electric_meter.room.room_group == user_group:
            has_access = True

    if not has_access:
        message_error(request, __TITLE__, "Nie masz uprawnień do tego zasobu.")
        return redirect(__REDIRECT__)

    response = FileResponse(open(report.report_file.path, 'rb'), content_type='text/csv')
    filename = os.path.basename(report.report_file.name)
    response['Content-Disposition'] = f'attachment; filename="{filename}"'
    return response