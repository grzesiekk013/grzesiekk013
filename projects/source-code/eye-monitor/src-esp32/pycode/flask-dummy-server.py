from flask import Flask, request
from Crypto.Cipher import AES
import time, json, csv, os

app = Flask(__name__)

CSV_FILE = "sensor_data_encrypt.csv"

@app.route('/api/post_encrypted', methods=['POST', 'GET'])
def handle_request():
    print(f"Request: {request.method} {request.url}")
    print(f"Headers:\n{dict(request.headers)}")

    body = request.data.decode('utf-8') if request.data else "{}"
    print(f"Raw Body:\n{body}")

    try:
        data = json.loads(body)

        if "encrypted" in data:
            print("=========================")
            print(data.get("encrypted"))
            print("=========================")
            hex_key = "abcdabcdabcdabcdabcdabcdabcdabcdabcdabcdabcdabcdabcdabcdabcdabcd"
            key = bytes.fromhex(hex_key)
            
            encrypted_hex = data.get("encrypted")

            encrypted_bytes = bytes.fromhex(encrypted_hex)

            cipher = AES.new(key, AES.MODE_ECB)
            decrypted = cipher.decrypt(encrypted_bytes)
            decrypted_json = make_json(decrypted)
            if decrypted_json:
                print("Odszyfrowany JSON:", decrypted_json)
                save_to_csv(decrypted_json)
                return "Odszyfrowane dane zapisane do CSV", 200
            else:
                return "Błąd podczas odszyfrowywania lub parsowania JSON", 400
            
            
        else:
            print(f"Parsed JSON:\n{data}") 
            save_to_csv(data)
            return "Data saved to CSV", 200
    except json.JSONDecodeError:
        return "Invalid JSON", 400
    return 'Otrzymano dane', 200

def make_json(body_bytes):
    try:
        body_str = unpad(body_bytes).decode('utf-8')
        return json.loads(body_str)
    except (json.JSONDecodeError, UnicodeDecodeError, IndexError):
        return ""

def unpad(padded_data: bytes) -> bytes:
    pad_len = padded_data[-1]  
    return padded_data[:-pad_len]

# @app.route('/api/post', methods=['POST'])
# def handle_request():
#     body = request.data
#     body_str = body.decode('utf-8')
#     print("Otrzymana zaszyfrowana wiadomość:")
#     print(body_str)
#     print("=========================")
#     print("RAW body (bytes):", body)
#     print("=========================")
#     print("RAW body (utf-8):", body.decode('utf-8'))
#     print("=========================")
#     try:
#         data = json.loads(body_str)
#         if "encrypted" in data:
#             print("=========================")
#             print(data.get("encrypted"))
#             print("=========================")
#     except json.JSONDecodeError:
#         return "Invalid JSON", 400
#     return 'Otrzymano dane', 200




def save_to_csv(data):
    row = {
        "dev_uuid": data.get("dev_uuid"),
        "dev_addr": data.get("dev_addr"),
        "dev_mac": data.get("dev_mac"),
        "time_epoch": data.get("time_epoch"),
        "command": data.get("command")
    }

    for sensor in data.get("data", {}).get("sensors", []):
        name = sensor.get("name")
        readings = sensor.get("readings", {})
        for key, value in readings.items():
            row[f"{name}_{key}"] = value

    fieldnames = list(row.keys())
    if os.path.exists(CSV_FILE) and os.path.getsize(CSV_FILE) > 0:
        with open(CSV_FILE, newline="") as file:
            reader = csv.DictReader(file)
            existing_fields = reader.fieldnames or []
            for f in existing_fields:
                if f not in fieldnames:
                    fieldnames.append(f)

    with open(CSV_FILE, mode="a", newline="") as file:
        writer = csv.DictWriter(file, fieldnames=fieldnames)

        if file.tell() == 0:
            writer.writeheader()

        writer.writerow(row)

@app.route('/api/ping', methods=['GET', 'POST', 'PUT', 'DELETE', 'PATCH'])
def ping():
    print()
    print(f"Request: {request.method} {request.url}")
    print(f"Headers:\n{dict(request.headers)}")

    body = request.data.decode('utf-8') if request.data else "No body received"
    print(f"Raw Body:\n{body}")

    return "", 200 

@app.route('/api/time', methods=['GET'])
def get_time():
    epoch_ms = int(time.time() * 1000)  
    print( f"{{\"time_epoch_ms\":{str(epoch_ms)}}}")
    return f"{{\"time_epoch_ms\":{str(epoch_ms)}}}", 200

if __name__ == "__main__":
    app.run(host="0.0.0.0", port=5000, debug=True)
