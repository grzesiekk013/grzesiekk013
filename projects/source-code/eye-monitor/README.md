# EYE Project - Environmental Monitoring System
> *Advanced IoT system for measuring, acquiring, and visualizing environmental data.*

## About the Project
Project EYE is an environmental monitoring system designed for rooms intended for study, work, or relaxation. Its primary goal is to improve overall comfort by automatically reacting when established parameter thresholds are exceeded. The system continuously reads data from sensors, transmits it to a central server from multiple measurement devices, visualizes the data, and allows users to define specific events triggered by changing environmental conditions.

## Key Features
* **Secure Communication:** The ESP32-S3 microcontroller connects to the server via WiFi, utilizing a REST API with encrypted data transmission.
* **Embedded Software:** Custom firmware for the ESP32 written entirely in C++.
* **Easy Configuration:** A fully configurable system managed directly through a built-in serial interface.
* **Web Application:** A comprehensive dashboard equipped with robust security mechanisms and room organization capabilities.
* **Scalability:** The backend server is designed to simultaneously receive and process data from multiple measuring devices.
* **Tech Stack:** Powered by a MySQL database engine with both Backend and Frontend developed in Django.

---

## Hardware & Technologies
* **Web Framework:** Django 
* **Database:** MySQL 
* **Microcontroller:** FireBeetle 2 ESP32-S3-WROOM 
* **Central Server:** OrangePi Zero 3 4GB (Linux Server) 
* **Sensors:**
  * **ENS160** – Air quality sensor 
  * **GY-906** – Contactless temperature sensor 
  * **SEN-22396** – CO2, humidity, and temperature sensor 

---

## System Architecture

### General Architecture
<p align="center">
  <img width="560" height="410" alt="image" src="https://github.com/user-attachments/assets/53d9c339-925d-4cb4-aff6-0a82ff64cf77" />
</p>

### Communication Flow
<p align="center">
  <img width="560" height="410" alt="image" src="https://github.com/user-attachments/assets/85c3caa0-8570-4489-a353-b58d84874b9a" />

</p>

### ESP32 Architecture
The embedded system architecture relies on multithreading, dividing specific tasks between the two cores of the processor to ensure uninterrupted sensor reading and network communication.
<p align="center">
  <img width="475" height="431" alt="image" src="https://github.com/user-attachments/assets/e19fd81a-fd3d-4b1e-b996-3fede5fb998b" />
  <img width="369" height="316" alt="image" src="https://github.com/user-attachments/assets/f576fa68-8f7b-4797-9fcb-efdc28345cfa" />
</p>

### Database Schema (ERD)
<p align="center">
  <img width="621" height="714" alt="image" src="https://github.com/user-attachments/assets/9b3634ad-d2d8-4b6d-be69-6733d583aaab" />

</p>

---

## Embedded Libraries (ESP32)
The microcontroller firmware utilizes a variety of specialized libraries for hardware interaction and network protocols:
* `Preferences` – For saving configuration data in the non-volatile Flash memory.
* `Esp32ping` – For pinging devices on the local network.
* `time` – For timekeeping and NTP server synchronization.
* `Aes` – For symmetric data encryption.
* `Wire` – For I2C sensor communication.
* `WiFi` / `HTTPClient` – For establishing router connections and sending HTTP requests to the Django server.
* `thread` – For handling dual-core multithreading operations.
* **Sensor Libraries:** `SparkFun_SCD4x`, `Adafruit_MLX90614`, `DFRobot_ENS160`.
* **Utility Libraries:** `string`, `sstream`, `iomanip`, `iostream`, `Arduino`, `map`.

---

## Serial Interface (ESP32 Configuration)
The EYE device features a custom built-in terminal (serial interface) that simplifies the configuration of network parameters, encryption keys, and system diagnostics prior to deployment.
<br><br>
**Help Panel:**
<br><br>
<img width="483" height="266" alt="image" src="https://github.com/user-attachments/assets/2ef05abb-495a-47b7-8279-fbebcfc097ee" />
<br><br>

**WiFi Connection Status:**
<br><br>
<img width="450" height="102" alt="image" src="https://github.com/user-attachments/assets/8f65d51c-9790-4465-822c-05a88130cf0f" />
<br><br>

**Time Synchronization Status:**
<br><br>
<img width="450" height="133" alt="image" src="https://github.com/user-attachments/assets/5c75c3f6-fb7d-432c-aeee-3da9aba99a66" />
<br><br>

**Device Information:**
<br><br>
<img width="450" height="145" alt="image" src="https://github.com/user-attachments/assets/d7f5460c-7886-4bb6-b13a-38255a6818d4" />

<br><br>
**Server Connection Status:**
<br><br>
<img width="450" height="153" alt="image" src="https://github.com/user-attachments/assets/2377df03-4a6e-4aec-b864-75b01cdd1108" />
<br><br>

**Sensor Readings Status:**
<br><br>
<img width="450" height="147" alt="image" src="https://github.com/user-attachments/assets/7f8aa272-7df9-40cc-ad96-89be323e9682" />
<br><br>

---

## Data Payload Structure
<br><br>
Data collected by the microcontroller is formatted into JSON packets before being transmitted to the server. To ensure data integrity and security, the device utilizes an onboard cryptographic module for symmetric AES encryption.
<br><br>
**Unencrypted JSON Body:**
<br><br>
<img width="420" height="360" alt="image" src="https://github.com/user-attachments/assets/5d5a14bb-0cfa-42e5-a12f-b522e5238a0d" />
<br><br>

**Encrypted JSON Body (AES):**
<br><br>
<img width="420" height="220" alt="image" src="https://github.com/user-attachments/assets/1e8d1bb0-a931-4ed0-9cfc-e232424f3526" />
<br><br>

---

## Web Dashboard (Django App)
System management, automated action definitions, and historical measurement reviews (categorized by rooms) are handled through a dedicated web application. It features two display modes: a simplified view (descriptive evaluation, e.g., "Good") and an advanced view (detailed historical charts).

<p align="center">
  <img width="750" height="500" alt="image" src="https://github.com/user-attachments/assets/6f1e123d-5d2d-4948-ac57-b71807422220" />
  <img width="750" height="500" alt="image" src="https://github.com/user-attachments/assets/4cb5b6e3-379e-47f4-bb40-e65ea65886b5" />
  <img width="750" height="500" alt="image" src="https://github.com/user-attachments/assets/88f4791b-84c5-49a0-a698-7d3e243541f2" />
  <img width="750" height="500" alt="image" src="https://github.com/user-attachments/assets/acea1940-ac50-4d3e-88cf-b6dde96ac5b2" />
  <img width="750" height="500" alt="image" src="https://github.com/user-attachments/assets/7e525eea-84cc-4b6c-87f7-6f579520bd70" />


</p>

---
> **Project Authors:** Grzegorz Lademann, Kamil Grzybek, Jakub Lidzbarski, Jarosław Wojciuk.
