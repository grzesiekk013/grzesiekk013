#include "network-utils.h"
#include "datatypes.h"

uint64_t get_server_time() {
  if (check_server_connection()) {
    HTTPClient http;
    http.setTimeout(2000);
    String server_url = "http://" + EYE_SERVER_IP + ":" + String(EYE_SERVER_PORT) + "/" + API_TIME_URL;
    http.begin(server_url);

    int responseCode = http.GET();
    if (responseCode == 200) {
      String rensponseContext = http.getString();
      // Serial.println(rensponseContext);
      int start_idx = rensponseContext.indexOf("\"time_epoch_ms\":") + 16;
      if (start_idx == -1) return (uint64_t)0;
      int end_idx = rensponseContext.indexOf("}", start_idx);
      String epoch_str = rensponseContext.substring(start_idx, end_idx);
      http.end();
      // long epoch_ms = epoch_str.toLong();
      return atoll(epoch_str.c_str());
    }
    else {
      http.end();
      TIME_STATUS = TIME_UNABLE_TO_SYNC;
      return (uint64_t)0;
    }
    
  } else return (uint64_t)0;
}
bool check_server_connection() {

  if (ping_server()) {
    HTTPClient http;
    http.setTimeout(2000);
    String server_url = "http://" + EYE_SERVER_IP + ":" + String(EYE_SERVER_PORT) + "/" + API_PING_URL;
    // String server_url = "http://192.168.0.45:5000/api/ping";
    http.begin(server_url);
    http.addHeader("Content-Type", "application/json");
    int responseCode = http.GET();
    http.end();
    if (responseCode == 200) APP_STATUS = APP_CONNECTED;
    else APP_STATUS = APP_UNABLE_TO_SEND_PACKET;
    
    return (responseCode == 200);
  } else return false;
}
bool ping_server() {
  IPAddress server;
  if (!server.fromString(EYE_SERVER_IP)) {
    APP_STATUS = APP_NOT_AVAILABLE;

    return false;
  }
  return Ping.ping(server);
}

void send_json_post_request(String URL, String Body) {
  if (EYE_SERVER_IP == "" || EYE_SERVER_PORT < 1) return;
  if (!check_server_connection()) return;
  HTTPClient http;
  http.setTimeout(2000);
  String server_url = "http://" + EYE_SERVER_IP + ":" + String(EYE_SERVER_PORT) + "/" + URL;
  http.begin(server_url);
  http.addHeader("Content-Type", "application/json");

  int responseCode = http.POST(Body);

  if (responseCode > 0) {
    String response = http.getString();
    Serial.println("( HTTP " + String(responseCode) + " )");
    // Serial.print(response);
    // Serial.println(" >");
  } else {
    Serial.println("( HTTP " + String(responseCode) + " )");
    // Serial.print(responseCode);
    // Serial.println(" )");
  }

  http.end();
}
void read_ip() {
  myIP = String(WiFi.localIP().toString());
}
void read_mac() {
  uint64_t chipid = ESP.getEfuseMac();
  MAC_ADDRESS = "";
  for (int i = 0; i < 6; i++) {
    uint8_t macByte = (uint8_t)(chipid >> (8 * (5 - i)));
    if (macByte < 16) {
      MAC_ADDRESS += "0";
    }
    MAC_ADDRESS += String(macByte, HEX);
  }
}


void connect_to_wifi() {
  disconnect_wifi();
  WiFi.begin(SSID.c_str(), WIFI_PASSWORD.c_str());
  int attempt = 0;
  Serial.println("( CONNECTING )");
  while (WiFi.status() != WL_CONNECTED && attempt < WIFI_CONNECT_ATTEMPTS) {
    delay(500);
    Serial.print(".");
    attempt++;
    WIFI_STATUS = WIFI_CONNECTING;
  }
  Serial.print("\n");
  if (WiFi.status() == WL_CONNECTED) {
    read_ip();
    WIFI_STATUS = WIFI_CONNECTED;
    
  } else {
    WIFI_STATUS = WIFI_DISCONNECTED;
    read_ip();
  }
}

void check_wifi_connection_status() {
  if (WiFi.status() == WL_CONNECTED) {
    WIFI_STATUS = WIFI_CONNECTED;
  } else {
    WIFI_STATUS = WIFI_DISCONNECTED;
  }
}

void reconnect_to_wifi() {
  disconnect_wifi();
  WiFi.begin(SSID.c_str(), WIFI_PASSWORD.c_str());
  int attempt = 0;
  while (WiFi.status() != WL_CONNECTED && attempt < 2) {
    delay(500);
    attempt++;
    WIFI_STATUS = WIFI_CONNECTING;
  }
  if (WiFi.status() == WL_CONNECTED) {
    read_ip();
    WIFI_STATUS = WIFI_CONNECTED;
  } else {
    WIFI_STATUS = WIFI_DISCONNECTED;
  }
}

void disconnect_wifi() {
  myIP = "";
  WIFI_STATUS = WIFI_DISCONNECTED;
  WiFi.disconnect();
}