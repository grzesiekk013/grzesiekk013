# Board  
DFROBOT FIREBEETLE 2 ESP-S3  
  
## Getting started tutorial  
https://www.electroniclinic.com/firebeetle-2-esp32-s3-wroom-1-getting-started-tutorial/  
  
### IDE Preferences URL    
https://raw.githubusercontent.com/espressif/arduino-esp32/gh-pages/package_esp32_index.json  
      
## TODO LIST  
// 1. Znaki wykluczone - specjalne -!#[]{}  
// 2. Parsowanie wejść  
// 3. input -> [/> ]  
// 4. log <> -> ()  
// 5. NTP CLIENT CONNECTION   
6. HTTP REQUEST INTERVAL  
// 7. ADD REBOOT COMMAND  
  
## PINOUT  
SPI: SCK 17, MOSI 15, MISO 16  
I2C: SCL 2, SDA 1  
UART: TX 43, RX 44  
  
## NTP  
tempus1.gum.gov.pl -> 194.146.251.100  
tempus2.gum.gov.pl -> 194.146.251.101  
  
### HTTP POST EXAMPLE BODY  
```json
{
    "dev_uuid": "000000000",
    "dev_addr": "000000000",
    "dev_mac": "00000000",
    "time_epoch": 00000000,
    "command": "post",
    "data": {
        "sensors": [
            {
                "name": "MLX90614", "id": 0, 
                "readings": {
                    "tempK": 230
                }
            },
            {
                "name": "MLX90614", "id": 1, 
                "readings": {
                    "tempK": 230
                }
            }
        ]
    }

}
```
