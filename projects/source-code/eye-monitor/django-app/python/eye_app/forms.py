from django import forms
from django.contrib.auth import get_user_model
from django.contrib.auth.forms import UserCreationForm
from .models import  Access, Room, RoomActions, RoomQualities, SensorDevice

User = get_user_model()

class DeviceAssignForm(forms.Form):
    device = forms.ModelChoiceField(
        queryset=SensorDevice.objects.all(),
        label="Wybierz urządzenie",
        widget=forms.Select(attrs={"class": "form-select"})
    )

class RoomQualitiesForm(forms.ModelForm):
    class Meta:
        model = RoomQualities
        exclude = ['room']
        widgets = {
            field.name: forms.NumberInput(attrs={"class": "form-control", "step": "any"})
            for field in RoomQualities._meta.fields if field.name != 'room'
        }

class RoomActionForm(forms.ModelForm):
    class Meta:
        model = RoomActions
        fields = ['parameter', 'custom_name', 'min', 'max', 'url']
        labels = {
            'parameter': 'Typ pomiaru',
            'custom_name': 'Nazwa (opcjonalna)',
            'min': 'Minimalna wartość',
            'max': 'Maksymalna wartość',
            'url': 'URL'
        }
        widgets = {
            'parameter': forms.Select(attrs={'class': 'form-select'}),
            'custom_name': forms.TextInput(attrs={'class': 'form-control', 'placeholder': 'np. Temperatura w sali C122'}),
            'min': forms.NumberInput(attrs={'class': 'form-control'}),
            'max': forms.NumberInput(attrs={'class': 'form-control'}),
            'url': forms.URLInput(attrs={'class': 'form-control'}),
        }

class AccessForm(forms.ModelForm):
    class Meta:
        model = Access
        fields = ['room', 'user', 'has_read_access', 'has_read_write_access']
        widgets = {
            'room': forms.HiddenInput(),
            'user': forms.HiddenInput(),
        }

class AddAccessForm(forms.ModelForm):
    user = forms.ModelChoiceField(queryset=User.objects.none(), label="User")

    class Meta:
        model = Access
        fields = ['user', 'has_read_access', 'has_read_write_access']
        widgets = {
            'has_read_access': forms.Select(choices=((1, 'True'), (0, 'False'))),
            'has_read_write_access': forms.Select(choices=((1, 'True'), (0, 'False'))),
        }
        labels = {
            'user': 'Użytkownik',
            'has_read_access': 'Odczyt',
            'has_read_write_access': 'Odczyt i Edycja',
        }


    def __init__(self, *args, **kwargs):
        room = kwargs.pop('room', None)
        super().__init__(*args, **kwargs)
        if room:
            existing_users = Access.objects.filter(room=room).values_list('user', flat=True)
            self.fields['user'].queryset = User.objects.exclude(id__in=existing_users)

class RoomForm(forms.ModelForm):
    class Meta:
        model = Room
        fields = ['device_id', 'room_number', 'floor', 'capacity', 'description']
        labels = {
            'device_id': 'Urządzenie (SensorDevice)',
            'room_number': 'Numer pokoju',
            'floor': 'Piętro',
            'capacity': 'Maksymalna liczba osób',
            'description': 'Opis pokoju',
        }
        widgets = {
            'device_id': forms.Select(attrs={'class': 'form-select'}),
            'room_number': forms.TextInput(attrs={
                'class': 'form-control',
                'placeholder': 'np. A101'
            }),
            'floor': forms.NumberInput(attrs={'class': 'form-control'}),
            'capacity': forms.NumberInput(attrs={'class': 'form-control'}),
            'description': forms.TextInput(attrs={
                'class': 'form-control',
                'placeholder': 'Opis pokoju'
            }),
        }

class CreateDeviceForm(forms.ModelForm):
    class Meta:
        model = SensorDevice
        fields = ['device_uuid', 'key', 'description']  # pomijamy `ip`, bo ma default

        labels = {
            'device_uuid': 'UUID',
            'key': 'Klucz',
            'description': 'Opis (opcjonalny)',
        }

        widgets = {
            'device_uuid': forms.TextInput(attrs={'class': 'form-control'}),
            'key': forms.TextInput(attrs={'class': 'form-control'}),
            'description': forms.TextInput(attrs={
                'class': 'form-control',
                'placeholder': 'Opis urządzenia'
            }),
        }