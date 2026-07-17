from flask import Flask, request, jsonify

app = Flask(__name__)

@app.route('/action', methods=['POST'])
def action():
    data = request.get_json()

    if not data:
        return jsonify({"error": "Brak danych JSON"}), 400

    print("Odebrany JSON:")
    print(data)

    return jsonify({"message": "JSON odebrany pomyślnie", "received": data}), 200

if __name__ == '__main__':
    app.run(host='0.0.0.0', port=8000, debug=True)
