#include "datatypes.h"
#include "serial-utils.h"
#include "flash-utils.h"
#include "network-utils.h"
#include "sensor-utils.h"       
#include "encryption-utils.h"      

#define print_ln(x) Serial.println(x)

String TOP_LINE = "__ EYE_OS ___________________________________________________________________\n";
String BOT_LINE = "_____________________________________________________________________________\n";




void reboot_system() {
  print_ln("( SYSTEM IS REBOOTING )");
  delay(1000);
  esp_restart();
}

bool isHexString(const String &str) {
    // Sprawdzenie, czy ciąg składa się wyłącznie z heksadecymalnych znaków
    for (unsigned int i = 0; i < str.length(); i++) {
        char c = str.charAt(i);
        if (!((c >= '0' && c <= '9') || (c >= 'a' && c <= 'f') || (c >= 'A' && c <= 'F'))) {
            return false;
        }
    }
    return true;
}

void write_uuid(String uuid) {
  if (uuid.length() != UUID_LEN) {
    print_ln("( UUID LENGTH SHOULD BE 64 SYMBOLS )");
    return;
  }
  if (!isHexString(uuid)) {
    print_ln("( PROVIDED UUID IS NOT HEXADECIMAL )");
    return;
  }
  UUID = uuid;
  save_string_flash("UUID", uuid);
}
void delete_key(){
  ENCRYPT_KEY = "";
  IS_ENCRYPTED_KEY = false;
  save_string_flash("ENCRYPT_KEY",ENCRYPT_KEY);

}



void write_encrypt_key(String key) {
  ENCRYPT_KEY = key;
  save_string_flash("ENCRYPT_KEY",ENCRYPT_KEY);
  IS_ENCRYPTED_KEY = make_hex_key(ENCRYPT_KEY);
}

void hex_key_after_reboot(String key){
  ENCRYPT_KEY = key;
  IS_ENCRYPTED_KEY = make_hex_key(ENCRYPT_KEY);
}


// DecareKesForAsS256sncrtptionnnnn
void write_wifi_ssid(String ssid) {
  if (ssid.length() <= 0) {
    print_ln("( SSID CAN'T BE EMPTY )");
    return;
  }
  disconnect_wifi();
  SSID = ssid;
  save_string_flash("SSID", ssid);
}
void write_wifi_password(String password) {
  disconnect_wifi();
  WIFI_PASSWORD = password;
  save_string_flash("WIFI_PASSWORD", password);
}

bool validate_ip_address(String ip) {
  if (char_counter(ip, '.') != 3) {
    print_ln("( PROVIDED IP ADDRES IS NOT VALID )");
    return false;
  }
  int tmp = 0;
  for (int i = 0; i < ip.length(); i++) {
    if (ip.charAt(i) == '.') {
      String octet = ip.substring(tmp, i);
      print_ln(octet);
      tmp = i + 1;
      if(octet.length() == 0 || octet.toInt() > 255 || octet.toInt() < 0) {
        print_ln("( PROVIDED IP ADDRES IS NOT VALID )");
        return false;
      }
    }
  }
  return true;
}


bool validate_port(int port) {
  if (port > 65535 || port <= 0) {
    print_ln("( PROVIDED PORT IS NOT VALID )");
    return false;
  }
  return true;
}
void write_app_ip(String ip) {
  if (!validate_ip_address(ip)) return;
  EYE_SERVER_IP = ip;
  save_string_flash("EYE_SERVER_IP", ip);
}


String encrypt_json(String json){
  int encryptedLen = 0;
  encrypt_value(json.c_str(),ENCRYPTED_BUFFER,encryptedLen);
  String hexResult = "";
  for (int i = 0; i < encryptedLen; i++){
    if (ENCRYPTED_BUFFER[i] < 0x10) hexResult += "0";
    hexResult += String(ENCRYPTED_BUFFER[i], HEX);
  }
  return hexResult;
}

void decrypt_json(){

char decryptedText[512];
decrypt_value(ENCRYPTED_BUFFER,ENCRYPTED_JSON_LENGTH,decryptedText);
Serial.println(decryptedText);
}

void write_app_port(int port) {

  EYE_SERVER_PORT = port;
  save_int_flash("EYE_SERVER_PORT", port);
}



