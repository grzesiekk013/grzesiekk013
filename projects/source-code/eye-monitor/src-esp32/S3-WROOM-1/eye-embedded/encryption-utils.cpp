#include "encryption-utils.h"
#include <AES.h>
#include <string.h>
#include <sstream>
#include <iomanip>
#include <string>
#include <iostream>

bool KEY_IS_VALID = false;
bool KEY_VALIDATION_CHECKED = false;
byte encrypted[512];
int global_output_len = 0;
int ENCRYPTED_JSON_LENGTH = 0;

AES256 aes;



byte _0 = 0x00;
byte _1 = 0x01;
byte _2 = 0x02;
byte _3 = 0x03;
byte _4 = 0x04;
byte _5 = 0x05;
byte _6 = 0x06;
byte _7 = 0x07;
byte _8 = 0x08;
byte _9 = 0x09;
byte _a = 0x0a;
byte _b = 0x0b;
byte _c = 0x0c;
byte _d = 0x0d;
byte _e = 0x0e;
byte _f = 0x0f;

// if (string_key.c_str()[0] = 'a' ){
//   boof[0] = _a;
// }

// byte key[32] = {
//   0x60, 0x3d, 0xeb, 0x10, 0x15, 0xca, 0x71, 0xbe,
//   0x2b, 0x73, 0xae, 0xf0, 0x85, 0x7d, 0x77, 0x81,
//   0x1f, 0x35, 0x2c, 0x07, 0x3b, 0x61, 0x08, 0xd7,
//   0x2d, 0x98, 0x10, 0xa3, 0x09, 0x14, 0xdf, 0xf4
// };

byte key[32] = {
  0x00
};



// byte k[64] = {
//   0x0, 0x1, 0x2, 0x3, 0x4, 0x5, 0x6, 0x7,
//   0x8, 0x9, 0xa, 0xb, 0xc, 0xd, 0xe, 0xf,
//   0x1, 0x3, 0x5, 0x7, 0x9, 0xb, 0xd, 0xf,
//   0x0, 0x2, 0x4, 0x6, 0x8, 0xa, 0xc, 0xe,
//   0x0, 0x4, 0x8, 0xc, 0x1, 0x5, 0x9, 0xd,
//   0x2, 0x6, 0xa, 0xe, 0x3, 0x7, 0xb, 0xf,
//   0x0, 0x1, 0x2, 0x3, 0x4, 0x5, 0x6, 0x7,
//   0x8, 0x9, 0xa, 0xb, 0xc, 0xd, 0xe, 0xf
// };


bool make_hex_key(String strKey){
  if(strKey.length() != 64) 
  {
    Serial.println(strKey.length());
    Serial.println("PROVIDED KEY LENGTH SHOULD BE 64 HEX DIGITS!");
    return false;
  }

  for(int i = 0; i < 64; i+=2)
  {
    byte l = (0x00);
    byte r = (0x00);
    switch (strKey[i]) {
      case '0': l = _0; break;
      case '1': l = _1; break;
      case '2': l = _2; break;
      case '3': l = _3; break;
      case '4': l = _4; break;
      case '5': l = _5; break;
      case '6': l = _6; break;
      case '7': l = _7; break;
      case '8': l = _8; break;
      case '9': l = _9; break;
      case 'a': case 'A': l = _a; break;
      case 'b': case 'B': l = _b; break;
      case 'c': case 'C': l = _c; break;
      case 'd': case 'D': l = _d; break;
      case 'e': case 'E': l = _e; break;
      case 'f': case 'F': l = _f; break;
      default: l = 0x00; break; 
    } 

    switch (strKey[i+1]) {
      case '0': r = _0; break;
      case '1': r = _1; break;
      case '2': r = _2; break;
      case '3': r = _3; break;
      case '4': r = _4; break;
      case '5': r = _5; break;
      case '6': r = _6; break;
      case '7': r = _7; break;
      case '8': r = _8; break;
      case '9': r = _9; break;
      case 'a': case 'A': r = _a; break;
      case 'b': case 'B': r = _b; break;
      case 'c': case 'C': r = _c; break;
      case 'd': case 'D': r = _d; break;
      case 'e': case 'E': r = _e; break;
      case 'f': case 'F': r = _f; break;
      default: r = 0x00; break;
    }


    byte left = l << 4;
    byte lr = left | r;
    key[i/2] = lr;
    
  }
  return true;
}



