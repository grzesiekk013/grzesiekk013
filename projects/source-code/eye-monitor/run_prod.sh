cd eye
source .venv/bin/activate
cd django-app/python
nohup env PYTHONPATH=$(pwd) python -m uvicorn project.asgi:application --host 0.0.0.0 --port 8000 --workers 4 &