void print_msg(){
  if (MESSAGE){
    print_ln(
      TOP_LINE + "\n" +
      "Your message is -> " + MESSAGE + "\n"
    );
  }
  else{
    print_ln("There is no message");
  }
}

void print_help() {
  print_ln(
    TOP_LINE + "\n" +
    "? -> Get Some Help (This window)\n"
    "![0]{}# -> Print WiFi status\n"
    "![1]{}# -> Print Time Client status\n"
    "![2]{}# -> Print EYE APP status\n"
    "![3]{}# -> Print Device info\n"
    "![4]{}# -> Print Sensor status\n"
    "\n"
    "![a]{SSID}#     -> Write WiFi SSID\n"
    "![b]{PASSWORD}# -> Write WiFi PASSWORD\n"
    "![c]{10.0.0.2}# -> Write EYE_APP IP\n"
    "![d]{8080}#     -> Write EYE_APP PORT\n" 
    "![e]{0 or 1}#   -> Do you want to use encryption ?\n"
    "![$$]{UUID}#    -> Write UUID to DEV\n" 
    "![$$$]{KEY}#    -> Write KEY to DEV\n" 
    "![$$$$]{}#      -> Delete KEY form DEV\n" 
    "![r]{}#         -> Reboot system\n" +
    BOT_LINE + "\n"
  );
}

void print_wifi_status() {
  print_ln(
    TOP_LINE + 
    "WIFI_STATUS   -> " + String(WIFI_STATUS_NAMES[WIFI_STATUS]) + "\n" +
    "WIFI_SSID     -> " + SSID + "\n" +
    "WIFI_PASSWORD -> " + WIFI_PASSWORD + "\n" +
    "MAC_ADDRESS   -> " + MAC_ADDRESS + "\n" +
    "IP_ADDRESS    -> " + myIP + "\n" +
    BOT_LINE
  );
}
void print_time_status() {
  print_ln(
    TOP_LINE +
    "TIME_STATUS    -> " + String(TIME_STATUS_NAMES[TIME_STATUS]) + "\n" +
    "TIME_EPOCH_MS  -> " + String(TIME_EPOCHMS) + "\n" +
    BOT_LINE
  );
}
void print_app_status() {
  print_ln(
    TOP_LINE + 
    "EYE_STATUS      -> " + String(APP_STATUS_NAMES[APP_STATUS]) + "\n" +
    "EYE_SERVER_IP   -> " + EYE_SERVER_IP + "\n" +
    "EYE_SERVER_PORT -> " + String(EYE_SERVER_PORT) + "\n" +
    BOT_LINE
  );
}
void print_dev_status() {
  print_ln(
    TOP_LINE + 
    "UUID           -> " + UUID + "\n" +
    "ENCRYPT_KEY    -> " + ENCRYPT_KEY + "\n" +
    "ENCRYPTED JSON -> " + (USE_ENCRYPTION ? "TRUE" : "FALSE") + "\n" +  
    // "            -> " + ENCRYPT_KEY.substring(16,16) + "\n" +
    // "            -> " + ENCRYPT_KEY.substring(32,16) + "\n" +
    // "            -> " + ENCRYPT_KEY.substring(48,16) + "\n" +
    BOT_LINE
  );
}
void print_sens_status() {
  print_ln(
    TOP_LINE + 
    "GY906    -> " + get_mlx90614() + "\n" +
    "ENS160   -> " + get_ens160() + "\n" +
    "SEN22396 -> " + get_senscdx4x() + "\n" +
    BOT_LINE
  );
}

void should_use_encryption(String input){

  if (input != "0" && input != "1"){
    print_ln("( WRONG INPUT NUMBER, YOU CAN CHOSE ONLY BETWEEN 0 AND 1 ! )");
    print_ln(input + " : " + input.length() + " : " + input[0]);
    return;
  }
  if (input == "0") 
  {
    USE_ENCRYPTION = false;
    save_bool_flash("USE_ENCRYPTION",USE_ENCRYPTION);
    
  }
  else if (input == "1") 
  {
    USE_ENCRYPTION = true;
    save_bool_flash("USE_ENCRYPTION",USE_ENCRYPTION);
    
  }
  
}

