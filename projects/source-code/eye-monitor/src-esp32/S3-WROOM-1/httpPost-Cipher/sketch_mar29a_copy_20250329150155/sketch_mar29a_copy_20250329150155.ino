#include "Cipher.h"
#include "WiFi.h"
#include "HTTPClient.h"

Cipher* cipher = new Cipher();
const char WIFI_SSID[] = "Example SSID";
const char WIFI_PASSWORD[] = "ExampleWIFIPassword";

String URL = "http://EYE.edu.pl";
String PathName = "/measurements";


void setup() {
  Serial.begin(115200);
  WiFi.begin(WIFI_SSID,WIFI_PASSWORD);
  Serial.println("Connecting");

  while(WiFi.status() != WL_CONNECTED){
    delay(500);
    Serial.print(".");
  }
  Serial.println("");
  Serial.print("Succesfully connected to WiFi Network");
  

  HTTPClient http;
  http.begin(URL + PathName);
  http.addHeader("Content-Type", "application/x-www-form-urlencoded");
  

  char* key = "Abcd1234@@!!0000";
  cipher->setKey(key);
  int32_t DeviceId = 1;
  String DeviceName = "ens160";
  int32_t tvoc = 500;
  int32_t eco2 = 100;
  int32_t aqi = 2;

  char buffer [100];
  sprintf(buffer, "Device ID: %d,  Device Name: %s,  tvoc: %d,  eco2: %d,  aqi: %d,",DeviceId,DeviceName,tvoc,eco2,aqi);
  String data = String(buffer);
  String cipherString = cipher->encryptString(data);

  int httpCode = http.POST(cipherString);

  if (httpCode > 0){
    if (httpCode == HTTP_CODE_OK){
      String payload = http.getString();
      Serial.println(payload);
    }
    else{
      Serial.printf("[HTTP] POST... code: %d\n", httpCode);
    }
  }
  else{
  Serial.printf("[HTTP] POST... failed, error: %s\n", http.errorToString(httpCode).c_str());
  }

  http.end();
  String decipherString = cipher->decryptString(cipherString);



}

void loop() {
  // put your main code here, to run repeatedly:
  delay(1000);
  Serial.println("Dziala !");
}
