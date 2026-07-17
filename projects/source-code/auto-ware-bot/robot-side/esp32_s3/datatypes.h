#include <Preferences.h>
#include "esp_system.h"
#include <ESPping.h>
#include <time.h>

#ifndef DATATYPES_H
#define DATATYPES_H

#define LED LED_BUILTIN
#define BAUDRATE_CFG 115200
#define BAUDRATE_LIDAR 115200

extern Preferences FLASH;
extern String SSID;
extern String WIFI_PASSWORD;

extern String SERVER_IP;
extern int SERVER_PORT;
// extern String NTP_SERVER_IP;
// extern int NTP_SERVER_PORT;

extern String UUID;
extern String MESSAGE;
extern int WIFI_CONNECT_ATTEMPTS;
extern bool FIRST_TRY_AFTER_REEBOOT;
extern String myIP;
extern String MAC_ADDRESS;
extern uint64_t  TIME_EPOCHMS;

extern char SERIAL_BUFF[128];
extern int SERIAL_BUFF_IDX;

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

extern E_WIFI_STATUS WIFI_STATUS;
extern E_APP_STATUS APP_STATUS;

extern const char* WIFI_STATUS_NAMES[3];
extern const char* APP_STATUS_NAMES[4];

extern const int UUID_LEN;
#endif

