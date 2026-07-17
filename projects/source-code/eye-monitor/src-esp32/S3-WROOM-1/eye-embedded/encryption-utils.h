#include "datatypes.h"

#ifndef ENCRYPTION_UTILS_H
#define ENCRYPTION_UTILS_H
#endif



bool is_key_valid();
void reset_key_validation();
void validate_key();

String encrypt_key(String Value);
String encrypt_value(bool Value);
String encrypt_value(float Value);
String encrypt_value(int Value);
String encrypt_value(String Value);
int padData(const byte* input, int inputLen, byte* output);
void make_hex_key(const char* inputText, char* outputHex, int& outputLen);
void hex_to_byte_array(const char* charHex, byte* outputKeyArray, int& byteArrayLen);
String hex_to_string(int len);
void encrypt_val(const char* inputText, int& outputLen);
String decrypt_val(int len);
// String decrypt_value(String Value);
String decrypt_key(String Value);
void hex_to_bytes(const String &hex_str, byte *output, size_t len);
bool is_hex(const String &str);
String bytes_to_hex(const byte *data, size_t len);

void encrypt_value(const char* inputText, byte* encrypted, int& outputLen);
void decrypt_value(const byte* encrypted, int inputLen, char* outputText);
String make_json_encrypted_to_send(String command, String data);

bool make_hex_key(String strKey);
extern bool KEY_IS_VALID;
extern bool KEY_VALIDATION_CHECKED;
extern byte key[32];
extern byte key2[32]; // do przeniesienia potem do datattypes
extern char hexadecimalKeyBuffer[128];
extern int hexadecimalKeyLength;
extern String encrypted_json;
extern String decrypted_json;
extern byte encrypted[512];


extern int global_output_len;
// extern bool IsEncryptedKeyValid;

//nowe
extern byte ENCRYPTED_BUFFER[512];
extern int ENCRYPTED_JSON_LENGTH;


