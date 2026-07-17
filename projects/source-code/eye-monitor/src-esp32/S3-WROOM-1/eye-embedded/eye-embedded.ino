#include <Arduino.h>
#include <thread>
#include "datatypes.h"
#include "flash-utils.h"
#include "network-utils.h"
#include "serial-utils.h"
#include "time-utils.h"
#include "sensor-utils.h"
#include "encryption-utils.h"


/* 
  5) DIODY SYGNALIZACJA PWM
  2) PRZETESTOWANIE WSZYSTKICH PRZYPADKOW WLACZNIE Z WYLACZENIEM ROUTERA ORAZ ZMIANIAMI POPRZEZ SERIAL
  0/4) PlatformIO + arduino ide libs 
  1) UUID - wymóg oraz długość 
  
*/

/* - VARIABLES - */

Preferences FLASH;

/* - - - */

String SSID = "";
String WIFI_PASSWORD = "";
String EYE_SERVER_IP = "";
String UUID = "";
String ENCRYPT_KEY = "";
bool IS_ENCRYPTED_KEY = false;
int EYE_SERVER_PORT = 0;
String MESSAGE = "";
String encrypted_json = "";
String decrypted_json = "";

/* - - - */

String API_URL = "api/post";
String API_ENCRYPTED_URL = "api/post_encrypted";
String API_PING_URL = "api/ping";
String API_TIME_URL = "api/time";

/* - - - */

String myIP = "";
String MAC_ADDRESS = "";
uint64_t  TIME_EPOCHMS = -1;
int WIFI_CONNECT_ATTEMPTS = 5;

/* - - - */

E_WIFI_STATUS WIFI_STATUS = WIFI_DISCONNECTED;
E_APP_STATUS APP_STATUS = APP_DISCONNECTED;
E_TIME_STATUS TIME_STATUS = TIME_NOT_SYNC;

/* - - - */

const int ENCRYPT_KEY_LEN = 32;
const int UUID_LEN = 64;
char SERIAL_BUFF[128];
int SERIAL_BUFF_IDX = 0;
bool LED_STATE = false;
bool USE_ENCRYPTION = false;
//Zmien wartość na odpowiednią, (43200 = 1 dznień ważny klucz)


/* - - - */
// //nowe
// abddabddabdxabxdabxdabddabddabzdagddabddabddabddabddabddabddabd
String HEX_KEY = "";
int hexadecimalKeyLength;
byte ENCRYPTED_BUFFER[512];
bool FIRST_TRY_AFTER_REEBOOT = true;


/* - - - */

/* - - - */
byte key2[];
char hexadecimalKeyBuffer[128];
// bool IsEncryptedKeyValid = false;

/* - - - */

const char* WIFI_STATUS_NAMES[3] = { "DISCONNECTED", "CONNECTING", "CONNECTED" };
const char* APP_STATUS_NAMES[4] = { "DISCONNECTED", "CONNECTED", "NOT_AVAILABLE", "UNABLE_TO_SEND_PACKET" };
const char* TIME_STATUS_NAMES[3] = { "NOT_SYNC", "SYNC", "UNABLE_TO_SYNC" };
const char* DEVICE_NAMES[4] = { "mlx90614", "dht11", "ens160", "sen22396"};

/* - - - */

void setup() {
  
  /* - SERIAL - */

  Serial.begin(BAUDRATE);

  /* - GPIO - */

  pinMode(D10, OUTPUT);
  pinMode(D11, OUTPUT);
  pinMode(D12, OUTPUT);
  pinMode(LED, OUTPUT);
  delay(500);

  /* - FLASH - */

  Serial.println("( WELCOME TO THE EYE_OS )");
  Serial.println("( BEGIN FLASH )");

  FLASH.begin("flash_memory", false);  // false = rw, true = r
  Serial.println("( LOAD FLASH )");
  load_flash();

  /* - WIFI - */

  read_mac();
  if (SSID != "") {
    Serial.println("( CONNECT TO WIFI )");
    connect_to_wifi();
  } else {
    Serial.println("( SSID IS EMPTY )");
  }

  /* - GPIO - */
  
  delay(2000);
  Serial.println("( EYE_OS STARTED )");
  read_ip();
  check_wifi_connection_status();
  print_wifi_status();
  Serial.println("( Type ? and press Enter for help )");

  /* - SENSORS - */
Serial.println("Init sensors...");
  init_sensors();
Serial.println("Init done.");
  update_sensors();
  reset_key_validation();
  xTaskCreatePinnedToCore(loop_core0, "Core0", 4096, NULL, 1, NULL, 0);
  xTaskCreatePinnedToCore(loop_core1, "Core1", 8192, NULL, 1, NULL, 1);

  /* - - - */
  setenv("TZ", "CET-1CEST,M3.5.0/2,M10.5.0/3", 1);
  tzset();

}

/* - WATCHDOG & HARDWARE */

