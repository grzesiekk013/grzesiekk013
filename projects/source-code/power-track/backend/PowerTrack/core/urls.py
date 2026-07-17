from django.urls import path
from . import views
from django.conf import settings
from django.conf.urls.static import static

urlpatterns = [
    path('', views.home, name='home'),
    path('reports', views.reports, name='reports'),

    path('manage/users', views.manage_users, name="manage_users"),
    path('manage/users/add', views.manage_add_user, name="manage_add_user"),
    path('manage/users/edit/<int:user_id>', views.manage_edit_user, name="manage_edit_user"),
    path('manage/users/delete/<int:user_id>', views.manage_delete_user, name="manage_delete_user"),

    path('manage/clients', views.manage_clients, name="manage_clients"),
    path('manage/clients/add', views.manage_add_client, name="manage_add_client"),
    path('manage/clients/edit/<int:client_id>', views.manage_edit_client, name="manage_edit_client"),
    path('manage/clients/delete/<int:client_id>', views.manage_delete_client, name="manage_delete_client"),

    path('manage/switch_boards', views.manage_switch_boards, name="manage_switch_boards"),
    path('manage/switch_boards/add', views.manage_add_switch_board, name="manage_add_switch_board"),
    path('manage/switch_boards/edit/<int:board_id>', views.manage_edit_switch_board, name="manage_edit_switch_board"),
    path('manage/switch_boards/delete/<int:board_id>', views.manage_delete_switch_board, name="manage_delete_switch_board"),

    path('manage/devices', views.manage_devices, name="manage_devices"),
    path('manage/devices/add', views.manage_add_device, name="manage_add_device"),
    path('manage/devices/edit/<int:device_id>', views.manage_edit_device, name="manage_edit_device"),
    path('manage/devices/delete/<int:device_id>', views.manage_delete_device, name="manage_delete_device"),

    path('manage/rooms', views.manage_rooms, name="manage_rooms"),
    path('manage/rooms/add', views.manage_add_room, name="manage_add_room"),
    path('manage/rooms/edit/<int:room_id>', views.manage_edit_room, name="manage_edit_room"),
    path('manage/rooms/delete/<int:room_id>', views.manage_delete_room, name="manage_delete_room"),


    path('manage/meters', views.manage_meters, name="manage_meters"),
    path('manage/meters/add', views.manage_add_meter, name="manage_add_meter"),
    path('manage/meters/edit/<int:meter_id>', views.manage_edit_meter, name="manage_edit_meter"),
    path('manage/meters/delete/<int:meter_id>', views.manage_delete_meter, name="manage_delete_meter"),

    path('manage/alerts', views.manage_alerts, name="manage_alerts"),
    path('manage/alerts/add', views.manage_add_alert, name="manage_add_alert"),
    path('manage/alerts/edit/<int:alert_id>', views.manage_edit_alert, name="manage_edit_alert"),
    path('manage/alerts/delete/<int:alert_id>', views.manage_delete_alert, name="manage_delete_alert"),

    path('manage/events', views.manage_events, name="manage_events"),
    path('manage/access', views.manage_access, name="manage_access"),

    path('api/ping', views.api_ping, name='api_ping'),
    path('api/logged', views.api_logged, name='api_logged'),
    path('api/device/last_edit', views.api_get_device_last_edit, name='api_get_device_last_edit'),
    path('api/device/config', views.api_get_device_full_config, name='api_get_device_full_config'),
    path('api/device/send_alert', views.api_send_alert, name='api_send_alert'),
    path('api/device/send_measurements', views.api_send_measurements, name='api_send_measurements'),

    path('message', views.message_center, name="message_center"),
    path('message/chat/add', views.message_add_chat, name="message_add_chat"),
    path('message/chat/<int:chat_id>/view', views.messages_view_chat, name="messages_view_chat"),
    path('message/chat/<int:chat_id>/delete', views.messages_delete_chat, name="messages_delete_chat"),
    path('message/chat/<int:chat_id>/edit', views.message_edit_chat, name="message_edit_chat"),
    path('message/chat/message/delete/<int:message_id>', views.message_delete, name="message_delete"),
    path('message/chat/<int:chat_id>/message/add', views.message_add_message, name="message_add_message"),

    path('alerts', views.alerts_center, name="alerts_center"),
    path('alerts/<int:alert_id>/delete', views.delete_alert_center, name="delete_alert_center"),
    path('alerts/<int:alert_id>/show', views.show_alert_center, name="show_alert_center"),
    path('alerts/<int:alert_id>/confirm', views.confirm_alert_center, name="confirm_alert_center"),

    path('error_view', views.error_view, name="error_view"),

    path('account/login', views.user_login, name="user_login"),
    path('account/logout', views.logout, name="logout"),
    path('account/password_change', views.password_change, name="password_change"),
    path('account/details', views.account_details, name="account_details"),

    path('meters/list', views.my_meters_home, name="my_meters_home"),
    path('meters/<int:meter_id>', views.my_meters_details, name="my_meters_details"),
    path('meters/<int:meter_id>/chart_data', views.get_meter_chart_data, name="get_meter_chart_data"),

    path('reports/list', views.reports_list, name="reports_list"),
    path('reports/create', views.reports_create, name="reports_create"),
    path('reports/delete/<int:report_id>', views.reports_delete, name="reports_delete"),
    path('reports/download/<int:report_id>', views.reports_download, name="reports_download"),
]

if settings.DEBUG:
    urlpatterns += static(settings.MEDIA_URL, document_root=settings.MEDIA_ROOT)