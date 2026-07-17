#include <Preferences.h>
#include "esp_system.h"
#include <ESPping.h>
#include <time.h>

#ifndef DATATYPES_H
#define DATATYPES_H

#define LED LED_BUILTIN
#define BAUDRATE 115200

extern Preferences FLASH;
extern String SSID;
extern String WIFI_PASSWORD;

extern String EYE_SERVER_IP;
extern int EYE_SERVER_PORT;
// extern String NTP_SERVER_IP;
// extern int NTP_SERVER_PORT;

extern String UUID;
extern String ENCRYPT_KEY;
extern bool IS_ENCRYPTED_KEY;
extern String API_URL;
extern String API_ENCRYPTED_URL;
extern String API_PING_URL;
extern String API_TIME_URL;
extern String MESSAGE;
extern int WIFI_CONNECT_ATTEMPTS;
extern bool FIRST_TRY_AFTER_REEBOOT;
extern String myIP;
extern String MAC_ADDRESS;
extern uint64_t  TIME_EPOCHMS;

extern char SERIAL_BUFF[128];
extern int SERIAL_BUFF_IDX;

extern bool USE_ENCRYPTION;
extern String HEX_KEY;
enum E_WIFI_STATUS { 
  WIFI_DISCONNECTED = 0,
  WIFI_CONNECTING = 1,
  WIFI_CONNECTED = 2,
  // WIFI_IDLE = 3,
  // WIFI_NO_SSID_AVAILABLE = 4,
};

enum E_APP_STATUS {
  APP_DISCONNECTED = 0,
  APP_CONNECTED = 1,
  APP_NOT_AVAILABLE = 2,
  APP_UNABLE_TO_SEND_PACKET = 3
};

enum E_TIME_STATUS {
  TIME_NOT_SYNC = 0,
  TIME_SYNC = 1,
  TIME_UNABLE_TO_SYNC = 2
};

enum E_DEVICE_TYPE {
  DEV_MLX90614 = 0,
  DEV_DHT11 = 1,
  DEV_ENS160 = 2,
  DEV_SEN22396 = 3
};

enum E_COM_INTERFACE {
  COMM_SPI = 0,
  COMM_UART = 1,
  COMM_I2C = 2,
  COMM_ANALOG = 3
};

extern E_TIME_STATUS TIME_STATUS;
extern E_WIFI_STATUS WIFI_STATUS;
extern E_APP_STATUS APP_STATUS;

extern const char* WIFI_STATUS_NAMES[3];
extern const char* APP_STATUS_NAMES[4];
extern const char* TIME_STATUS_NAMES[3];
extern const char* DEVICE_NAMES[4];

extern const int ENCRYPT_KEY_LEN;
extern const int UUID_LEN;
#endif


