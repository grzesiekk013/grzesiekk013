#include <Arduino.h>

#ifndef SERIAL_UTILS_H
#define SERIAL_UTILS_H
#endif

void print_help();
void print_wifi_status();
void print_dev_status();
void print_sens_status();

void print_msg();
void serial_read();
void parse_command();

void write_wifi_ssid(String ssid);
void write_wifi_password(String password);
void write_app_ip(String ip);
void write_app_port(int port);
void write_uuid(String uuid);
void write_msg(String msg);

int char_counter(String str, char c);

void reboot_system();
bool validate_ip_address(String ip);
bool validate_port(int port);

bool isNumber(String time);