#include <WiFi.h>
#include <HTTPClient.h>

#ifndef NETWORK_UTILS_H
#define NETWORK_UTILS_H
#endif

uint64_t  get_server_time();
bool check_server_connection();
bool ping_server();
void send_json_post_request(String URL, String Body);
void connect_to_wifi();
void disconnect_wifi();
void check_wifi_connection_status();
void reconnect_to_wifi();
void disconnect_wifi();
bool http_put();
bool http_post();
bool http_get();
void read_mac();
void read_ip();