void serial_read() {
  while (Serial.available()) {
    char c = Serial.read();
    
    if (SERIAL_BUFF_IDX >= 128) {
      SERIAL_BUFF_IDX = 0;
      memset(SERIAL_BUFF, 0, sizeof(SERIAL_BUFF));
      print_ln("( SERIAL_BUFF OVERFLOW, type command again )");
    } else {
      if (c == '\n') {
        parse_command();
      } else {
        SERIAL_BUFF[SERIAL_BUFF_IDX] = c;
        SERIAL_BUFF_IDX++;
      }
    }
  }
}

int char_counter(String str, char c) {
  int cnt = 0;
  int pos = str.indexOf(c);
  while (pos != -1) {
    cnt++;
    pos = str.indexOf(c, pos+1);
  }
  return cnt;
}

const char special_chars[] = "!?#[]{}";

void parse_command() {
  // USE SERIAL_BUFF
  print_ln("[/> " + String(SERIAL_BUFF)+" ]");
  if (strchr(SERIAL_BUFF, '?')) {
    print_help();
    memset(SERIAL_BUFF, 0, sizeof(SERIAL_BUFF));
    SERIAL_BUFF_IDX=0;
    return;
  }
  // Check special characters
  for (int i = 0; special_chars[i] != '\0'; i++) {
    if (char_counter(String(SERIAL_BUFF), special_chars[i]) > 1 ) {
      print_ln("( YOU CAN'T USE SPECIAL CHARACTERS !#?[]{} MORE THAN ONCE )");
      memset(SERIAL_BUFF, 0, sizeof(SERIAL_BUFF));
      SERIAL_BUFF_IDX=0;
      return;
    }
  }
  String COMMAND = "";
  String ARGUMENT = "";

  if (strchr(SERIAL_BUFF, '!') == NULL || strchr(SERIAL_BUFF, '#') == NULL) {
    print_ln("( Wrong command syntax, type ? for help )");
    memset(SERIAL_BUFF, 0, sizeof(SERIAL_BUFF));
    SERIAL_BUFF_IDX=0;
    return;
  }

  char *startBrackets = strchr(SERIAL_BUFF, '[');
  char *endBrackets = strchr(SERIAL_BUFF, ']');
  char *startCurly = strchr(SERIAL_BUFF, '{');
  char *endCurly = strchr(SERIAL_BUFF, '}');

  char bracketsContent[128] = {0};
  char curlyContent[128] = {0};

  if (startBrackets && endBrackets && startBrackets < endBrackets) {
    strncpy(bracketsContent, startBrackets + 1, endBrackets - startBrackets - 1);
    bracketsContent[endBrackets - startBrackets - 1] = '\0';
  }

  if (startCurly && endCurly && startCurly < endCurly) {
    strncpy(curlyContent, startCurly + 1, endCurly - startCurly - 1);
    curlyContent[endCurly - startCurly - 1] = '\0';
  }

  COMMAND = String(bracketsContent);
  ARGUMENT = String(curlyContent);

  if (COMMAND == "0") {
    print_wifi_status();
  } else if (COMMAND == "1") {
    print_time_status();
  } else if (COMMAND == "2") {
    print_app_status();
  } else if (COMMAND == "3") {
    print_dev_status();
  } else if (COMMAND == "4") {
    print_sens_status();
  } else if (COMMAND == "a") {
    write_wifi_ssid(ARGUMENT);
  } else if (COMMAND == "b") {
    write_wifi_password(ARGUMENT);
  } else if (COMMAND == "c") {
    write_app_ip(ARGUMENT);
  } else if (COMMAND == "d") {
    int num = ARGUMENT.toInt();
    if (num == 0) print_ln("( Provided Port is Wrong )");
    write_app_port(num); 
  } else if (COMMAND == "e") {
    should_use_encryption(ARGUMENT);
  } else if (COMMAND == "$$") {
    write_uuid(ARGUMENT);
  } else if (COMMAND == "$$$") {
    write_encrypt_key(ARGUMENT); 
  } else if (COMMAND == "$$$$"){
    delete_key();
  }else if (COMMAND == "r") {
    reboot_system();
  } else {
    print_ln("( Unknown command, type ? and Enter for help )");
  }
  memset(SERIAL_BUFF, 0, sizeof(SERIAL_BUFF));
  SERIAL_BUFF_IDX=0;
}



