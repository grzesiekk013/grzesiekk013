#include <ESP8266WiFi.h>
#include <NTPClient.h>
#include <WiFiUdp.h>
#include <time.h>
#ifndef STASSID
#define STASSID "your-ssid";
#define STAPSK  "your-password";
#endif

const char* ssid = STASSID;
const char* password = STAPSK;

String wszystkieZaluzjeStatus = "u gory";
String wejscioweZaluzjeStatus = "u gory";
String formattedDate;
String dayStamp;
String timeStamp;
String secretKey="PhjCVzvU7GPvgC33CMxnRsj8CY9p4n2K";

int godzina,minuta,sekunda;
int najwczesniejszySwit = 6;
int najpozniejszySwit = 8;
int najwczesniejszyWieczor = 16;
int najpozniejszyWieczor = 23;
int czasWlaczeniaRelaya = 3;

bool wszystkie_do_gory_called = false;
bool wszystkie_na_dol_called = false;
bool wejscie_do_gory_called = false;
bool wejscie_na_dol_called = false;
bool brama_garazowa_called = false;
bool brama_wjazdowa_called = false;
bool schedule_dzisiaj_opuszczone = false;
bool schedule_dzisiaj_podniesione = false;
bool czujnikStatus = false;
bool czasUstawionyPoprawnie = false;
bool ustaw_czas_called = false;

const int OUT1 = D0;
const int OUT2 = D1;
const int OUT3 = D2;
const int OUT4 = D3;
const int OUT5 = D4;
const int OUT6 = D5;
const int OUT7 = D6;
const int OUT8 = D7;
const int ErrorWifi = D9;
const int ErrorCzas = D10;
const int czujnik = A0;
const int czujnikBariera = 400;

WiFiServer server(80);
WiFiUDP ntpUDP;
NTPClient timeClient(ntpUDP);
void setup() {
  pinMode(OUT1, OUTPUT);
  pinMode(OUT2, OUTPUT);
  pinMode(OUT3, OUTPUT);
  pinMode(OUT4, OUTPUT);
  pinMode(OUT5, OUTPUT);
  pinMode(OUT6, OUTPUT);
  pinMode(OUT7, OUTPUT);
  pinMode(OUT8, OUTPUT);
  pinMode(ErrorWifi, OUTPUT);
  pinMode(ErrorCzas, OUTPUT);
  pinMode(LED_BUILTIN, OUTPUT);
  delay(1000);
  digitalWrite(ErrorWifi, HIGH);

  Serial.begin(115200);

  // Connect to WiFi network
  Serial.println();
  Serial.print(F("Connecting to "));
  Serial.println(ssid);
  WiFi.mode(WIFI_STA);
  WiFi.begin(ssid, password);

  while (WiFi.status() != WL_CONNECTED) {
    delay(500);
    Serial.print(F("."));
  }
  digitalWrite(ErrorWifi, LOW);
  Serial.println();
  Serial.println("WiFi connected: " + WiFi.SSID() + " (" + WiFi.RSSI() + " dB)");
  WiFi.setAutoReconnect(true);
  WiFi.persistent(true);
  Serial.println("Mac address: "+ WiFi.macAddress());  

  timeClient.begin();
  timeClient.setTimeOffset(3600);

  server.begin();
  Serial.println("IP Address: " + WiFi.localIP().toString());

  //synchronizacja czasu
  if(!timeClient.update()){
     Serial.println("Boot time: read time FAILED");
    digitalWrite(ErrorCzas, HIGH);
  }else{
    Serial.println("Boot time: "+ timeClient.getFormattedTime());
    czasUstawionyPoprawnie = true;
    digitalWrite(ErrorCzas, LOW);
  }
  Serial.println("Secret Key: "+secretKey);
  
}

