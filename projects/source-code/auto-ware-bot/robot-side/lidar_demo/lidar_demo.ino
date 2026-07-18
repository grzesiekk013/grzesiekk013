#include <Arduino.h>

// Konfiguracja pinów
#define LIDAR_RX_PIN 3  // Twój wybrany pin (podłączony do TX Lidara)
#define LIDAR_TX_PIN -1 // TX nie jest potrzebny, jeśli tylko czytamy dane

// Struktura ramki (uproszczona dla czytelności)
// Delta 2A zazwyczaj: 0xAA (Start) | Len | Addr | Type | ... Data ... | Checksum

void setup() {
  // 1. Inicjalizacja portu szeregowego do komputera (USB)
  Serial.begin(115200);
  // Dajemy chwilę na otwarcie monitora
  delay(2000);
  Serial.println("System startuje...");
  Serial.println("Oczekiwanie na dane z Lidara na pinie IO3...");

  // 2. Inicjalizacja portu dla Lidara
  // Ważne: Zmieniamy baud rate na 115200 zgodnie z prośbą
  Serial1.begin(115200);
}

void loop() {
  // Sprawdzamy, czy są dane w buforze
  if (Serial1.available()) {
    uint8_t byte = Serial1.read();
    Serial.println(byte);
    // Szukamy nagłówka ramki 0xAA
    if (byte == 0xAA) {
      // Delta 2A często wysyła pakiety o różnej długości, ale spróbujmy odczytać kluczowe bajty.
      // Czekamy chwilę na resztę ramki (bezpiecznik czasowy)
      unsigned long timeout = millis();
      while (Serial1.available() < 10 && millis() - timeout < 10) {
        // Czekaj na wypełnienie bufora
      }
      
      if (Serial1.available() >= 7) { // Minimalna długość, żeby wyciągnąć dane
        uint8_t buffer[10];
        Serial1.readBytes(buffer, 8); // Czytamy kolejne bajty po 0xAA

        // Analiza wg standardu 3irobotix (przykładowa struktura ramki danych)
        // buffer[0] zwykle to długość lub typ ramki
        // Spróbujmy znaleźć dane kąta i odległości
        // W typowej ramce 'measurement':
        // Offsety mogą się różnić zależnie od wersji firmware'u Lidara przy 115200
        
        // Próba dekodowania (zakładając standardowe przesunięcie po nagłówku)
        // Często: Kąt (2B) | Odległość (2B)
        
        // Zgodnie z dokumentacją niektórych wersji, po 0xAA następuje bajt długości
        // Tu stosujemy podejście "surowe" żebyś widział co przychodzi, jeśli protokół jest inny przy 115200
        
        uint16_t distRaw = (buffer[3] << 8) | buffer[4]; // Przykład: bajty 4 i 5
        uint16_t angleRaw = (buffer[1] << 8) | buffer[2]; // Przykład: bajty 2 i 3
        
        float distance = distRaw / 4.0; // Przeliczenie na mm
        float angle = angleRaw / 100.0; // Przeliczenie na stopnie

        // Formatowanie wyjścia dla czytelności (Globalny porządek)
        Serial.print("RAW Hex: ");
        for(int i=0; i<5; i++) {
            Serial.print(buffer[i], HEX); 
            Serial.print(" ");
        }
        
        // Wyświetlamy przeliczone dane, jeśli mają sens fizyczny
        if(distance > 0 && distance < 10000) {
            Serial.print(" | Kąt: ");
            Serial.print(angle);
            Serial.print(" | Dystans: ");
            Serial.println(distance);
        } else {
            Serial.println(" | (Szukanie synchronizacji...)");
        }
      }
    }
  }
}