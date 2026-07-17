#include "sensor-utils.h"
#include "datatypes.h"
#include "Wire.h"
#include <map>

TwoWire myWire = TwoWire(0);

ENSSensor ens_sensor(Wire);
SCD4xSensor spk_sensor(Wire);
MLX90614Sensor mlx_sensor(Wire);

void init_sensors() {
  Wire.begin();
  ens_sensor.init();
  spk_sensor.init();
  mlx_sensor.init();
}

void update_sensors() {
  if (ens_sensor.available()) ens_sensor.update();
  if (spk_sensor.available()) spk_sensor.update();
  if (mlx_sensor.available()) mlx_sensor.update();
}

String get_mlx90614() {
  if (!mlx_sensor.available()) return String("X");
  return "{"
          "\"OBJ_TEMPERATURE\":" + String(mlx_sensor.currentState[0]) + ","
          "\"AMB_TEMPERATURE\":" + String(mlx_sensor.currentState[1]) +
          "}";
}

String get_ens160() {
if (!ens_sensor.available()) return String("X");
return "{"
        "\"AQI\":" + String(ens_sensor.currentState[0]) + ","
        "\"TVOC\":" + String(ens_sensor.currentState[1]) + ","
        "\"ECO2\":" + String(ens_sensor.currentState[2]) +
        "}";
}

String get_senscdx4x() {
if (!spk_sensor.available()) return String("X");
return "{"
        "\"CO2\":" + String(spk_sensor.currentState[0]) + ","
        "\"TEMPERATURE\":" + String(spk_sensor.currentState[1]) + ","
        "\"HUMIDITY\":" + String(spk_sensor.currentState[2]) +
        "}";
}

String make_json_to_send(String command, String data) {
  return String(
        "{"
        " \"dev_uuid\":\"" + UUID + "\"," 
        " \"dev_addr\":\"" + myIP + "\"," 
        " \"dev_mac\":\"" + MAC_ADDRESS + "\"," 
        " \"time_epoch\":" + String(TIME_EPOCHMS) + "," 
        " \"command\": \"" + command + "\"," 
        " \"data\": {" + data +
        "}"
        "}" 
    );
}

String make_json_encrypted_to_send(String data) {
  return String(
        "{"
       
        " \"dev_mac\":\"" + MAC_ADDRESS + "\"," 
        " \"data\": {" + data +
        "}"
        "}" 
    );
}


String add_encrypt_prefix_json(String command, String data){
  return String(
    "{"
    " \"dev_uuid\":\"" + UUID + "\"," 
    " \"time_epoch\":" + String(TIME_EPOCHMS) + "," 
     " \"dev_addr\":\"" + myIP + "\","
     " \"command\": \"" + command + "\"," 
    "\"encrypted\":\"" + data + "\""
    "}"
  );
}

String read_sensors_as_json() {
  return String(
        "\"sensors\": [" +
              ens_sensor.read() + "," +
              spk_sensor.read() + "," +
              mlx_sensor.read() + 
        "]"
    );
}

/*   SENSORS    */

void Sensor::init() {}
void Sensor::update() {}
String Sensor::read() {return "";}
bool Sensor::available() {return false;}

/* - - - */

ENSSensor::ENSSensor(TwoWire& wireRef) : wire(wireRef) {
  ens160 = new DFRobot_ENS160_I2C(&wire, 0x53);
  this->COM_INTERFACE = COMM_I2C;
  this->DEVICE_TYPE = DEV_ENS160;
}

void ENSSensor::init() {
  if (ens160->begin() == 0) {  
    FOUND = true;
  } else {
    FOUND = false;
  }
}
void ENSSensor::update() {
  if (!FOUND) return;

  ens160->getENS160Status();
  currentState[0] = ens160->getAQI();
  currentState[1] = ens160->getTVOC();
  currentState[2] = ens160->getECO2();
}

String ENSSensor::read() {
  if (!FOUND) return "{}";

  return String("{\"name\":\"" + String(DEVICE_NAMES[this->DEVICE_TYPE]) + "\"," +
         "\"readings\":{" +
         "\"aqi\":"+ String(currentState[0]) +
         ",\"tvoc\":" + String(currentState[1]) +
         ",\"eco2\":" + String(currentState[2]) + "}}");
}

bool ENSSensor::available() {
  return FOUND;
}

/* - - - */

SCD4xSensor::SCD4xSensor(TwoWire& wireRef) : wire(wireRef) {
  this->COM_INTERFACE = COMM_I2C;
  this->DEVICE_TYPE = DEV_SEN22396;
}

void SCD4xSensor::init() {
  if (scd4x.begin(wire) != true) {
    FOUND = false;
  } else {
    scd4x.setAutomaticSelfCalibrationEnabled(true);
    // scd4x.startContinuousMeasurement();
    FOUND = true;
  }
}

void SCD4xSensor::update() {
  if (!FOUND) return;


    if (scd4x.readMeasurement());
    currentState[0] = scd4x.getCO2();
    currentState[1] = scd4x.getTemperature();
    currentState[2] = scd4x.getHumidity();
  
}

String SCD4xSensor::read() {
  if (!FOUND) return "{}";

  return String("{\"name\":\"" + String(DEVICE_NAMES[this->DEVICE_TYPE]) + "\"," +
         "\"readings\":{" +
         "\"co2\":"+ String(currentState[0]) +
         ",\"temperature\":" + String(currentState[1]) +
         ",\"humidity\":" + String(currentState[2]) + "}}");
}

bool SCD4xSensor::available() {
  return FOUND;
}

/* - - - */

MLX90614Sensor::MLX90614Sensor(TwoWire& wireRef) : wire(wireRef) {
  this->COM_INTERFACE = COMM_I2C;
  this->DEVICE_TYPE = DEV_MLX90614;
}

void MLX90614Sensor::init() {
  if (!mlx90614.begin(MLX90614_I2CADDR, &wire)) {
    FOUND = false;
  } else {
    FOUND = true;
  }
}

void MLX90614Sensor::update() {
  if (!FOUND) return;

  currentState[0] = mlx90614.readObjectTempC();
  currentState[1] = mlx90614.readAmbientTempC();
}

String MLX90614Sensor::read() {
  if (!FOUND) return "{}";
  return String("{\"name\":\"" + String(DEVICE_NAMES[this->DEVICE_TYPE]) + "\"," +
         "\"readings\":{" +
         "\"object_temperature\":"+ String(currentState[0]) +
         ",\"ambient_temperature\":" + String(currentState[1]) + "}}");
}

bool MLX90614Sensor::available() {
  return FOUND;
}