byte plain[32];


String hex_to_string(int len){
  String hexString = "";
  for (int i = 0; i < len; i++){
    if(encrypted[i] < 16) hexString += "0";
    hexString += String(encrypted[i], HEX);
  }
  return hexString;
}





void encrypt_value(const char* inputText, byte* encrypted, int& outputLen) {

if(sizeof(key) == 32){
  int inputLen = strlen(inputText);
  int paddedLen = inputLen + (16 - (inputLen % 16));
  byte* padded = (byte*)malloc(paddedLen);
  if (!padded) {
      Serial.println("( ERROR: NO MEMORY FOR BUFFER PADDED ! )");
      outputLen = 0;
      return;
    }
  int padVal = 16 - (inputLen % 16);
  memcpy(padded, inputText, inputLen);
    for (int i = 0; i < padVal; i++) {
      padded[inputLen + i] = padVal;
    }
    aes.setKey(key, sizeof(key));

    for (int i = 0; i < paddedLen; i += 16) {
      aes.encryptBlock(encrypted + i, padded + i);
    }
    outputLen = paddedLen;
    ENCRYPTED_JSON_LENGTH = outputLen;
    free(padded);
  }
else{
  Serial.println("( ERROR: INVALID KEY SIZE ! )");
  }
}

// String make_json_to_send(String command, String data) {
//   return String(
//         "{"
//         " \"dev_uuid\":\"" + UUID + "\"," 
//         " \"dev_addr\":\"" + myIP + "\"," 
//         " \"dev_mac\":\"" + MAC_ADDRESS + "\"," 
//         " \"time_epoch\":" + String(TIME_EPOCHMS) + "," 
//         " \"command\": \"" + command + "\"," 
//         " \"data\": {" + data +
//         "}"
//         "}" 
//     );
// }





void decrypt_value(const byte* encrypted, int inputLen, char* outputText) {
  if (sizeof(key) != 32) {
    Serial.println("( ERROR: INVALID KEY SIZE ! )");
    return;
  }

  if (inputLen % 16 != 0) {
    Serial.println("( ERROR: ENCRYPTED DATA LENGTH IS NOT A MULTIPLE OF 16 ! )");
    return;
  }

  byte* decrypted = (byte*)malloc(inputLen);
  if (!decrypted) {
    Serial.println("( ERROR: NO MEMORY FOR BUFFER DECRYPTED ! )");
    return;
  }

  aes.setKey(key, sizeof(key));

  for (int i = 0; i < inputLen; i += 16) {
    aes.decryptBlock(decrypted + i, encrypted + i);
  }

  int padVal = decrypted[inputLen - 1];

  if (padVal < 1 || padVal > 16) {
    Serial.println("( ERROR: INVALID PADDING ! )");
    free(decrypted);
    return;
  }

  int realLen = inputLen - padVal;


  memcpy(outputText, decrypted, realLen);
  outputText[realLen] = '\0';
  Serial.print(" DECRYPTED MESSAGE: ");
  Serial.println(outputText);

  free(decrypted);
}


void reset_key_validation() {
  KEY_VALIDATION_CHECKED = false;
  KEY_IS_VALID = false;
  validate_key();
}

bool is_hex(const String &str) {
    for (unsigned int i = 0; i < str.length(); i++) {
        char c = str.charAt(i);
        if (!((c >= '0' && c <= '9') || (c >= 'a' && c <= 'f') || (c >= 'A' && c <= 'F'))) {
            return false;
        }
    }
    return true;
}

void validate_key() {
  KEY_VALIDATION_CHECKED = true;
  KEY_IS_VALID = (ENCRYPT_KEY != "" && ENCRYPT_KEY.length() == 64 && is_hex(ENCRYPT_KEY));
  
}

bool is_key_valid() {
  return KEY_IS_VALID;
}

String encrypt_value(bool Value) {
  if (!is_key_valid()) return String();

  return String();
}
// String encrypt_value(float Value) {
//   return String();
// }
// String encrypt_value(int Value) {
//   return String();
// }
// String encrypt_value(String Value) {
//   return String();
// }
String encrypt_key(String Value) {
  return String();
}

// String decrypt_value(String Value) {
//   return String();
// }

// String decrypt_key(String Value) {
//   return String();
// }