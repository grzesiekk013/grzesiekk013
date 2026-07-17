#include <Arduino.h>

#ifndef SERIAL_UTILS_H
#define SERIAL_UTILS_H
#endif

void print_help();
void print_wifi_status();
void print_time_status();
void print_app_status();
void print_dev_status();
void print_sens_status();
void print_encrypt_sensor_as_json();
void print_msg();
void serial_read();
void parse_command();
void encrypt_message();
void decrypt_message();
void write_wifi_ssid(String ssid);
void write_wifi_password(String password);
void write_app_ip(String ip);
void write_app_port(int port);
void write_encrypt_key(String key);
void write_uuid(String uuid);
void write_msg(String msg);
int char_counter(String str, char c);
void set_expiration_key_time(String nbr);
void reboot_system();
bool validate_ip_address(String ip);
bool validate_port(int port);
String add_encrypt_prefix_json(String data);
String encrypt_json_request(String json);
bool isNumber(String time);

String encrypt_json(String json);
void decrypt_json();
void delete_key();
void should_use_encryption(String input);
void hex_key_after_reboot(String key);