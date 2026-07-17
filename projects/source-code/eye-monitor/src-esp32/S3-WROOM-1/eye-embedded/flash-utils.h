#include <Arduino.h>

#ifndef FLASH_UTILS_H
#define FLASH_UTILS_H
#endif

bool does_preference_exists(String name);
void load_flash();
void save_int_flash(String key, int value);
void save_string_flash(String key, String value);
void save_bool_flash(String key, bool value);