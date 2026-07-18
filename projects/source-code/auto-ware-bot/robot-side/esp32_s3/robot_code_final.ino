#include <thread> 
#include <WiFi.h>
#include <WebSocketsClient.h>
#include <Wire.h>
#include <Adafruit_HMC5883_U.h>
#define LIDAR_SERIAL Serial1  
#define MMC5883L_ADDR 0x36

enum ROBOT_STATE {
  INIT,
  NO_WIFI,
  NO_CONNECTION_TO_SERVER,
  IS_THERE_ANY_INSTRUCTION,
  SENDING_LIDAR_DATA,
  GET_INSTRUCTION,
  PROCEEDING_INSTRUCTION,
  PROCEEDING_INSTRUCTION_DONE,
  SENDING_DONE,
};
uint8_t WS_CNT = 0;

volatile bool IS_WAITING_FOR_MESSSAGE = false;

bool LOCK_LIDAR = false;
bool PROCESSING_CALLBACK = false;

volatile ROBOT_STATE STATE = ROBOT_STATE::INIT;
ROBOT_STATE STATE_MEMORY = ROBOT_STATE::IS_THERE_ANY_INSTRUCTION;
String ROBOT_STATE_NAMES[] = {"INIT", "NO_WIFI", "NO_CONNECTION_TO_SERVER", "IS_THERE_ANY_INSTRUCTION", "SENDING_LIDAR_DATA", "GET_INSTRUCTION","PROCEEDING_INSTRUCTION", "PROCEEDING_INSTRUCTION_DONE", "SENDING_DONE"};

Adafruit_HMC5883_Unified mag = Adafruit_HMC5883_Unified(12345);

//STEP MOTOR 1 PIN
const int STEP_PIN = 13;
const int DIR_PIN = 12;
const int EN_PIN = 4;

//STEP MOTOR 2 PIN
const int STEP_PIN1 = 8;
const int DIR_PIN1 = 6;
const int EN_PIN1 = 5;

//WIFI DATA
const char* SSID = "Cudy-2G";
const char* PASSWORD = "Molunia2021";
const char* ws_host = "10.0.12.20";
const int ws_port = 8765;

bool is_movement_stopped = true;

//LIDAR DATA
uint8_t LIDAR_BUFF[512];
int CNT = 0;
uint8_t START_BYTE_LIDAR = 0xAA;
int DIST_BUFF_LIDAR[180];
int INDX_CNTR = 0;
int CURRENT_ANGLE_TEMP = 0;
bool IS_LIDAR_TIME = true;
bool IS_WAITING_FOR_NEXT_INSTRUCTION = false;
WebSocketsClient webSocket;




extern volatile byte DIRECTION = 0b0000;
extern volatile uint32_t SPEED = 5;
float DISTANCE_PER_STEP = 0.1226;

void loop_core0(void *pvParameters);
void loop_core1(void *pvParameters);





void configure_step_motor(int step_pin, int dir_pin, int en_pin){
  pinMode(step_pin, OUTPUT);
  pinMode(dir_pin, OUTPUT);
  pinMode(en_pin, OUTPUT);
  digitalWrite(en_pin, LOW);
}

void parseFrame() {
  LOCK_LIDAR = true;
  uint8_t buffer_length[2];
  if (LIDAR_SERIAL.readBytes(buffer_length, 2) != 2) return;

  // Konwersja Big Endian
  uint16_t packet_len = (buffer_length[0] << 8) | buffer_length[1];
  if (packet_len > 1000 || packet_len == 0) return;

  // Reszta ramki, czyli payload z CRC
  size_t payload_and_checksum = packet_len + 2;
  
  if (LIDAR_SERIAL.readBytes(LIDAR_BUFF, payload_and_checksum) != payload_and_checksum) return;
  
  if (LIDAR_BUFF[1] != 0x61) return; 

  // długość ramki w 2 bajtach
  uint16_t data_length = (LIDAR_BUFF[3] << 8) | LIDAR_BUFF[4];

  // odczytanie kątu startowego
  uint16_t start_angle_raw = (LIDAR_BUFF[8] << 8) | LIDAR_BUFF[9];
  float start_angle = start_angle_raw * 0.01;

  // odczytanie liczby próbek
  int sample_count = (data_length - 5) / 3;
  if (sample_count <= 0) return;

  // obliczenie współczynnika kąta
  float coeff = 360.0 / (16.0 * sample_count);

  // pętla po próbkach
  int base_offset = 10;

  for (int i = 0; i < sample_count; i++) {
    int idx = base_offset + (i * 3);
    // jakość, dystans MSB, dystans LSB

    uint8_t quality = LIDAR_BUFF[idx];
    uint8_t dist_high = LIDAR_BUFF[idx + 1];
    uint8_t dist_low  = LIDAR_BUFF[idx + 2];
    uint16_t dist_raw = (dist_high << 8) | dist_low;

    uint16_t distance_cm = dist_raw / 44;

    // Obliczenie kąta  
    float current_angle = start_angle + (i * coeff);
    if (current_angle >= 360.0) {
      current_angle -= 360.0;
      }
    if (distance_cm > 0 && quality > 0) {
        
      int indx = current_angle / 2;
      DIST_BUFF_LIDAR[indx] = (int)(distance_cm);
      }
    

  }
  LOCK_LIDAR = false;

}
String combine_msg_lidar(){
  String msg = "!lidar;180;";
  msg.reserve(1024);
  for (int i = 0; i < 180; i++){
    msg += String(DIST_BUFF_LIDAR[i]);
    msg += ";";

  }
  msg += "radil!";

  return msg;
}