void loop_core0(void *pvParameters) {
  unsigned long lastD10High = 0;
  unsigned long lastD10Low = 0;
  unsigned long lastLedToggle = 0;
  unsigned long lastD12Toggle = 0;
  unsigned long lastSensorUpdate = 0;
  unsigned long lastWifiCheck = 0;
  unsigned long lastReadIp = 0;
  unsigned long lastReconnect = 0;

  bool ledState = false;

  while (true) {
    unsigned long now = millis();
    /* - SERIAL - */
    
    serial_read();

    /* - LED - */

    if (now - lastD10High  >= 600)
    {
      digitalWrite(D10, HIGH);
      lastD10High = now;
    } 
    if (now - lastD10Low  >= 800) 
    {
      digitalWrite(D10, LOW);
      lastD10Low = now;
    }
    if (now - lastLedToggle >= 6400) {
      if (WIFI_STATUS == WIFI_DISCONNECTED || WIFI_STATUS == WIFI_CONNECTING) {
      LED_STATE = !LED_STATE;
      digitalWrite(LED, LED_STATE);
      } else if (WIFI_STATUS == WIFI_CONNECTED) {
        digitalWrite(LED, HIGH);
      }
      lastLedToggle = now;
    }

    if (now - lastD12Toggle >= 3200) {
    if (APP_STATUS == APP_DISCONNECTED) digitalWrite(D12, !digitalRead(D12));
      else if (APP_STATUS == APP_CONNECTED)  digitalWrite(D12, LOW);
      else { digitalWrite(D12, !digitalRead(D12));}
      lastD12Toggle = now;
    }

    /* - TIME - */

    if (TIME_STATUS == TIME_SYNC) {
      struct timeval tv;
      gettimeofday(&tv, NULL);
      TIME_EPOCHMS = (tv.tv_sec * 1000L) + (tv.tv_usec / 1000);
      
    }

    /* - SENSORS - */

     if (now - lastSensorUpdate >= 12800) {
      update_sensors();
      lastSensorUpdate = now;
    }

    /* - WIFI - */

     if (now - lastWifiCheck >= 10000) {
      check_wifi_connection_status();
      lastWifiCheck = now;
      // Serial.println(TIME_EPOCHMS);
    }

    if (now - lastReadIp >= 3200) {
      if (myIP == "" || myIP == "0.0.0.0") {
        read_ip();
      }
      lastReadIp = now;
    }
    
     if (now - lastReconnect >= 10000) {
      if (WIFI_STATUS == WIFI_DISCONNECTED) {
        reconnect_to_wifi();
        lastReconnect = now;
      }
    }


    /* - - - */
    
    vTaskDelay(pdMS_TO_TICKS(100));
  }
}

/* - NETWORK COMMUNICATION - */


void loop_core1(void *pvParameters) {
  unsigned long lastTime = 0;
  unsigned long interval = 1000;
  unsigned long timeOffset = millis();
  unsigned long lastTimeSyncAttempt = 0;
  unsigned long lastLedD11High = 0;
  unsigned long lastLedD11Low = 0;
  unsigned long lastSensorSend = 0;
  unsigned long lastKeyExpirationCheck = 0;
  unsigned long lastConnectionWithApp = 0;
  while (true) {

    /* - TIME - */
    unsigned long now = millis() - timeOffset;

    // if (now >= 100000){
    //   Serial.println("its" + String(now));
    //   timeOffset = millis();
    // }
    if (now - lastTimeSyncAttempt >= 400) {
      
      if (TIME_STATUS != TIME_SYNC && WIFI_STATUS == WIFI_CONNECTED) {
        uint64_t epoch_ms = get_server_time();
        Serial.println("( SYNCHRONIZED TIME "+String(epoch_ms)+" MS )");
        if (epoch_ms > 0) {
          struct timeval tv;
          tv.tv_sec = epoch_ms / 1000; 
          tv.tv_usec = (epoch_ms % 1000) * 1000;  
          settimeofday(&tv, NULL);
          TIME_STATUS = TIME_SYNC;
        }
      }
      lastTimeSyncAttempt = now;
    }

    /* - LED - */

     if (now - lastLedD11High >= 600) {
      digitalWrite(D11, HIGH);
      lastLedD11High = now;
    }
    if (now - lastLedD11Low >= 800) {
      digitalWrite(D11, LOW);
      lastLedD11Low = now;
    }

    if (now - lastConnectionWithApp >= 15000){
      check_server_connection();
      lastConnectionWithApp = now;
    }

    /* - SENSORS - */
  if (now - lastSensorSend >= 60000) {
      
      if (WIFI_STATUS == WIFI_CONNECTED && APP_STATUS == APP_CONNECTED && UUID.length() == 64) {
        if(FIRST_TRY_AFTER_REEBOOT){
          if(ENCRYPT_KEY.length() == 64){
          hex_key_after_reboot(ENCRYPT_KEY);
          FIRST_TRY_AFTER_REEBOOT = false;
          }
          else{
            Serial.println("( THERE IS NO KEY IN FLASH )");
            FIRST_TRY_AFTER_REEBOOT = false;
          }
        }
      else{
         if (!USE_ENCRYPTION){
          // Serial.println("( JSON WITHOUT ENCRYPTING )");
          send_json_post_request(API_URL,make_json_to_send("post",read_sensors_as_json()));
         }
         else 
        { 
          if (!IS_ENCRYPTED_KEY)
          {
          Serial.println("( YOU DON'T HAVE A KEY )");
          }
          
            String json_to_encrypt = make_json_encrypted_to_send(read_sensors_as_json());
            String encrypted_json_new = encrypt_json(json_to_encrypt);
            String pref = add_encrypt_prefix_json("post",encrypted_json_new);
            // Serial.println(pref);
            send_json_post_request(API_ENCRYPTED_URL,pref);
          
        }
      }
      }
      lastSensorSend = now;
  }
    /* - - - */
    vTaskDelay(pdMS_TO_TICKS(100));
    if (now > 100000) 
    {
    timeOffset = millis();
    }

    
  }
}

void loop() {
}
