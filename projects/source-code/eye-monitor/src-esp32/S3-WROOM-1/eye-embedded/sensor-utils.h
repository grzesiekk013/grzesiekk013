#include <Wire.h>
#include <SparkFun_SCD4x_Arduino_Library.h>
#include <Adafruit_MLX90614.h>
#include <DFRobot_ENS160.h>
#include "datatypes.h"

#ifndef SENSOR_UTILS_H
#define SENSOR_UTILS_H


void init_sensors();
void update_sensors();
String read_sensors_as_json();
String make_json_to_send(String command, String data);
String make_json_encrypted_to_send(String data);
String add_encrypt_prefix_json(String command, String data);
String get_mlx90614();
String get_ens160();
String get_senscdx4x();

class Sensor
{
  public:
    virtual void init() = 0;
    virtual String read() = 0;
    virtual void update() = 0;
    virtual bool available() = 0;
    float currentState[16];

    E_DEVICE_TYPE DEVICE_TYPE;
    E_COM_INTERFACE COM_INTERFACE;

    bool FOUND = false;
    int DEVICE_NUM = -1;
    int DEVICE_ID = -1;   
};

class ENSSensor : public Sensor {
  public:
    ENSSensor(TwoWire& wireRef);
    void init() override;
    void update() override;
    String read() override;
    bool available() override;
  
  private:
    DFRobot_ENS160_I2C* ens160; 
    TwoWire& wire;
};

class SCD4xSensor : public Sensor {
  public:
    SCD4xSensor(TwoWire& wireRef);
    void init() override;
    void update() override;
    String read() override;
    bool available() override;

  private:
    SCD4x scd4x;
    TwoWire& wire;
};

class MLX90614Sensor : public Sensor {
  public:
    MLX90614Sensor(TwoWire& wireRef);
    void init() override;
    void update() override;
    String read() override;
    bool available() override;

  private:
    Adafruit_MLX90614 mlx90614;
    TwoWire& wire;
};

#endif