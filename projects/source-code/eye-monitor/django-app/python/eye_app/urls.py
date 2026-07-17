from django.urls import path
from . import views
from django.contrib.auth import views as auth_views
from .views import CustomLogoutView

urlpatterns = [
    path('', views.main, name='main'),
    path('create-room', views.create_room, name='create_room'),
    path('room-actions/<int:room_id>/', views.room_actions, name='room_actions'),
    path('room-advanced/<int:room_id>/', views.room_advanced, name='room_advanced'),
    path('room-details/<int:room_id>/', views.room_details, name='room_details'),
    path('room-device/<int:room_id>/', views.room_device, name='room_device'),
    path('room-edit/<int:room_id>/', views.room_edit, name='room_edit'), #unused
    path('room-delete-access/<int:room_id>/<int:access_id>', views.room_delete_access, name='room_delete_access'),
    # path('room-add-access/<int:room_id>/', views.room_add_access, name='room_add_access'),
    path('room-manage-access/<int:room_id>/', views.room_manage_access, name='room_manage_access'),
    path('room-names/<int:room_id>/', views.room_names, name='room_names'),
    path('room-overall/<int:room_id>/', views.room_overall, name='room_overall'),
    path('manage-accounts/', views.manage_accounts, name='manage_accounts'),
    path('manage-devices/', views.manage_devices, name='manage_devices'),
    path('edit-device/<int:sensordevice_id>/', views.edit_device, name='edit_device'),
    path('delete-device/<int:sensordevice_id>/', views.delete_device, name='delete_device'),
    path('room-variables-visibility/<int:room_id>/', views.room_variables_visibility, name='room_variables_visibility'),
    path('account/add/', views.add_account, name='add_account'),
    path('account/edit/<int:user_id>/', views.edit_account, name='edit_account'),
    path('account/delete/<int:user_id>/', views.delete_account, name='delete_account'),
    path('login/', auth_views.LoginView.as_view(template_name='eye_app/login.html'), name='login'),
    path('register/', views.register, name='register'),
    path('logout/', CustomLogoutView.as_view(), name='logout'),
    path('konto/', views.account_profile, name='account_profile'),
    path('api/post', views.api_post, name='api_post'),
    path('api/post_encrypted', views.api_post_encrypted, name='api_post_encrypted'),
    path('api/ping', views.api_ping, name='api_ping'),
    path('api/time', views.api_time, name='api_time'),
    path('room-actions/<int:room_id>/delete/<int:action_id>/', views.delete_action, name='delete_action'),
    path('room-actions/<int:room_id>/edit/<int:action_id>/', views.edit_action, name='edit_action'),
    path('room-delete/<int:room_id>/', views.room_delete, name='room_delete'),
]