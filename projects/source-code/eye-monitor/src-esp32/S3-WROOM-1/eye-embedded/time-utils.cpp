#include "time-utils.h"
#include "datatypes.h"
// #include <NTPClient.h>
// #include <WiFiUdp.h>

// WiFiUDP UDP;
// NTPClient* NTP_CLIENT = nullptr;

// void cleanupNTP() {
//     if (NTP_CLIENT != nullptr) {
//         delete NTP_CLIENT;
//         NTP_CLIENT = nullptr;
//     }
// }

// void ntp_init() {
//     cleanupNTP();
//     // Serial.println("( CONNECTING TO NTP SERVER )");

//     if (NTP_SERVER_IP.length() > 0) {
//         const char* serverIP = NTP_SERVER_IP.c_str(); // Konwersja String na const char*
//         NTP_CLIENT = new NTPClient(UDP, serverIP, 3600, 60000); // Inicjalizacja NTPClient z serwerem
//         NTP_CLIENT->begin();
//         delay(500);
//     } else {
//         Serial.println("( NTP SERVER IP IS EMPTY )");
//     }
// }


// unsigned long ntp_epoch_ms() {
//   return NTP_CLIENT->getEpochTime() * 1000;
// }
// void ntp_update() {
//   if (NTP_CLIENT->isTimeSet()) {
//       NTP_STATUS = NTP_CONNECTED;
//       Serial.println("( NTP CONNECTED )");
//   } else {
//       NTP_STATUS = NTP_DISCONNECTED;
//       Serial.println("( NTP UNABLE TO CONNECT )");
//   }
//   NTP_CLIENT->update();
// }