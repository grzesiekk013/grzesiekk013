import requests
import time
import math

URL = "http://10.0.13.219:8000/api/post"
DEV_UUID = "0000000000000000000000000000000000000000000000000000000000000001"
DEV_MAC = "00:11:22:33:44:55"

# Stałe do symulacji czasu
START_TIME_MS = int(time.time() * 1000)  # lub zastąp konkretną wartością w ms
STEP_MS = 30 * 60 * 1000  # 30 minut w milisekundach
TOTAL_ITERATIONS = 7 * 24 * 12  # 7 dni, co 30 minut = 336 iteracji

def generate_sensor_data(t):
    # sinusoidalne wartości dla różnych sensorów
    ens160 = {
        "name": "ens160",
        "readings": {
            "aqi": round(math.sin(0.1 * t) * 5 + 5, 2),
            "tvoc": round(math.sin(0.15 * t) * 100 + 150, 2),
            "eco2": round(math.sin(0.05 * t) * 200 + 600, 2)
        }
    }
    sen22396 = {
        "name": "sen22396",
        "readings": {
            "co2": round(math.sin(0.08 * t) * 20 + 420, 2),
            "temperature": round(math.sin(0.12 * t) * 2 + 21, 2),
            "humidity": round(math.sin(0.09 * t) * 3 + 45, 2)
        }
    }
    mlx90614 = {
        "name": "mlx90614",
        "readings": {
            "object_temperature": round(math.sin(0.12 * t) * 2 + 21, 2),
        }
    }
    return [ens160, sen22396, mlx90614]

def main():
    t = 0
    current_time_ms = START_TIME_MS

    for i in range(TOTAL_ITERATIONS):
        sensors = generate_sensor_data(t)
        payload = {
            "dev_uuid": DEV_UUID,
            "dev_mac": DEV_MAC,
            "dev_addr": "127.0.0.1",
            "time_epoch": current_time_ms,
            "command": "post",
            "data": {
                "sensors": sensors
            }
        }

        try:
            res = requests.post(URL, json=payload)
            print(f"[{t}] Sent JSON | Time: {current_time_ms} | Status: {res.status_code}")
        except Exception as e:
            print(f"Error: {e}")

        # Następna iteracja
        current_time_ms += STEP_MS
        t += 1
        time.sleep(0.1)  # Krótkie opóźnienie dla testów, zamiast 1s

    print("Zakończono generowanie danych dla 7 dni.")

if __name__ == "__main__":
    main()