void setup() {
  Serial.begin(115200);
  LIDAR_SERIAL.begin(115200, SERIAL_8N1, 9, -1);
  Wire.begin();

  Wire.beginTransmission(0x68);
  Wire.write(0x37);
  Wire.write(0x02);
  Wire.endTransmission();
  delay(10);

  // 2. Inicjalizacja MMC5883L
  Wire.beginTransmission(MMC5883L_ADDR);
  Wire.write(0x08); // Rejestr kontrolny
  Wire.write(0x01); // Soft reset
  Wire.endTransmission();
  delay(20);

  Wire.beginTransmission(MMC5883L_ADDR);
  Wire.write(0x08); 
  Wire.write(0x0D); // Tryb ciągły, 100Hz
  Wire.endTransmission();

  configure_step_motor(STEP_PIN,DIR_PIN,EN_PIN);
  configure_step_motor(STEP_PIN1,DIR_PIN1,EN_PIN1);
  if (!mag.begin()) Serial.println("Błąd HMC5883L");

  WiFi.begin(SSID,PASSWORD);
  while(WiFi.status() != WL_CONNECTED){
    vTaskDelay(pdMS_TO_TICKS(500));
    STATE = ROBOT_STATE::NO_WIFI;
    Serial.println(ROBOT_STATE_NAMES[STATE]);
  }

  STATE = ROBOT_STATE::IS_THERE_ANY_INSTRUCTION;

  webSocket.begin("10.0.12.20", 8765);
  webSocket.onEvent(webSocketEvent);
  webSocket.setReconnectInterval(5000);
  
  xTaskCreatePinnedToCore(loop_core0, "Core0", 4096, NULL, 1, NULL, 0);
  xTaskCreatePinnedToCore(loop_core1, "Core1", 16384, NULL, 1, NULL, 1);

  
}

void loop_core0(void *pvParameters){

  digitalWrite(EN_PIN1, LOW);
  digitalWrite(EN_PIN, LOW);
  while(true){

        if (LIDAR_SERIAL.available()){
          uint8_t first_byte = LIDAR_SERIAL.read();
          // Serial.println("LIDAR_READ");
          if (first_byte == START_BYTE_LIDAR)
          parseFrame();
        }
      if (STATE == ROBOT_STATE::PROCEEDING_INSTRUCTION){

      
        //Do przodu
        if(DIRECTION == 0){
          digitalWrite(DIR_PIN, HIGH);
          digitalWrite(DIR_PIN1, LOW);
        } //Do tyłu
        else if(DIRECTION == 1){
          digitalWrite(DIR_PIN, LOW);
          digitalWrite(DIR_PIN1, HIGH);
        } // W prawo
        else if(DIRECTION == 2){
          digitalWrite(DIR_PIN, HIGH);
          digitalWrite(DIR_PIN1, HIGH);
        } // W lewo
        else if(DIRECTION == 3){
          digitalWrite(DIR_PIN, LOW);
          digitalWrite(DIR_PIN1, LOW);
        }

        
        if(DIRECTION == 2 or DIRECTION == 3){
          if (SPEED == 1){
            SPEED = 263;
          }
          else if (SPEED == 2){
            SPEED = 526;
          }
          else if (SPEED == 3){
            SPEED = 789;
          }
          else if (SPEED == 4){
            SPEED = 1052;
          }
        }

        for (int x = 0; x < SPEED / DISTANCE_PER_STEP; x++){
          // Serial.print(".");
          digitalWrite(STEP_PIN, HIGH);
          digitalWrite(STEP_PIN1, HIGH);

          delayMicroseconds(2000);
            
          digitalWrite(STEP_PIN, LOW);
          digitalWrite(STEP_PIN1, LOW);
          delayMicroseconds(2000);

          if (x % 10 == 0) {
            vTaskDelay(pdMS_TO_TICKS(1));
          }
        }
          // Serial.println("...");
          is_movement_stopped = true;
          SPEED = 0;
          STATE = ROBOT_STATE::PROCEEDING_INSTRUCTION_DONE;
      }
      

    vTaskDelay(pdMS_TO_TICKS(1));
  }
}