void relays(String ktore, String zadanie){
    if (ktore == "wszystkie"){
        if (zadanie == "do_gory"){
          wszystkie_do_gory_called = false;
          wszystkieZaluzjeStatus = "u gory";
          //digitalWrite
          digitalWrite(OUT1, LOW);//nieparzyste do gory
          digitalWrite(OUT3, LOW);//nieparzyste do gory
          digitalWrite(OUT5, LOW);//nieparzyste do gory
          delay(czasWlaczeniaRelaya*1000);
           digitalWrite(OUT1, HIGH);//nieparzyste do gory
          digitalWrite(OUT3, HIGH);//nieparzyste do gory
          digitalWrite(OUT5, HIGH);//nieparzyste do gory
          //digitalWrite
        }
        else{
          wszystkie_na_dol_called = false;
          wszystkieZaluzjeStatus = "na dole";
          //digitalWrite
          digitalWrite(OUT2, LOW);//parzyste na dol
          digitalWrite(OUT4, LOW);//parzyste na dol
          digitalWrite(OUT6, LOW);//parzyste na dol
          delay(czasWlaczeniaRelaya*1000);
          //digitalWrite
          digitalWrite(OUT2, HIGH);//parzyste na dol
          digitalWrite(OUT4, HIGH);//parzyste na dol
          digitalWrite(OUT6, HIGH);//parzyste na dol
        }
    }
    if (ktore == "wejscie"){
        if (zadanie == "do_gory"){
          wejscie_do_gory_called = false;
          wejscioweZaluzjeStatus = "u gory";
          //digitalWrite
           
          digitalWrite(OUT5, LOW);//nieparzyste do gory
          delay(czasWlaczeniaRelaya*1000);
          //digitalWrite
          digitalWrite(OUT5, HIGH);//nieparzyste do gory
        }
        else{
           wejscie_na_dol_called = false;
           wejscioweZaluzjeStatus = "na dole";
          //digitalWrite
          digitalWrite(OUT6, LOW);//parzyste na dol
          delay(czasWlaczeniaRelaya*1000);
          digitalWrite(OUT6, HIGH);//parzyste na dol
          //digitalWrite
        }
    }        
    Serial.println("Relays: "+ktore+" "+zadanie);
}
void brama_proc(String obj){
    if (obj == "brama"){
      brama_wjazdowa_called = false;
          //digitalWrite
          digitalWrite(OUT7, LOW);
          delay(czasWlaczeniaRelaya*1000);
          digitalWrite(OUT7, HIGH);
          //digitalWrite
		}
    if (obj == "garaz"){
      brama_garazowa_called = false;
          //digitalWrite
          digitalWrite(OUT8, LOW);
          delay(czasWlaczeniaRelaya*1000);
          digitalWrite(OUT8, HIGH);
          //digitalWrite
		}
    Serial.println("Relays: "+obj);
}
//klasa led
void schedule(){
 if (czasUstawionyPoprawnie){
    if(analogRead(czujnik)>czujnikBariera){
      czujnikStatus=true;
    }else{
      czujnikStatus=false;
    }

    String now = timeClient.getFormattedTime();
    godzina = now.substring(0,2).toInt();
    minuta = now.substring(3,5).toInt();
    sekunda = now.substring(6,8).toInt();

     
     if (WiFi.status() == WL_CONNECTED) {
     digitalWrite(ErrorWifi, LOW);
  }else if((minuta % 2) == 0 ){
    digitalWrite(ErrorWifi, HIGH);
    WiFi.disconnect();
    WiFi.begin(ssid, password);
    delay(5000);
    Serial.print(".");
  }
    //ify
    if(godzina >=1 && godzina<=3 &&(minuta%5)==0){ //miedzy 1 a 3 co 5 min
        schedule_dzisiaj_opuszczone = false;
        schedule_dzisiaj_podniesione = false;
    }
    
    if(!schedule_dzisiaj_podniesione){ //6-8 co 5 min jesli jasno
        if(godzina>=najwczesniejszySwit && godzina <=najpozniejszySwit && (minuta % 5) == 0 && czujnikStatus){
        schedule_dzisiaj_podniesione = true;
        //podnies do gory
        relays("wszystkie","do gory");
        Serial.print(" "+String(godzina)+":"+String(sekunda));
        Serial.println();
        }
    }
    if (!schedule_dzisiaj_opuszczone){ //18-23 co 5 min jesli ciemno
        if(godzina >=najwczesniejszyWieczor && godzina <=najpozniejszyWieczor && (minuta % 5) == 0 &&  !czujnikStatus){
        schedule_dzisiaj_opuszczone = true;
        //opusc na dol
        relays("wszystkie","na dol");
        Serial.print(" "+String(godzina)+":"+String(sekunda));
        Serial.println();
        }
    }
    if(!schedule_dzisiaj_podniesione){
      if(godzina>=najpozniejszySwit && godzina < najwczesniejszyWieczor && (minuta % 5) == 0 && sekunda>=15 && sekunda<=30){ //godzina 8-16 ci 5 min przedziale 15-30s
          schedule_dzisiaj_podniesione = true;
          //podnies do gory
          relays("wszystkie","do gory");
            Serial.print(" "+String(godzina)+":"+String(sekunda)+" h");
        Serial.println();
      }
    }
    if(!schedule_dzisiaj_opuszczone){
      if(godzina >=najpozniejszyWieczor && (minuta % 5) == 0 && sekunda>=15 && sekunda<=30){ //godzina po 23 co 5 min przedziale 15-30s
      schedule_dzisiaj_opuszczone = true;  
      //opusc na dol
      relays("wszystkie","na dol");
        Serial.print(" "+String(godzina)+":"+String(sekunda)+" h");
        Serial.println();
      }
  }
  }else{
     if(!timeClient.update()){
     Serial.println("Fetch time: read time FAILED");
     digitalWrite(ErrorCzas, HIGH);
  }else{
    Serial.println("Fetch time: SUCCESS!");
    czasUstawionyPoprawnie = true;
    digitalWrite(ErrorCzas, LOW);
  }
  }
}
void ustaw_czas(){}
void fakeThread(){
  digitalWrite(LED_BUILTIN, HIGH);
  delay(500);
    if (wszystkie_do_gory_called){relays("wszystkie", "do_gory");}
        if (wszystkie_na_dol_called){relays("wszystkie", "na_dol");}
        if (wejscie_do_gory_called){relays("wejsciowe", "do_gory");}
        if (wejscie_na_dol_called){relays("wejsciowe", "na_dol");}
        if (brama_wjazdowa_called){brama_proc("brama");}
        if (brama_garazowa_called){brama_proc("garaz");}
        if (ustaw_czas_called){ustaw_czas();}
        
  schedule();
  digitalWrite(LED_BUILTIN, LOW);
}

