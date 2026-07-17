from django.apps import AppConfig

class EyeAppConfig(AppConfig):
    default_auto_field = 'django.db.models.BigAutoField'
    name = 'eye_app'

    def ready(self):
        import eye_app.models  # <-- to uruchomi sygnał `post_save`