void zero_lidar_buff() {
  // for (int i = 0; i < 180; i++) {
  //   DIST_BUFF_LIDAR[i] = 0;
  // }
}
  void tick() {
    Serial.println(ROBOT_STATE_NAMES[STATE] + " | " + (IS_WAITING_FOR_MESSSAGE ? "IS_WAITING_FOR_MESSSAGE": ""));
  }

void webSocketEvent(WStype_t type, uint8_t * payload, size_t length) {
    if (type != WStype_TEXT || !IS_WAITING_FOR_MESSSAGE) return;

    String msg = (char*)payload;

    if (msg.startsWith("!any_cmd;")) {
        int f_idx = msg.indexOf(';');
        int s_idx = msg.indexOf(';', f_idx + 1);
        if (msg.substring(f_idx + 1, s_idx) != "0") {
            STATE = ROBOT_STATE::SENDING_LIDAR_DATA;
        } else {
            // Serwer mówi: nic nie rób, więc zostajemy w tym samym stanie
            // ale musimy odblokować wysyłanie kolejnego zapytania za chwilę
        }
    } 
    else if (msg.startsWith("!lidar")) {
        if (STATE == ROBOT_STATE::SENDING_LIDAR_DATA) {
            STATE = ROBOT_STATE::GET_INSTRUCTION;
        }
    } 
    else if (msg.startsWith("!get_cmds;")) {
        int s1 = msg.indexOf(';');
        int s2 = msg.indexOf(';', s1 + 1);
        int s3 = msg.indexOf(';', s2 + 1);
        
        if (s1 != -1 && s2 != -1 && s3 != -1) {
            DIRECTION = msg.substring(s1 + 1, s2).toInt();
            SPEED = msg.substring(s2 + 1, s3).toInt();
            STATE = ROBOT_STATE::PROCEEDING_INSTRUCTION; // Przekazujemy pałeczkę do Core 0
        }
    } 
    else if (msg.startsWith("!cmd_done")) {
        STATE = ROBOT_STATE::IS_THERE_ANY_INSTRUCTION;
    }

    IS_WAITING_FOR_MESSSAGE = false; // Zawsze odblokuj po otrzymaniu odpowiedzi
}

unsigned long last_request_time = 0;

void loop_core1(void *pvParameters) {
  while (true) {
    webSocket.loop();
    vTaskDelay(pdMS_TO_TICKS(10));

    // 1. Sprawdź, czy w ogóle mamy połączenie
    if (!webSocket.isConnected()) {
      if (STATE != ROBOT_STATE::NO_CONNECTION_TO_SERVER) {
        Serial.println("Utracono połączenie z serwerem!");
        STATE = ROBOT_STATE::NO_CONNECTION_TO_SERVER;
        IS_WAITING_FOR_MESSSAGE = false;
      }
      continue; 
    } else if (STATE == ROBOT_STATE::NO_CONNECTION_TO_SERVER) {
      // Jeśli połączenie wróciło, wróć do sprawdzania instrukcji
      STATE = ROBOT_STATE::IS_THERE_ANY_INSTRUCTION;
    }

    // 2. Obsługa Timeoutu
    if (IS_WAITING_FOR_MESSSAGE && (millis() - last_request_time > 3000)) {
      Serial.println("BŁĄD: Timeout odpowiedzi serwera. Resetowanie flagi...");
      IS_WAITING_FOR_MESSSAGE = false;
    }

    if (IS_WAITING_FOR_MESSSAGE) continue;

    // 3. Maszyna stanów
    switch (STATE) {
      case IS_THERE_ANY_INSTRUCTION:
        webSocket.sendTXT("!any_cmd;;dmc_yna!");
        IS_WAITING_FOR_MESSSAGE = true;
        last_request_time = millis();
        break;

      case SENDING_LIDAR_DATA: {
        String lidarData = combine_msg_lidar();
        webSocket.sendTXT(lidarData);
        IS_WAITING_FOR_MESSSAGE = true;
        last_request_time = millis();
        break;
      }

      case GET_INSTRUCTION:
        webSocket.sendTXT("!get_cmds;;sdmc_teg!");
        IS_WAITING_FOR_MESSSAGE = true;
        last_request_time = millis();
        break;

      case PROCEEDING_INSTRUCTION:
        
        break;

      case PROCEEDING_INSTRUCTION_DONE:
        STATE = ROBOT_STATE::SENDING_DONE;
        break;

      case SENDING_DONE:
        webSocket.sendTXT("!cmd_done;;enod_dmc!");
        IS_WAITING_FOR_MESSSAGE = true;
        last_request_time = millis();
        break;
        
      default:
        break;
    }
  }
}
void loop() {

}