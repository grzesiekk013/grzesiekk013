#include <WiFi.h>
#include <WebSocketsClient.h>

#ifndef NETWORK_UTILS_H
#define NETWORK_UTILS_H
#endif

WebSocketsClient webSocket;

uint64_t  get_server_time();
bool check_server_connection();
bool ping_server();

void connect_to_wifi();
void disconnect_wifi();
void check_wifi_connection_status();
void reconnect_to_wifi();
void disconnect_wifi();
void read_mac();
void read_ip();

void ws_send();

void ws_event();