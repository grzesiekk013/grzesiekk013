#include "network_utils.h"
#include "datatypes.h"

bool check_server_connection() {
  return ping_server();
}

bool ping_server() {
  IPAddress server;
  if (!server.fromString(EYE_SERVER_IP)) {
    APP_STATUS = APP_NOT_AVAILABLE;

    return false;
  }
  return Ping.ping(server);
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