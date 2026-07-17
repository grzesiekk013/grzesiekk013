#include "flash-utils.h"
#include "datatypes.h"
#include "encryption-utils.h"
bool does_preference_exists(String name) {
  return FLASH.isKey(name.c_str());
}

void load_flash() {
  SSID = does_preference_exists("SSID") ? FLASH.getString("SSID") : "";
  WIFI_PASSWORD = does_preference_exists("WIFI_PASSWORD") ? FLASH.getString("WIFI_PASSWORD") : "";

  EYE_SERVER_IP = does_preference_exists("EYE_SERVER_IP") ? FLASH.getString("EYE_SERVER_IP") : "";
  EYE_SERVER_PORT = does_preference_exists("EYE_SERVER_PORT") ? FLASH.getInt("EYE_SERVER_PORT") : 0;
  UUID = does_preference_exists("UUID") ? FLASH.getString("UUID") : "";

  ENCRYPT_KEY = does_preference_exists("ENCRYPT_KEY") ? FLASH.getString("ENCRYPT_KEY") : "";
  USE_ENCRYPTION = does_preference_exists("USE_ENCRYPTION") ? FLASH.getBool("USE_ENCRYPTION") : false;
}

void save_string_flash(String key, String value) {
  FLASH.putString(key.c_str(), value.c_str());
}

void save_int_flash(String key, int value) {
  FLASH.putInt(key.c_str(), value);
}

void save_bool_flash(String key, bool value){
  FLASH.putBool(key.c_str(), value);
}




