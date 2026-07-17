import requests
import time
import math
import json
from Crypto.Cipher import AES
from Crypto.Util.Padding import pad

URL = "http://10.0.13.219:8000/api/post_encrypted"
DEV_UUID = "0000000000000000000000000000000000000000000000000000000000000002"
DEV_MAC = "00:11:22:33:44:55"
DEV_ADDR = "10.0.13.215"

AES_KEY = "0000000000000000000000000000000000000000000000000000000000000002"  # 64 znaki = 32 bajty

START_TIME_MS = int(time.time() * 1000)
STEP_MS = 30 * 60 * 1000
TOTAL_ITERATIONS = 7 * 24 * 12

def generate_sensor_data(t):
    return [
        {
            "name": "ens160",
            "readings": {
                "aqi": round((math.sin(0.1 * t) + 1) * 2 + 1, 2),
                "tvoc": round(math.sin(0.15 * t) * 100 + 150, 2),
                "eco2": round(math.sin(0.05 * t) * 200 + 600, 2)
            }
        },
        {
            "name": "sen22396",
            "readings": {
                "co2": round(math.sin(0.08 * t) * 20 + 420, 2),
                "temperature": round(math.sin(0.12 * t) * 2 + 21, 2),
                "humidity": round(math.sin(0.09 * t) * 3 + 45, 2)
            }
        },
        {
            "name": "mlx90614",
            "readings": {
                "object_temperature": round(math.sin(0.12 * t) * 2 + 21, 2),
            }
        }
    ]

def encrypt_json_ecb(payload: dict, key: bytes) -> str:
    cipher = AES.new(key, AES.MODE_ECB)
    plaintext = json.dumps(payload).encode("utf-8")
    ciphertext = cipher.encrypt(pad(plaintext, AES.block_size))
    return ciphertext.hex()

def main():
    t = 0
    current_time_ms = START_TIME_MS

    for i in range(TOTAL_ITERATIONS):
        sensors = generate_sensor_data(t)
        raw_payload = {
            "dev_mac": DEV_MAC,
            "data": {
                "sensors": sensors
            }
        }
        key = bytes.fromhex(AES_KEY)
        encrypted_payload = encrypt_json_ecb(raw_payload, key)

        final_payload = {
            "dev_uuid": DEV_UUID,
            "time_epoch": current_time_ms,
            "dev_addr": DEV_ADDR,
            "command": "post",
            "encrypted": encrypted_payload
        }

        try:
            res = requests.post(URL, json=final_payload)
            print(f"[{t}] Sent encrypted JSON (ECB) | Time: {current_time_ms} | Status: {res.status_code}")
        except Exception as e:
            print(f"Error: {e}")

        current_time_ms += STEP_MS
        t += 1
        time.sleep(1)

    print("Zakończono generowanie danych (ECB).")

if __name__ == "__main__":
    main()