void loop() {
  WiFiClient client = server.available();
  if (!client) {
    fakeThread();
    return;
  }
  client.setTimeout(5000);

  //READ REQUEST AND FIND STRINGS
  String req = client.readStringUntil('\r');
  Serial.println(req);
  if (req.indexOf("wszystkie/na_dol") != -1) {wszystkie_na_dol_called=true;}
  if (req.indexOf("wszystkie/do_gory") != -1) {wszystkie_do_gory_called=true;}
  if (req.indexOf("wejscie/na_dol") != -1) {wejscie_na_dol_called=true;}
  if (req.indexOf("wejscie/do_gory") != -1) {wejscie_do_gory_called=true;}
  if (req.indexOf("brama/"+secretKey) != -1) {brama_wjazdowa_called=true;}
  if (req.indexOf("garaz/"+secretKey) != -1) {brama_garazowa_called=true;}
  if (req.indexOf("ustaw_czas/") != -1) {ustaw_czas_called=true;}//not used

  while (client.available()) {
    client.read();
  }

 // HTML Post
        client.print(F("HTTP/1.1 200 OK\r\nContent-Type: text/html\r\n\r\n<!DOCTYPE HTML>\r\n<html><head><link rel=\"icon\" href=\"data:,\"></head><body>\r\n "));
  client.print("Polaczono do WiFi: " + WiFi.SSID());
  client.print(";<br>");
  client.print("Status wszystkich rolet: " + wszystkieZaluzjeStatus);
  client.print(";<br>");
  client.print("Status wejsciowych rolet: " + wejscioweZaluzjeStatus);
  client.print(";<br>");
  client.print("Status czujnika zmierzchu: ");
  client.print(String(czujnikStatus));
  client.print(";<br>");
  client.print("Lokalny adres ip: ");
  client.print(WiFi.localIP());
  client.print(";<br>");
  client.print("Otwiera od godziny: " + String(najwczesniejszySwit));
  client.print(";<br>");
  client.print("Otwiera do godziny: " + String(najpozniejszySwit));
  client.print(";<br>");
  client.print("Zamyka od godziny: " + String(najwczesniejszyWieczor));
  client.print(";<br>");
  client.print("Zamyka do godziny: " + String(najpozniejszyWieczor));
  client.print(";<br>");
  client.print("Czy czas ustawiony poprawnie: " + String(czasUstawionyPoprawnie));
  client.print(";<br>");
  client.print("</body></html>");
  /*if(czasUstawionyPoprawnie){
    client.print("Czas: " + godzina+":"+minuta+":"+sekunda);
  client.print(";<br>");
  }*/
}
