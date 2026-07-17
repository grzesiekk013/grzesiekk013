import base64
from django.utils.functional import SimpleLazyObject
from .models import Device

"""
    Zbiór metod odpowiada ze mechanizm autentykacji urządzeń, celem jest dalsze uproszczenie kodu i zachowanie standardu pisania w Django
"""

def get_device_from_auth_header(request):
    """
        Metoda sprawdza nagłówek Authorization base64(uuid, password)
        oraz próbuje uwierzytelnić urządzenie na podstawie ww. nagłówka
    :param request:
    :return: Brak lub instancja zalogowanego urządzenia
    """

    auth_header = request.headers.get('Authorization')

    if not auth_header:
        return None
    try:
        if auth_header.startswith('Basic '):
            encoded_uuid_passwd = auth_header.split(' ')[1]
            decoded_uuid_passwd = base64.b64decode(encoded_uuid_passwd).decode('utf-8')
            uuid, passwd = decoded_uuid_passwd.split(':', 1)

            device = Device.authenticate(uuid=uuid, password=passwd)
            if device and device.is_active:
                return device
    except:
        return None
    return None

class DeviceAuthmMiddleware:
    def __init__(self, get_response):
        self.get_response = get_response

    def __call__(self, request):
        """
            Metoda pozwala na używanie request.device zapisując do obiektu żądania request "luźny" link do Device
            Służy do bezstanowej autentykacji urządzeń
        :param request:
        :return:
        """
        # Logic hidden for security and copyright protection reasons (engineering thesis).
        # Full source code is available to recruiters upon request.
        pass
