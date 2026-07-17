#include <Arduino.h>
#include <thread>
#include "datatypes.h"
#include "flash_utils.h"
#include "network_utils.h"
#include "serial_utils.h"

/* - VARIABLES - */

Preferences FLASH;

String SSID = "";
String WIFI_PASSWORD = "";
String SERVER_IP = "";
String UUID = "";
int SERVER_PORT = 0;
String MESSAGE = "";

/* - - - */

String myIP = "";
String MAC_ADDRESS = "";
uint64_t  TIME_EPOCHMS = -1;
int WIFI_CONNECT_ATTEMPTS = 5;


/* - - - */

E_WIFI_STATUS WIFI_STATUS = WIFI_DISCONNECTED;
E_APP_STATUS APP_STATUS = APP_DISCONNECTED;

/* - - - */

const int UUID_LEN = 64;
char SERIAL_BUFF[128];
int SERIAL_BUFF_IDX = 0;
bool LED_STATE = false;
bool USE_ENCRYPTION = false;
bool FIRST_TRY_AFTER_REEBOOT = true;

/* - - - */ 

const char* WIFI_STATUS_NAMES[3] = { "DISCONNECTED", "CONNECTING", "CONNECTED" };
const char* APP_STATUS_NAMES[4] = { "DISCONNECTED", "CONNECTED", "NOT_AVAILABLE", "UNABLE_TO_SEND_PACKET" };


void setup() {
  /* - - SERIAL - - */

  Serial.begin(BAUDRATE_CFG);
  Serial1.begin(BAUDRATE_LIDAR);

  delay(1000);

  /* - - FLASH - - */

  Serial.println("( WELCOME TO AWB_OS )");
  Serial.println("( READING FLASH )");
  FLASH.begin("flash_memory", false); // false means RW, r is read only
  load_flash();
  Serial.println("( FLASH LOADED )");
  
  /* - - WIFI - - */
  Serial.println("( SETTING UP WIFI )");
  read_mac();
  if (SSID != "") {
    Serial.println("( CONNECTING TO WIFI )");
    connect_to_wifi();
  } else {
    Serial.println("( PLEASE PROVIDE SSID )");
  }

  delay(1000);

  Serial.println("( AWB_OS STARTED) ");
  read_ip();
  check_wifi_connection_status();
  print_wifi_status();
  Serial.println("( Type ? and press ENTER for help )");

  /* - - LIDAR - - */

  /* - - STEPPERS - - */

  /* - - THREADS - - */

  xTaskCreatePinnedToCore(loop_core0, "Core0", 4096, NULL, 1, NULL, 0);
  xTaskCreatePinnedToCore(loop_core1, "Core1", 8192, NULL, 1, NULL, 1);

  setenv("TZ", "CET-1CEST,M3.5.0/2,M10.5.0/3", 1);
  tzset();
}

void loop_core0(void *pvParameters) {
  while(true) {
    Serial.println("> TICK CORE 0");
    delay(1000);
  }
}

void loop_core1(void *pvParameters) {
  while(true) {
    Serial.println("> TICK CORE 1");
    delay("1500");
  }
}

void loop() {
  // put your main code here, to run repeatedly:

}
